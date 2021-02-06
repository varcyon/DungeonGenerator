
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad, ExecuteInEditMode]
public class DungeonEditor : Editor {
    public static Vector3 currentHandlePos = Vector3.zero;
    static Vector3 oldHandlePos = Vector3.zero;
    public static bool isMouseInValidArea;

    static RaycastHit hit = new RaycastHit();
    static float handleSizeHalfExtent = 0.125f;
    static Color paintColor = Color.yellow;
    static Color eraseColor = Color.white;
    static Color wireCubeColor;
    static LevelEditorBlocks LEBs;
    static Vector2 scrollPos;
    static GameObject blockBrush;
    static bool blockBrushCreated;
    static bool canRotate = true;
    static int rotateAmount = 90;
    static Material blockBrushMat;
    public static int selectedBlock {
        get { return EditorPrefs.GetInt("SelectedEditorBlock", 0); }
        set { EditorPrefs.SetInt("SelectedEditorBlock", value); }
    }

    public static int selectedTag {
        get { return EditorPrefs.GetInt("SelectedTag", 0); }
        set { if(value == selectedTag) { return; }
            EditorPrefs.SetInt("SelectedTag", value);    
        }
    }
    public static int selectedTool {
        get { return EditorPrefs.GetInt("SelectedEditorTool", 0); }
        set {
            if (value == selectedTool) { return; }
            EditorPrefs.SetInt("SelectedEditorTool", value);

            switch (value) {
                case 0:
                    EditorPrefs.SetBool("isDungeonEditorEnabled", false);
                    Tools.hidden = false;
                    break;
                case 1:
                    EditorPrefs.SetBool("isDungeonEditorEnabled", true);
                    EditorPrefs.SetBool("SelectBlockNextToMousePos", false);
                    wireCubeColor = eraseColor;
                    Tools.hidden = true;
                    break;
                case 2:
                    EditorPrefs.SetBool("isDungeonEditorEnabled", true);
                    EditorPrefs.SetBool("SelectBlockNextToMousePos", true);
                    wireCubeColor = paintColor;
                    Tools.hidden = true;
                    break;
            }
        }
    }

    static DungeonEditor() {
       Scene scene= SceneManager.GetActiveScene();
       //TURN on Dungeon editor for specific scenes
        // if(scene.name != "Main" ) { return; }
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
        LEBs = AssetDatabase.LoadAssetAtPath<LevelEditorBlocks>("Assets/DungeonGenerator/ScriptableObjects/LevelEditorPrefabDB.asset");
        blockBrushMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/DungeonGenerator/Materials/BlockBrushMat.mat");
        EditorPrefs.SetInt("SelectedEditorBlock", 0);
        EditorPrefs.SetInt("SelectedEditorTool", 0);
        EditorPrefs.SetBool("isDungeonEditorEnabled", true);
        Tools.hidden = false;
    }
    void OnDestroy() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sv) {
        //Draws bottom tool bar
        DrawEditorMenu(sv.position);
        bool isDungeonEditorEnabled = EditorPrefs.GetBool("isDungeonEditorEnabled");

        if (!isDungeonEditorEnabled) { return; }
        //If the paint tool is selected then the object selection is shown
        if (selectedTool == 2)
            DrawSelectionButtons(sv);

        //By creating a new ControlID here we can grab the mouse input to the SceneView and prevent Unitys default mouse handling from happening
        //FocusType.Passive means this control cannot receive keyboard input since we are only interested in mouse input
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);

        //handles the position of the wireframe / mouse position
        UpdateHandlePos();
        //checks if the mouse is in the valid position IE. not over one of the GUIs
        UpdateIsMouseInValidArea(sv.position);
        //Repaints the scene
        UpdateRepaint();
        //Draws the wireframe tool and handles the snapping
        DrawCube(currentHandlePos);
        //If erase is selected you can remove a block
        if (selectedTool == 1) {
            RemoveSelected();
        }
        //if paint is selected you can add a block
        if (selectedTool == 2) {
            AddSelected();
        }
        //debug helper to see if we are grabbing the right block
        LEBs.selectedBlock = LEBs.blocks[selectedBlock].prefab ? LEBs.blocks[selectedBlock].prefab : LEBs.blocks[0].prefab;
    }

    private static void DrawSelectionButtons(SceneView sv) {
        Handles.BeginGUI();
        GUI.Box(new Rect(10, 25, 115, sv.position.height - 80), GUIContent.none, EditorStyles.textArea);
        scrollPos = GUI.BeginScrollView(new Rect(10, 25, 115, sv.position.height - 80), scrollPos, new Rect(10, 10, 100, 128 * LEBs.blocks.Count));
        for (int i = 0; i < LEBs.blocks.Count; i++) {
            DrawSelectionButton(i, sv.position);
        }
        GUI.EndScrollView();
        Handles.EndGUI();
    }

    private static void DrawSelectionButton(int index, Rect sceneViewRect) {
        bool isActive = false;
        if (selectedTool == 2 && index == selectedBlock) {
            isActive = true;
        }

        Texture2D previewImage = AssetPreview.GetAssetPreview(LEBs.blocks[index].prefab);
        GUIContent buttonContent = new GUIContent(previewImage);

        GUI.Label(new Rect(15, index * 128 + 10, 90, 20), LEBs.blocks[index].blockName);
        bool isToggled = GUI.Toggle(new Rect(15, index * 128 + 30, 90, 90), isActive, buttonContent, GUI.skin.button);

        if (isToggled && !isActive) {
            selectedBlock = index;
            handleSizeHalfExtent = LEBs.blocks[selectedBlock].prefab.transform.localScale.y / 2;
        }

    }

    private static void RemoveSelected() {
        if (Event.current.type == EventType.MouseDown &&
           Event.current.button == 0 && !Event.current.alt) {
            if (isMouseInValidArea) {
                if (hit.transform != null) {
                    Transform room = hit.transform.gameObject.transform.parent;
                    List<GameObject> ItemsAdded = new List<GameObject>();
                    for (int i = 0; i < room.transform.childCount; i++) {
                        if (room.transform.GetChild(i).gameObject.layer == LayerMask.NameToLayer("AddedRoomItems")) {
                            ItemsAdded.Add(room.transform.GetChild(i).gameObject);
                        }
                    }
                    foreach (GameObject item in ItemsAdded) {
                        if (hit.transform.gameObject == item) {
                            Undo.DestroyObjectImmediate(item);
                            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                            return;
                        }
                    }
                }
            }
        }
    }

    private static void DrawEditorMenu(Rect position) {
        Handles.BeginGUI();
        if (selectedTool == 2) {
            GUILayout.BeginArea(new Rect(0, 0, position.width, 50), EditorStyles.toolbar);
            string[] buttonLabels = new string[] { "Blocks", "Interactive", "Objects" };
            selectedTag = GUILayout.SelectionGrid(
                selectedTag,
                buttonLabels,
                3,
                EditorStyles.toolbarButton,
                GUILayout.Width(300));

            GUILayout.EndArea();
        }

        GUILayout.BeginArea(new Rect(0, position.height - 45, position.width, 50), EditorStyles.toolbar);
        {
            string[] buttonLabels2 = new string[] { "None", "Erase", "Paint" };
            selectedTool = GUILayout.SelectionGrid(
                selectedTool,
                buttonLabels2,
                3,
                EditorStyles.toolbarButton,
                GUILayout.Width(300));
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void AddSelected() {
        if (Event.current.type == EventType.MouseDown &&
            Event.current.button == 0 &&
            !Event.current.alt) {
            if (isMouseInValidArea) {
                //for testing just create primitive
                GameObject newCube = Instantiate(LEBs.blocks[selectedBlock].prefab);
                newCube.transform.parent = hit.transform.parent;
                newCube.transform.position = currentHandlePos;
                newCube.transform.eulerAngles = blockBrush.transform.eulerAngles;
                newCube.layer = LayerMask.NameToLayer("AddedRoomItems");
                Undo.RegisterCreatedObjectUndo(newCube, "Add Cube");

                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

    }

    private static void DrawCube(Vector3 center) {
        if (!isMouseInValidArea) {
            if (blockBrush != null) {
                DestroyImmediate(blockBrush);
                blockBrushCreated = false;
            }
            return;
        }
        Tools.hidden = true;
        if (!blockBrushCreated && isMouseInValidArea && selectedTool == 2) {
            blockBrush = Instantiate(LEBs.blocks[selectedBlock].prefab);
            blockBrush.GetComponentInChildren<MeshRenderer>().sharedMaterial = blockBrushMat;
            if (blockBrush.name.Contains("Corner")) {
                blockBrush.GetComponentInChildren<MeshCollider>().enabled = false;
            } else {
                blockBrush.GetComponentInChildren<BoxCollider>().enabled = false;
            }
            blockBrushCreated = true;
        } else if (blockBrushCreated && isMouseInValidArea && selectedTool == 2) {
            blockBrush.transform.position = currentHandlePos;

            if (Event.current.keyCode == KeyCode.Q && canRotate) {
                blockBrush.transform.Rotate(rotateAmount, 0f, 0f, Space.World);
                canRotate = false;
                EditorCoroutineUtility.StartCoroutineOwnerless(WaitToRotate());
            }
            if (Event.current.keyCode == KeyCode.W && canRotate) {
                blockBrush.transform.Rotate(0f, rotateAmount, 0f, Space.World);
                canRotate = false;
                EditorCoroutineUtility.StartCoroutineOwnerless(WaitToRotate());
            }
            if (Event.current.keyCode == KeyCode.E && canRotate) {
                blockBrush.transform.Rotate(0f, 0f, rotateAmount, Space.World);
                canRotate = false;
                EditorCoroutineUtility.StartCoroutineOwnerless(WaitToRotate());
            }
            
            if (Event.current.keyCode == KeyCode.A && canRotate) {
                blockBrush.transform.Rotate(-rotateAmount, 0f, 0f, Space.World);
                canRotate = false;
                EditorCoroutineUtility.StartCoroutineOwnerless(WaitToRotate());
            }
            if (Event.current.keyCode == KeyCode.S && canRotate) {
                blockBrush.transform.Rotate(0f, -rotateAmount, 0f, Space.World);
                canRotate = false;
                EditorCoroutineUtility.StartCoroutineOwnerless(WaitToRotate());
            }
            if (Event.current.keyCode == KeyCode.D && canRotate) {
                blockBrush.transform.Rotate(0f, 0f, -rotateAmount, Space.World);
                canRotate = false;
                EditorCoroutineUtility.StartCoroutineOwnerless(WaitToRotate());
            }
            
        }
    }
    private static IEnumerator WaitToRotate() {
        yield return new EditorWaitForSeconds(0.25f);
        canRotate = true;
    }
    private static void UpdateRepaint() {
        if (currentHandlePos != oldHandlePos) {
            SceneView.RepaintAll();
            oldHandlePos = currentHandlePos;
        }
    }

    private static void UpdateIsMouseInValidArea(Rect sceneViewRect) {
        bool isInValidArea = Event.current.mousePosition.y < sceneViewRect.height - 50 && Event.current.mousePosition.x > 120
            && Event.current.mousePosition.x < sceneViewRect.width - 50 && Event.current.mousePosition.y > 50;
        if (isInValidArea != isMouseInValidArea) {
            isMouseInValidArea = isInValidArea;
            SceneView.RepaintAll();
        }
    }

    private static void UpdateHandlePos() {
        if (Event.current == null) { return; }
        // annoying part... the snapping to a grid under 1m
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            if (selectedTool == 2 && !Event.current.shift) {
                currentHandlePos.y = (Mathf.Round(hit.point.y * 4) / 4) + handleSizeHalfExtent;
                if (hit.normal.x == -1) {
                    currentHandlePos.x = (Mathf.Round(hit.point.x * 4) / 4) - handleSizeHalfExtent;
                } else {
                    currentHandlePos.x = (Mathf.Round(hit.point.x * 4) / 4) + handleSizeHalfExtent;
                }
                if (hit.normal.z == -1) {
                    currentHandlePos.z = (Mathf.Round(hit.point.z * 4) / 4) - handleSizeHalfExtent;
                } else {
                    currentHandlePos.z = (Mathf.Round(hit.point.z * 4) / 4) + handleSizeHalfExtent;
                }
                /*
                if (blockBrush != null && blockBrush.transform.rotation.y % 2 != 0) {
                    blockBrush.transform.position += currentHandlePos - hit.normal * (handleSizeHalfExtent*2);
                }
                */
            } else if (selectedTool == 2 && Event.current.shift) {
                currentHandlePos = hit.point + hit.normal * handleSizeHalfExtent;

            } else if (selectedTool == 1) {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("AddedRoomItems")) {
                    handleSizeHalfExtent = hit.transform.gameObject.transform.localScale.y / 2;

                    currentHandlePos = hit.transform.position;
                    wireCubeColor.a = 1f;
                } else {
                    wireCubeColor.a = 0f;
                }
            }
        }
    }


}
