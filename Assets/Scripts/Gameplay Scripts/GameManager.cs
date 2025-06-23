using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject car;
    public Transform startPos;
    public CinemachineCamera Camera;
    public CheckpointSystem checkpointSystem;
    public UIManager uiManager;
    public InputActionAsset inputActions;
    
    [Header("Configuración")]
    public int totalLaps = 3;
    
    // Variables privadas
    private int currentLap = 1;
    private bool raceFinished = false;
    private float raceTime = 0f;
    private ArcadeCarController carController;
    private Rigidbody carRigidbody;


    public WheelSkid[] wheelSkids;
    private InputAction respawnAction;

    void Start()
    {
        //InitializeComponents();
        //StartCoroutine(DelayedResetPosition()); // Reset con delay
        checkpointSystem.OnLapCompleted += HandleLapCompletion;
        var map = inputActions.FindActionMap("Driving");
        respawnAction = map.FindAction("Respawn");
        respawnAction.Enable();
    }

    void InitializeComponents()
    {
        carController = car.GetComponent<ArcadeCarController>();
        carRigidbody = car.GetComponent<Rigidbody>();
        checkpointSystem.OnLapCompleted += HandleLapCompletion;
        
        // Configuración inicial del Rigidbody
        carRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void SetupCar(GameObject carInstance, Transform startPosInstance)
    {
        this.car = carInstance;
        this.startPos = startPosInstance;

         //Teletransporte inmediato
        /*car.transform.position = startPos.position;
        car.transform.rotation = startPos.rotation;*/

        carController = car.GetComponent<ArcadeCarController>();
        carRigidbody = car.GetComponent<Rigidbody>();
        wheelSkids = car.GetComponentsInChildren<WheelSkid>();

        // Cinemachine
        //Camera.Follow = car.transform;
        //Camera.LookAt = car.transform;

        // Checkpoint system
        checkpointSystem.car = car.transform;

        // Rigidbody config
        carRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        //carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        car.GetComponentInChildren<SpawnDissolveController>()?.PlayDissolve();

        StartCoroutine(DelayedResetPosition()); // Reset con delay
    }

    IEnumerator DelayedResetPosition()
    {
        yield return new WaitForFixedUpdate(); // Espera al próximo FixedUpdate
        
        yield return new WaitForSeconds(1f); // Pequeño delay para asegurar estabilidad
        Debug.LogWarning("DALE POSICION");
        //carRigidbody.isKinematic = true;
        //carController.transform.SetLocalPositionAndRotation(startPos.localPosition, startPos.localRotation);
        carRigidbody.isKinematic = false;
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        
    }

    void Update()
    {
        if (!raceFinished)
        {
            raceTime += Time.deltaTime;
            uiManager.UpdateRaceUI(currentLap, totalLaps, raceTime);
            if (respawnAction.IsPressed()) ForceRespawn();
        }
    }

    private void HandleLapCompletion()
    {
        currentLap++;
        
        if (currentLap > totalLaps)
        {
            FinishRace();
        }
        else
        {
            Debug.Log($"Vuelta {currentLap}/ {totalLaps}");
            // Efecto visual/sonoro opcional al completar vuelta
        }
    }

    private void FinishRace()
    {
        raceFinished = true;
        uiManager.ShowFinishPanel(raceTime);
        
        // Desactivar controles suavemente
        carController.enabled = false;
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
    }

    // Para reinicio manual (por ejemplo, al caer al vacío)
    public void ForceRespawn()
{
    if (car == null || carRigidbody == null || checkpointSystem == null) return;
    car.GetComponentInChildren<SpawnDissolveController>()?.PlayDissolve();

    Vector3 respawnPos = checkpointSystem.GetRespawnPosition();
    Quaternion respawnRot = Quaternion.Euler(0, 0, 0);

    carRigidbody.linearVelocity = Vector3.zero;
    carRigidbody.angularVelocity = Vector3.zero;

    carRigidbody.MovePosition(respawnPos);
    carRigidbody.MoveRotation(respawnRot);


    // Opcional: Desactivar el control del coche hasta que el VFX termine
    StartCoroutine(EnableCarAfterDelay(1.5f)); // Ajusta el tiempo según la duración del VFX
}

    IEnumerator EnableCarAfterDelay(float delay)
    {
        carController.enabled = false;
        yield return new WaitForSeconds(delay);
        carController.enabled = true;
    }
}