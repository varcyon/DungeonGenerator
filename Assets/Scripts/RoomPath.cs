using UnityEngine;

[ExecuteInEditMode]
public class RoomPath : MonoBehaviour {
    private bool wall;
    private bool wall2;
    private bool wall3;
    private bool wall4;
    float colliderHalfExtant = 0.5f;

    

    public void DrawPath() {
        if(this.gameObject.name == "NewLevelStart") {
            for (int i = 0; i < 9; i++) {
            Debug.DrawLine(this.transform.position + new Vector3(i * 0.1f, 0, i * 0.1f), this.transform.position + (Vector3.down * 10) + new Vector3(i * 0.1f, 0, i * 0.1f), Color.blue);
            }
        }
        //z+
        Collider[] colliders = Physics.OverlapBox(this.transform.position + new Vector3(0, 1, 0) + (Vector3.forward * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        foreach (Collider c in colliders) {
            if (c.gameObject.CompareTag("Room") || c.gameObject.CompareTag("DontDestroyMe")) {
                wall = true;
            }
        }
        if (!wall) {
            for (int i = 0; i < 9; i++) {

            Debug.DrawLine(this.transform.position + new Vector3(i*0.1f, 0.6f, i * 0.1f), this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f) + (Vector3.forward * 5f), Color.red);
            }
        }

        //Z-
        Collider[] colliders2 = Physics.OverlapBox(this.transform.position + new Vector3(0, 1, 0) + (Vector3.back * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        foreach (Collider c in colliders2) {
            if (c.gameObject.CompareTag("Room") || c.gameObject.CompareTag("DontDestroyMe")) {
                wall2 = true;
            }
        }
        if (!wall2) {
            for (int i = 0; i < 9; i++) {

                Debug.DrawLine(this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f), this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f) + (Vector3.back * 5f), Color.red);
            }
        }
        //X+
        Collider[] colliders3 = Physics.OverlapBox(this.transform.position + new Vector3(0, 1, 0) + (Vector3.right * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        foreach (Collider c in colliders3) {
            if (c.gameObject.CompareTag("Room") || c.gameObject.CompareTag("DontDestroyMe")) {
                wall3 = true;
            }
        }
        if (!wall3) {
            for (int i = 0; i < 9; i++) {

                Debug.DrawLine(this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f), this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f) + (Vector3.right * 5f), Color.red);
            }
        }
        //X-
        Collider[] colliders4 = Physics.OverlapBox(this.transform.position + new Vector3(0, 1, 0) + (Vector3.left * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        foreach (Collider c in colliders4) {
            if (c.gameObject.CompareTag("Room") || c.gameObject.CompareTag("DontDestroyMe")) {
                wall4 = true;
            }
        }
        if (!wall4) {
            for (int i = 0; i < 9; i++) {

                Debug.DrawLine(this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f), this.transform.position + new Vector3(i * 0.1f, 0.6f, i * 0.1f) + (Vector3.left * 5f), Color.red);
            }
        }

    }
    /*
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(this.transform.position + new Vector3(0, 1, 0) + (Vector3.forward * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        Gizmos.DrawWireCube(this.transform.position + new Vector3(0, 1, 0) + (Vector3.back * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        Gizmos.DrawWireCube(this.transform.position + new Vector3(0, 1, 0) + (Vector3.right * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));
        Gizmos.DrawWireCube(this.transform.position + new Vector3(0, 1, 0) + (Vector3.left * 5f), new Vector3(colliderHalfExtant, colliderHalfExtant, colliderHalfExtant));

    }
    */
}
