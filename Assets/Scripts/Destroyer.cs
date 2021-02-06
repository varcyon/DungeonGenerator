using UnityEngine;

[ExecuteInEditMode]
public class Destroyer : MonoBehaviour
{
    //Destroying any room spawners when that land on the star room
    void Update()
    {   
        Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        foreach (Collider item in colliders)
        {
            if(item.CompareTag("RoomSpawner"))
            {
              // DestroyImmediate(item.gameObject);
            }
        }
    }


}
