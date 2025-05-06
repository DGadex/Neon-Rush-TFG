using UnityEngine;

public class MovementScript : MonoBehaviour {

    public float speed = 5f;
    public float rotationSpeed = 200f;

    private Transform myTransform;

    void Start() {
        myTransform = transform;
    }

    void Update() {

        // Rotate the object based on mouse movement
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        myTransform.Rotate(Vector3.up, mouseX, Space.World);
        myTransform.Rotate(Vector3.left, mouseY, Space.Self);

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) {
            movement += myTransform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            movement -= myTransform.forward;
        }
        if (Input.GetKey(KeyCode.A)) {
            movement -= myTransform.right;
        }
        if (Input.GetKey(KeyCode.D)) {
            movement += myTransform.right;
        }

        movement.Normalize();
        myTransform.Translate(movement * speed * Time.deltaTime);
    }
}