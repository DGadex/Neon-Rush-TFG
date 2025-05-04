using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Referencia al sistema de checkpoints
    public CheckpointSystem checkpointSystem;

    void OnTriggerEnter(Collider other) {
        // Verificar si el objeto que pasó es el coche
        if (other.CompareTag("Player")) {
            // Notificar al sistema de checkpoints que se pasó este checkpoint
            checkpointSystem.OnCheckpointPassed(this);
        }
    }
}
