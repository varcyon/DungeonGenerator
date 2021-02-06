using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RoomsDatabase", menuName = "ScriptableObjects/RoomsDatabase")]
public class RoomsDatabase : ScriptableObject
{
    public List<GameObject> savedDungeons;
    public GameObject startRoom;
    public List<GameObject> roomsList = new List<GameObject>();

    public List<bool> levelHasStairCase = new List<bool>(); 

    public List<GameObject> topRooms = new List<GameObject>();
    public List<GameObject> bottomRooms = new List<GameObject>();
    public List<GameObject> leftRooms = new List<GameObject>();
    public List<GameObject> rightRooms = new List<GameObject>();

    public List<GameObject> topSCRooms = new List<GameObject>();
    public List<GameObject> bottomSCRooms = new List<GameObject>();
    public List<GameObject> leftSCRooms = new List<GameObject>();
    public List<GameObject> rightSCRooms = new List<GameObject>();

    public List<GameObject> stairCaseToppers = new List<GameObject>();

    public List<GameObject> topTrapRooms = new List<GameObject>();
    public List<GameObject> bottomTrapRooms = new List<GameObject>();
    public List<GameObject> leftTrapRooms = new List<GameObject>();
    public List<GameObject> rightTrapRooms = new List<GameObject>();


    [HideInInspector] public List<RoomSpawner> roomSpawners = new List<RoomSpawner>();
    [HideInInspector] public List<RoomPath> roomDestroyers = new List<RoomPath>();
}
