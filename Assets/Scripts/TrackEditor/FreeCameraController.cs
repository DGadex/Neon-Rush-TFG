using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreeCameraController : MonoBehaviour
{
    public float movementSpeed = 25f;
    public float rotationSpeed = 200f;
    public Transform cameraTransform;
    public GameObject targetObject;
    private float yaw = 0f;
    private float pitch = 0f;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GraphicRaycaster graphicRaycaster;
    [SerializeField]
    private TMP_InputField text;
    [SerializeField]
    private InputManager inputManager;

    private bool escribiendo = false;
    public bool probando = false;

    void Start()
    {

    }

    void Update()
    {
        if (!escribiendo && !probando)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, -89f, 89f); // Prevent flipping

                cameraTransform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movementSpeed = 50f;
            }
            else
            {
                movementSpeed = 25f;
            }

            // WASD movement
            Vector3 move = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            );

            transform.position += cameraTransform.rotation * move * movementSpeed * Time.deltaTime;

            // Ascend/Descend with Q/E
            if (Input.GetKey(KeyCode.Q))
                transform.position += Vector3.down * movementSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.E))
                transform.position += Vector3.up * movementSpeed * Time.deltaTime;
        }
    }

    public void SetBoolFalse()
    {
        escribiendo = false;
    }

    public void SetBoolTrue()
    {
        escribiendo = true;
    }

    public void SetProbandoFalse()
    {
        probando = false;
        canvas.enabled = true;
        graphicRaycaster.enabled = true;
        text.enabled = true;
    }

    public void SetProbandoTrue()
    {
        probando = true;
        canvas.enabled = false;
        graphicRaycaster.enabled = false;
        text.enabled = false;
        inputManager.Deselect();
    }

}
