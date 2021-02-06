using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RoomSpawner : MonoBehaviour {
    public enum RoomType { Top, Bottom, Left, Right, newLevel }
    public RoomType roomType;
    public RoomsDatabase rooms;
    GameObject go;
    public bool multiLevel;
    public float trapAmmount;
    public float trapRoomChance;
    public float stairCaseChance;
    public float stairCaseAmount;
    public int maxLevel;
    public bool needsStairCaseTopper;


    public void SpawnRoom() {

        Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        foreach (Collider item in colliders) {
            if (item.gameObject.transform.parent != this.gameObject.transform.parent) {
                if (item.GetComponentInParent<RoomSpawner>() != null) {
                    break;
                } else if (item.gameObject.transform.parent.CompareTag("Room") || item.gameObject.transform.parent.CompareTag("StartRoom") || item.gameObject.transform.parent.CompareTag("Floor")) {

                    // DestroyImmediate(this);
                    return;
                }
            }
        }
        if (maxLevel == 0) { maxLevel = 999999; }
        trapAmmount = (trapRoomChance / 100);
        stairCaseAmount = (stairCaseChance / 1000);
        rooms = AssetDatabase.LoadAssetAtPath<RoomsDatabase>("Assets/DungeonGenerator/ScriptableObjects/RoomsDatabase.asset");
        float rand = Random.Range(0f, 1f);
        switch (roomType) {
            case RoomSpawner.RoomType.Top:
                if (multiLevel && rooms.levelHasStairCase.Count < maxLevel && !rooms.levelHasStairCase[(int)this.transform.position.y / 10] &&  rand <= stairCaseAmount) {
                    go = Instantiate(rooms.bottomSCRooms[UnityEngine.Random.Range(0, rooms.bottomSCRooms.Count)], this.transform);
                    rooms.levelHasStairCase[(int)this.transform.position.y / 10] = true;
                    rooms.levelHasStairCase.Add(new bool());

                } else if (rand < trapAmmount) {
                    go = Instantiate(rooms.bottomTrapRooms[UnityEngine.Random.Range(0, rooms.bottomTrapRooms.Count)], this.transform);
                } else {
                    go = Instantiate(rooms.bottomRooms[UnityEngine.Random.Range(0, rooms.bottomRooms.Count)], this.transform);
                }
                break;
            case RoomSpawner.RoomType.Bottom:
                if (multiLevel && rooms.levelHasStairCase.Count < maxLevel && !rooms.levelHasStairCase[(int)this.transform.position.y / 10] && rand <= stairCaseAmount) {
                    go = Instantiate(rooms.topSCRooms[UnityEngine.Random.Range(0, rooms.topSCRooms.Count)], this.transform);
                    rooms.levelHasStairCase[(int)this.transform.position.y / 10] = true;
                    rooms.levelHasStairCase.Add(new bool());

                } else if (rand < trapAmmount) {
                    go = Instantiate(rooms.topTrapRooms[UnityEngine.Random.Range(0, rooms.topTrapRooms.Count)], this.transform);
                } else {
                    go = Instantiate(rooms.topRooms[UnityEngine.Random.Range(0, rooms.topRooms.Count)], this.transform);
                }
                break;
            case RoomSpawner.RoomType.Left:
                if (multiLevel && rooms.levelHasStairCase.Count < maxLevel && !rooms.levelHasStairCase[(int)this.transform.position.y / 10] && rand <= stairCaseAmount) {


                    go = Instantiate(rooms.rightSCRooms[UnityEngine.Random.Range(0, rooms.rightSCRooms.Count)], this.transform);
                    rooms.levelHasStairCase[(int)this.transform.position.y / 10] = true;
                    rooms.levelHasStairCase.Add(new bool());

                } else if (rand < trapAmmount) {
                    go = Instantiate(rooms.rightTrapRooms[UnityEngine.Random.Range(0, rooms.rightTrapRooms.Count)], this.transform);
                } else {
                    go = Instantiate(rooms.rightRooms[UnityEngine.Random.Range(0, rooms.rightRooms.Count)], this.transform);
                }
                break;
            case RoomSpawner.RoomType.Right:
                if (multiLevel && rooms.levelHasStairCase.Count < maxLevel && !rooms.levelHasStairCase[(int)this.transform.position.y / 10] && rand <= stairCaseAmount) {
                   

                    go = Instantiate(rooms.leftSCRooms[UnityEngine.Random.Range(0, rooms.leftSCRooms.Count)], this.transform);
                    rooms.levelHasStairCase[(int)this.transform.position.y / 10] = true;
                    rooms.levelHasStairCase.Add(new bool());
                } else if (rand < trapAmmount) {
                    go = Instantiate(rooms.leftTrapRooms[UnityEngine.Random.Range(0, rooms.leftTrapRooms.Count)], this.transform);
                } else {
                    go = Instantiate(rooms.leftRooms[UnityEngine.Random.Range(0, rooms.leftRooms.Count)], this.transform);
                }
                break;
            case RoomSpawner.RoomType.newLevel: {
                    go = Instantiate(rooms.stairCaseToppers[UnityEngine.Random.Range(0, rooms.stairCaseToppers.Count)], this.transform);

                }


                break;
            default:
                break;
        }
        rooms.roomsList.Add(go);
        go.transform.parent = transform.parent;
        DestroyImmediate(this);
    }
}

