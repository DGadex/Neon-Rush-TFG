using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;

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

    void Start()
    {
        if (freeLookCamera != null) freeLookCamera.SetActive(true);
        if (followCamera != null) followCamera.gameObject.SetActive(false);
        if (testWarningPanel != null) testWarningPanel.SetActive(false);
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
        if (carController != null) carController.canMove = false;
        
        checkpointSystem.InitializeCheckpoint();
        checkpointSystem.OnLapCompleted += HandleLapCompletion;
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

        if (carController != null) carController.canMove = false;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("🔄 Respawn o cancelación detectada. Prueba cancelada.");
            CancelTest();
        }
    }
}
