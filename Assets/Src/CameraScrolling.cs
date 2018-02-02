// Moves the camera around when the player's cursor is at the edge of the screen
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScrolling : MonoBehaviour {
    public float speed = 13;
    public float sensitiveArea = 0.2f;
    public float xMin = -100;
    public float xMax =  100;
    public float zMin = -100;
    public float zMax =  100;

    void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
    }

    // position the camera so that it is centered on a target
    public void FocusOn(Vector3 pos) {
        // decrease forward*distance from pos. good enough for now.
        float height = transform.position.y;
        transform.position = pos - (transform.rotation * Vector3.forward * height);

        // the previous calculation is not 100% exact, which often causes us to
        // zoom in a bit too far. make sure to keep initial height.
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
