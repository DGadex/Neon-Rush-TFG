using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;

public class TestSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject carInScene; // Referencia al coche ya presente en el prefab de la meta
    public Button spawnButton;

    [Header("Cámaras")]
    public GameObject freeLookCamera;
    public CinemachineCamera followCamera;

    [Header("Guardar y Prueba")]
    public GuardarJSON guardarJSON;
    public GameObject testWarningPanel;

    private bool isCarSpawned = false;
    private bool testPassed = false;
    private bool testActive = false;

    private ArcadeCarController carController;

    void Start()
    {
        spawnButton.onClick.AddListener(ToggleCar);

        if (freeLookCamera != null) freeLookCamera.SetActive(true);
        if (followCamera != null) followCamera.gameObject.SetActive(false);

        if (testWarningPanel != null) testWarningPanel.SetActive(false);
    }

    void ToggleCar()
    {
        if (isCarSpawned)
        {
            EndTest();
        }
        else
        {
            StartTest();
        }
    }

    void StartTest()
    {
        if (carInScene == null)
        {
            Debug.LogError("No se ha asignado el coche desde el prefab de la meta.");
            return;
        }

        isCarSpawned = true;
        testActive = true;
        testPassed = false;

        // Activar cámara de seguimiento
        if (followCamera != null)
        {
            followCamera.gameObject.SetActive(true);
            followCamera.Follow = carInScene.transform;
            followCamera.LookAt = carInScene.transform;
        }

        if (freeLookCamera != null) freeLookCamera.SetActive(false);

        carController = carInScene.GetComponent<ArcadeCarController>();
        if (carController != null) carController.canMove = false;

        Invoke(nameof(EnableCarControl), 1f);

        Debug.Log("🚗 Prueba iniciada con coche existente");
    }

    void EnableCarControl()
    {
        if (carController != null)
            carController.canMove = true;
    }

    void EndTest()
    {
        isCarSpawned = false;
        testActive = false;

        if (freeLookCamera != null) freeLookCamera.SetActive(true);
        if (followCamera != null) followCamera.gameObject.SetActive(false);

        if (!testPassed && testWarningPanel != null)
        {
            testWarningPanel.SetActive(true);
        }

        Debug.Log("🚗 Fin de prueba - Vuelta a edición");
    }

    public void MarkTestAsPassed()
    {
        testPassed = true;
        if (testWarningPanel != null) testWarningPanel.SetActive(false);
    }

    public bool CanSaveLevel()
    {
        return testPassed;
    }

    void Update()
    {
        if (!testActive) return;

        if (carInScene == null)
        {
            Debug.LogWarning("❌ El coche fue destruido sin completar la prueba.");
            EndTest();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("🔄 Respawn solicitado durante test. Cancelando prueba.");
            EndTest();
        }
    }

    public void AssignCarFromMeta(GameObject carFromMeta)
    {
        carInScene = carFromMeta;
    }
}
