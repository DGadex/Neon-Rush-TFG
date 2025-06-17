using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    // Lista de checkpoints (puertas) en el circuito
    public List<Checkpoint> checkpoints;

    // Tiempo por sector (último sector completado)
    private float[] sectorTimes;

    // Tiempo total de la vuelta
    private float totalTime;

    // Checkpoint actual (índice en la lista)
    private int currentCheckpointIndex = 0;

    // Referencia al coche
    public Transform car;
    // Evento para notificar vueltas completadas
    public delegate void LapCompletedAction();
    public event LapCompletedAction OnLapCompleted;

    public int quantity = 0;

    void Start() {
        // Inicializar el array de tiempos por sector
        sectorTimes = new float[checkpoints.Count];

        // Iniciar el temporizador total
        totalTime = 0f;
    }

    void Update() {
        // Actualizar el tiempo total
        totalTime += Time.deltaTime;
    }

    // Método para registrar el paso por un checkpoint
    public void OnCheckpointPassed(Checkpoint checkpoint) {
        // Verificar si el checkpoint es el siguiente en la secuencia
        if (checkpoint == checkpoints[currentCheckpointIndex]) {
            // Registrar el tiempo del sector
            sectorTimes[currentCheckpointIndex] = totalTime;

            // Mostrar el tiempo del sector en la consola
            Debug.Log($"Sector {currentCheckpointIndex + 1} completado en {sectorTimes[currentCheckpointIndex]:F2} segundos.");

            // Actualizar el checkpoint actual
            currentCheckpointIndex++;

            // Si se completó la última puerta, reiniciar el contador
            if (currentCheckpointIndex >= checkpoints.Count) {
                Debug.Log("¡Vuelta completada!");
                Debug.Log($"Tiempo total: {totalTime:F2} segundos.");
                ResetCheckpoints();
            }
        }
    }

    // Método para reiniciar los checkpoints
    private void ResetCheckpoints()
    {
        currentCheckpointIndex = 0;
        totalTime = 0f;

        // Notificar que se completó una vuelta
        if (OnLapCompleted != null)
        {
            OnLapCompleted();
        }
    }

    

    // Método para obtener la posición de respawn
    public Vector3 GetRespawnPosition() {
        if (currentCheckpointIndex > 0) {
            // Respawn en el último checkpoint pasado
            return checkpoints[currentCheckpointIndex - 1].transform.position;
        } else {
            // Respawn en la posición inicial
            return checkpoints[0].transform.position;
        }
    }
}