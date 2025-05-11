using UnityEngine;

public class SkidmarkManager : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public TrailRenderer trailRenderer;
    public float skidThreshold = 0.3f;

    private WheelHit wheelHit;

    void Update()
    {
        if (wheelCollider.GetGroundHit(out wheelHit))
        {
            float slip = Mathf.Abs(wheelHit.sidewaysSlip);
            trailRenderer.emitting = slip > skidThreshold;

            // Posiciona el trail justo donde la rueda toca el suelo
            transform.position = wheelHit.point + wheelHit.normal * 0.02f; // un poco por encima

            // Alinea la rotación con la normal del suelo
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, wheelHit.normal), wheelHit.normal);
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }
}
