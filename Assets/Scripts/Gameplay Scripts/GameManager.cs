using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Referencias
    public GameObject car; // Objeto del coche (con Rigidbody y ArcadeCarController)
    public Transform startPos; // Empty "StartPos" para respawn inicial
    public CheckpointSystem checkpointSystem; // Sistema de checkpoints
    public int totalLaps = 3; // Vueltas totales del circuito

    // Variables internas
    private int currentLap = 1;
    private bool raceFinished = false;
    private ArcadeCarController carController; // Script de control del coche
    private Rigidbody carRigidbody;

    void Start()
    {
        // Inicializar componentes del coche
        carController = car.GetComponent<ArcadeCarController>();
        carRigidbody = car.GetComponent<Rigidbody>();

        // Posicionar el coche al inicio
        ResetCarPosition();

        // Configurar el checkpoint system
        checkpointSystem.OnLapCompleted += HandleLapCompletion;
    }

    // Reinicia la posición del coche al StartPos
    public void ResetCarPosition()
    {
        car.transform.position = startPos.position;
        car.transform.rotation = startPos.rotation;
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
    }

    // Lógica cuando se completa una vuelta
    private void HandleLapCompletion()
    {
        currentLap++;

        if (currentLap > totalLaps)
        {
            FinishRace();
        }
        else
        {
            Debug.Log($"¡Vuelta {currentLap - 1} completada! Vueltas restantes: {totalLaps - currentLap + 1}");
        }
    }

    // Finalizar la carrera
    private void FinishRace()
    {
        raceFinished = true;
        Debug.Log("¡Carrera terminada!");

        // Deshabilitar controles
        carController.enabled = false; // Desactiva el script de control

        // Opcional: Congelar el coche
        carRigidbody.linearVelocity = Vector3.zero; // Congela la velocidad
        carRigidbody.angularVelocity = Vector3.zero; // Congela la rotación
        //carRigidbody.isKinematic = true;
    }
}