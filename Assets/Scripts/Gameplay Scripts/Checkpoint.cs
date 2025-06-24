using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Referencia al sistema de checkpoints
    public CheckpointSystem checkpointSystem;
    public bool isFinishLine = false;
    [Header("Sonido")]
    public AudioSource checkpointSound;
    public AudioSource finishLineSound;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkpointSystem.OnCheckpointPassed(this);

            if (isFinishLine)
            {
                if (finishLineSound != null) finishLineSound.Play();
            }
            else
            {
                if (checkpointSound != null) checkpointSound.Play();
            }
        }
    }
}
