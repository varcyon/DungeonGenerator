using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DungeonCreatorWizard : EditorWindow
{
    static GUISkin skin;
    static RoomsDatabase rooms;
    static Material endRoomMat;
    static bool assetsFound;
    static List<GameObject> roomSpawners;
    bool roomsFinished;
    GameObject activeDungeon;
    string dungeonName;
    bool finished;
    public bool possibleMultiLevel;
    public int maxLevels =0;
    public float newLevelChance;
    public bool trapRooms;
    public float trapPercentage = 0;
    public List<RoomPath> roomDestroyersDis = new List<RoomPath>();
    int lastSpawnerCount;
    int currentSpawnerCount;
    [MenuItem("Varcyon Sariou Games/Create Dungeon Wizard")]
    static void CreateDungeonCreator()
    {
        GetWindow<DungeonCreatorWizard>(false, "Dungeon Creator");
        rooms = AssetDatabase.LoadAssetAtPath<RoomsDatabase>("Assets/DungeonGenerator/ScriptableObjects/RoomsDatabase.asset");
        endRoomMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/DungeonGenerator/Materials/EndRoomMat.mat");
        skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/DungeonGenerator/DungeonGeneratorSkin.guiskin");
        assetsFound = true;
    }
    private void OnEnable()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    private void OnUpdate()
    {
        if (assetsFound && rooms.roomSpawners.Count > 0)
        {
            SpawnRooms();
        }

        if (GameObject.FindGameObjectWithTag("StartRoom") && lastSpawnerCount == currentSpawnerCount)
        {
            roomsFinished = true;
        }
        Repaint();
        SceneView.RepaintAll();
    }



    private void OnGUI()
    {
        dungeonName = EditorGUILayout.TextField("Dungeon Name", dungeonName);
        possibleMultiLevel = EditorGUILayout.Toggle("Possible multiple levels?", possibleMultiLevel);
        if (possibleMultiLevel) {
            GUILayout.Label("Max amount of levels. 0 for unlimited");
            maxLevels = (int)EditorGUILayout.Slider(maxLevels,0,20);
            GUILayout.Label("Percent chance of a new level.");

            newLevelChance = (int) EditorGUILayout.Slider(newLevelChance, 0f, 100f);
        }
        trapRooms = EditorGUILayout.Toggle("Chance of a trap room.", trapRooms);
        if (trapRooms)
        {
            trapPercentage =(int) EditorGUILayout.Slider(trapPercentage, 0f, 100f);
        }


        if (GUILayout.Button("Create/Reroll"))
        {
            if (dungeonName == null)
            {
                EditorUtility.DisplayDialog("Error", "Please enter a name for the Dungeon", "FINE!");
                return;
            }
            rooms.levelHasStairCase.Clear();
            rooms.levelHasStairCase.Add(new bool());
            finished = false;
            roomsFinished = false;
            rooms.roomsList.Clear();
            DestroyImmediate(GameObject.FindGameObjectWithTag("StartRoom"));
            activeDungeon = PrefabUtility.InstantiatePrefab(rooms.startRoom) as GameObject;
            activeDungeon.name = dungeonName;
            Selection.activeObject = activeDungeon;
            rooms.roomSpawners.Clear();
            rooms.roomSpawners = FindObjectsOfType<RoomSpawner>().ToList();
            foreach (RoomSpawner spawner in rooms.roomSpawners)
            {
                spawner.trapRoomChance = trapPercentage;
                spawner.multiLevel = possibleMultiLevel;
                spawner.maxLevel = maxLevels;
                spawner.stairCaseChance = newLevelChance;
                spawner.SpawnRoom();
            }
            foreach (GameObject room in rooms.roomsList)
            {
                room.tag = "DontDestroyMe";
            }
            rooms.roomSpawners.Clear();
            rooms.roomSpawners = FindObjectsOfType<RoomSpawner>().ToList();
        }
        if (GUILayout.Button("Update Dungeon Name"))
        {
            activeDungeon.name = dungeonName;
        }

        if (GUILayout.Button(""))
        {

        }
        GUILayout.Label("Once rooms finish spawning finalize appears.");
        GUILayout.Label("Press finalize to create boss room and clean up");
        if (roomsFinished)
        {
            if(GUILayout.Button("Show Path")) {
                RoomPath[] roomPaths = FindObjectsOfType<RoomPath>();
                foreach (RoomPath path in roomPaths) {
                    path.DrawPath();
                }
            }






            if (GUILayout.Button("Finalize", skin.GetStyle("FinalizeBTN")))
            {
                MeshRenderer[] renderers = rooms.roomsList[rooms.roomsList.Count - 1].transform.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer r in renderers)
                {
                    r.material = endRoomMat;
                }
                RoomSpawner[] spawners = FindObjectsOfType<RoomSpawner>();
                foreach (RoomSpawner go in spawners)
                {
                    DestroyImmediate(go);
                }
                /*
                RoomPath[] destroyers = FindObjectsOfType<RoomPath>();
                foreach (RoomPath go in destroyers)
                {
                    DestroyImmediate(go);
                }
                */
                DestroyImmediate(FindObjectOfType<Destroyer>());
                rooms.roomDestroyers.Clear();
                finished = true;
            }
        }
        if (rooms.savedDungeons != null)
        {
            GUILayout.Label("Dugeons to Load");
            List<string> templist = new List<string>();
            GUILayout.BeginScrollView(new Vector2(0, 0));
            foreach (GameObject dungeon in rooms.savedDungeons)
            {

                GUILayout.Space(5);
                if (GUILayout.Button(dungeon.name, skin.GetStyle("buttonList")))
                {
                    finished = false;
                    roomsFinished = false;
                    DestroyImmediate(GameObject.FindGameObjectWithTag("StartRoom"));
                    GameObject go = Instantiate(dungeon);
                    go.name = dungeon.name;
                    dungeonName = dungeon.name;
                    Repaint();
                }
            }
            GUILayout.Space(5);
            GUILayout.EndScrollView();


        }
        if (finished)
        {
            if (GUILayout.Button("Save"))
            {
                if (!rooms.savedDungeons.Contains(activeDungeon))
                {
                    GameObject go = PrefabUtility.SaveAsPrefabAsset(activeDungeon, "Assets/DungeonGenerator/SavedDungeons/" + dungeonName + ".prefab");
                    if (!rooms.savedDungeons.Contains(go))
                        rooms.savedDungeons.Add(go);

                }
            }
        }

        if (GameObject.FindGameObjectWithTag("StartRoom") && GUILayout.Button("Destroy Dungeon"))
        {
            finished = false;
            roomsFinished = false;
            DestroyImmediate(GameObject.FindGameObjectWithTag("StartRoom"));
            dungeonName = "";
        }


        Repaint();

    }

    private void SpawnRooms()
    {
        rooms.roomSpawners.Clear();
        rooms.roomSpawners = FindObjectsOfType<RoomSpawner>().ToList();
        lastSpawnerCount = rooms.roomSpawners.Count;
        foreach (RoomSpawner spawner in rooms.roomSpawners)
        {
            spawner.trapRoomChance = trapPercentage;
            spawner.multiLevel = possibleMultiLevel;
            spawner.maxLevel = maxLevels;
            spawner.stairCaseChance = newLevelChance;
            spawner.SpawnRoom();
        }
        rooms.roomSpawners.Clear();
        rooms.roomSpawners = FindObjectsOfType<RoomSpawner>().ToList();
        currentSpawnerCount = rooms.roomSpawners.Count;
    }
}
