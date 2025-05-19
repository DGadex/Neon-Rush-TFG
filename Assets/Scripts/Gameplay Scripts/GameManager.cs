using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject car; 
    public Transform startPos;
    public CheckpointSystem checkpointSystem;
    public UIManager uiManager;
    
    [Header("Configuración")]
    public int totalLaps = 3;
    
    // Variables privadas
    private int currentLap = 1;
    private bool raceFinished = false;
    private float raceTime = 0f;
    private ArcadeCarController carController;
    private Rigidbody carRigidbody;

    void Start()
    {
        InitializeComponents();
        StartCoroutine(DelayedResetPosition()); // Reset con delay
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

    IEnumerator DelayedResetPosition()
    {
        yield return new WaitForFixedUpdate(); // Espera al próximo FixedUpdate
        
        carRigidbody.isKinematic = true;
        car.transform.SetPositionAndRotation(startPos.position, startPos.rotation);
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        carRigidbody.isKinematic = false;
    }

    void Update()
    {
        if (!raceFinished)
        {
            raceTime += Time.deltaTime;
            uiManager.UpdateRaceUI(currentLap, totalLaps, raceTime);
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
        currentLap = 1;
        raceFinished = false;
        StartCoroutine(DelayedResetPosition());
    }
}