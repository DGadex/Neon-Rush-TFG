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
        }
    }
    public void OnCheckpointPassed(Checkpoint checkpoint)
    {
        if (checkpoint == finishLine && !finishLineUnlocked)
        {
            return;
        }

        if (checkpoint == finishLine && finishLineUnlocked)
        {
            OnLapCompleted?.Invoke();
            ResetCheckpoints();
            return;
        }

        if (!checkpointsPassed.Contains(checkpoint) && checkpoint != finishLine)
        {
            checkpointsPassed.Add(checkpoint);

            int totalNonFinishCheckpoints = checkpoints.Count - 1;
            if (checkpointsPassed.Count == totalNonFinishCheckpoints)
            {
                finishLineUnlocked = true;

                OnFinishLineUnlocked?.Invoke();
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
        cp.checkpointSystem = this;
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

    public void UnregisterCheckpoint(Checkpoint cp)
    {
        if (checkpoints.Contains(cp))
        {
            checkpoints.Remove(cp);
            checkpointsPassed.Remove(cp);
            if (cp == finishLine)
            {
                finishLine = null;
                finishLineUnlocked = false;
            }
        }
    }
}