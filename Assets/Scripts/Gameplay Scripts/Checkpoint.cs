using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Referencia al sistema de checkpoints
    public CheckpointSystem checkpointSystem;
    public bool isFinishLine = false;


    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            checkpointSystem.OnCheckpointPassed(this);
        }
    }
}
