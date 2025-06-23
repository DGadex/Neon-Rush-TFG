using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.InputSystem;

public class TestSpawner : MonoBehaviour
{
    [Header("Cámaras")]
    public GameObject freeLookCamera;
    public CinemachineCamera followCamera;

    [Header("Guardar y Prueba")]
    public GuardarJSON guardarJSON;
    public GameObject testWarningPanel;
    public ObjectPlacer objectPlacer;

    public GameObject currentCar;
    private bool testPassed = false;
    private bool testActive = false;

    public ArcadeCarController carController;
    public CheckpointSystem checkpointSystem;
    public FreeCameraController freeCamera;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private InputAction respawnAction;

    void Start()
    {
        if (freeLookCamera != null) freeLookCamera.SetActive(true);
        if (followCamera != null) followCamera.gameObject.SetActive(false);
        if (testWarningPanel != null) testWarningPanel.SetActive(false);

        var map = new InputActionMap("Driving");
        respawnAction = new InputAction("Respawn", binding: "<Keyboard>/r");
        respawnAction.Enable();
    }

    public void AssignCarFromMeta(GameObject car, GameObject fc)
    {
        followCamera = fc.GetComponent<CinemachineCamera>();
        currentCar = car;
        carController = car.GetComponent<ArcadeCarController>();

        checkpointSystem = FindObjectOfType<CheckpointSystem>();
        freeCamera = FindObjectOfType<FreeCameraController>();

        if (carController != null)
            carController.canMove = false;
    }

    public void StartTest()
    {
        if (currentCar == null)
        {
            Debug.LogError("No hay coche asignado para el test.");
            return;
        }

        if (followCamera != null) followCamera.gameObject.SetActive(true);
        if (freeLookCamera != null) freeLookCamera.SetActive(false);

        testPassed = false;
        testActive = true;
        objectPlacer.escenarioProbado = false;
        if (carController != null)
        {
            carController.canMove = false;
            carController.enabled = true;}

        checkpointSystem.InitializeCheckpoint();
        checkpointSystem.OnLapCompleted += HandleLapCompletion;

        // Guardamos la posición inicial del test
        initialPosition = currentCar.transform.position;
        initialRotation = currentCar.transform.rotation;

        currentCar.GetComponentInChildren<SpawnDissolveController>()?.PlayDissolve();
        Invoke(nameof(EnableCarControl), 1f);

        if (freeCamera != null) freeCamera.SetProbandoTrue();
    }

    void EnableCarControl()
    {
        if (carController != null)
            carController.canMove = true;
    }

    void HandleLapCompletion()
    {
        testPassed = true;
        objectPlacer.escenarioProbado = true;
        CancelTest();
        if (testWarningPanel != null) testWarningPanel.SetActive(false);
        Debug.Log("✅ Circuito superado correctamente.");
    }

    public void CancelTest()
    {
        if (followCamera != null) followCamera.gameObject.SetActive(false);
        if (freeLookCamera != null) freeLookCamera.SetActive(true);

        if (carController != null)
        {
            carController.canMove = false;
            carController.enabled = false;
        }

        if (!testPassed && testWarningPanel != null)
        {
            testWarningPanel.SetActive(true);
        }

        testActive = false;
        if (freeCamera != null) freeCamera.SetProbandoFalse();

    }

    public bool CanSaveLevel()
    {
        return testPassed;
    }

    void Update()
    {
        if (!testActive) return;

        if (respawnAction.WasPerformedThisFrame())
        {
            Debug.Log("🔄 Respawn detectado. Reiniciando posición del coche.");
            RespawnCar();
        }
    }

    void RespawnCar()
    {
        if (currentCar == null) return;

        Rigidbody rb = currentCar.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(initialPosition);
            rb.MoveRotation(initialRotation);
        }

        currentCar.GetComponentInChildren<SpawnDissolveController>()?.PlayDissolve();

        if (carController != null)
        {
            carController.canMove = false;
            Invoke(nameof(EnableCarControl), 1.5f); // Delay según VFX
        }
        CancelTest();
    }
}
