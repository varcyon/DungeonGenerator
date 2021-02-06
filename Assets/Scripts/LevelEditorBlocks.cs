using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LevelBlockData {
    public string blockName;
    public ObjectTags objectTags = ObjectTags.None;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "LevelEditorPrefabDB", menuName = "ScriptableObjects/Level Editor Prefab DB")]
public class LevelEditorBlocks : ScriptableObject {
    public GameObject selectedBlock;
    public List<LevelBlockData> blocks = new List<LevelBlockData>();
}
public enum ObjectTags {
    None,
    Block,
    Interactive,
    Decorative
}