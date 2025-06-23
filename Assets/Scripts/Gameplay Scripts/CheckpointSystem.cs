using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Tooltip("Lista completa de checkpoints, incluyendo el de meta.")]
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
    private HashSet<Checkpoint> checkpointsPassed = new HashSet<Checkpoint>();
    private float totalTime;

    public Transform car;

    public delegate void LapCompletedAction();
    public event LapCompletedAction OnLapCompleted;

    public delegate void FinishLineUnlockedAction();
    public event FinishLineUnlockedAction OnFinishLineUnlocked;

    private Checkpoint finishLine;
    private bool finishLineUnlocked = false;

    void Start()
    {
        /*totalTime = 0f;

        // Buscar el checkpoint de meta
        foreach (Checkpoint cp in checkpoints)
        {
            if (cp.isFinishLine)
            {
                finishLine = cp;
                break;
            }
        }

        if (finishLine == null)
        {
            Debug.LogError("No se ha encontrado un checkpoint de meta en la lista.");
        }*/
    }

    void Update()
    {
        totalTime += Time.deltaTime;
    }


    public void InitializeCheckpoint()
    {
        totalTime = 0f;

        // Buscar el checkpoint de meta
        foreach (Checkpoint cp in checkpoints)
        {
            if (cp.isFinishLine)
            {
                finishLine = cp;
                Debug.Log("LLAMADA META");
                break;
            }
        }

        if (finishLine == null)
        {
            Debug.LogError("No se ha encontrado un checkpoint de meta en la lista.");
        }
    }
    public void OnCheckpointPassed(Checkpoint checkpoint)
    {
        // Si es el checkpoint de meta pero no está desbloqueado aún, ignorar
        if (checkpoint == finishLine && !finishLineUnlocked)
        {
            Debug.Log("El checkpoint de meta aún no está desbloqueado.");
            return;
        }

        // Si es la meta y ya está desbloqueado, completar la vuelta
        if (checkpoint == finishLine && finishLineUnlocked)
        {
            Debug.Log("¡Vuelta completada!");
            Debug.Log($"Tiempo total: {totalTime:F2} segundos.");

            // Notificar antes de resetear
            OnLapCompleted?.Invoke();
            ResetCheckpoints();
            return;
        }

        // Si no es la meta y no se ha pasado aún
        if (!checkpointsPassed.Contains(checkpoint) && checkpoint != finishLine)
        {
            checkpointsPassed.Add(checkpoint);
            Debug.Log($"Checkpoint {checkpoint.name} registrado.");

            // Si ahora solo queda la meta sin pasar, desbloquearla
            int totalNonFinishCheckpoints = checkpoints.Count - 1; // descontamos el de meta
            if (checkpointsPassed.Count == totalNonFinishCheckpoints)
            {
                finishLineUnlocked = true;
                Debug.Log("¡Checkpoint de meta desbloqueado!");

                OnFinishLineUnlocked?.Invoke(); // Para efectos visuales
            }
        }
    }

    private void ResetCheckpoints()
    {
        checkpointsPassed.Clear();
        totalTime = 0f;
        finishLineUnlocked = false;
    }

    public void RegisterCheckpoint(Checkpoint cp)
    {
        checkpoints.Add(cp);
        cp.checkpointSystem = this; // Asignar el sistema de checkpoints al checkpoint
    }
    public Vector3 GetRespawnPosition()
    {
        if (checkpointsPassed.Count > 0)
        {
            Checkpoint lastCheckpoint = null;
            foreach (var cp in checkpoints)
            {
                if (checkpointsPassed.Contains(cp))
                    lastCheckpoint = cp;
            }
            return lastCheckpoint.transform.position;
        }
        else
        {
            return checkpoints[0].transform.position;
        }
    }
}