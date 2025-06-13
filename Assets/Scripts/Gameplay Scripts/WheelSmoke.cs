using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(WheelCollider))]
public class WheelSmoke : MonoBehaviour 
{
    [Header("Configuración")]
    public VisualEffect smokeVFX;
    public float minSlipForSmoke = 0.5f;
    public float maxIntensity = 20.0f; // Aumentado para coincidir con MAX_SKID_INTENSITY
    public float spawnRateMultiplier = 10f;
    public float wheelSlipMultiplier = 10f; // Similar a WHEEL_SLIP_MULTIPLIER

    [Header("Referencias")]
    [SerializeField] private Rigidbody vehicleRb;
    private WheelCollider wheelCollider;
    private WheelHit wheelHit;
    private float currentIntensity;
    private bool isSmokeActive = false;
    private float lastFixedUpdateTime;

    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        StopSmoke();
    }

    void FixedUpdate()
    {
        lastFixedUpdateTime = Time.time;
    }

    void LateUpdate()
    {
        if (wheelCollider.GetGroundHit(out wheelHit))
        {
            // 1. Deslizamiento lateral (igual que en skidmarks)
            Vector3 localVelocity = transform.InverseTransformDirection(vehicleRb.linearVelocity);
            float lateralSlip = Mathf.Abs(localVelocity.x);

            // 2. Deslizamiento por rotación (wheelspin) - Método probado de skidmarks
            float wheelAngularVelocity = wheelCollider.radius * ((2 * Mathf.PI * wheelCollider.rpm) / 60);
            float carForwardVel = Vector3.Dot(vehicleRb.linearVelocity, transform.forward);
            float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * wheelSlipMultiplier;

            // Ajuste para evitar humo al acelerar normalmente (como en skidmarks)
            wheelSpin = Mathf.Max(0, wheelSpin * (10 - Mathf.Abs(carForwardVel)));

            // Combinamos ambos
            currentIntensity = (lateralSlip + wheelSpin) / maxIntensity;

            if (currentIntensity > minSlipForSmoke)
            {
                Vector3 smokePoint = wheelHit.point + Vector3.up * 0.05f;
                UpdateSmokeVFX(smokePoint);
            }
            else
            {
                StopSmoke();
            }
        }
        else
        {
            StopSmoke();
        }
    }

    void UpdateSmokeVFX(Vector3 position)
    {
        smokeVFX.transform.position = position + Vector3.up * 0.1f;
        smokeVFX.SetFloat("SpawnRate", currentIntensity * spawnRateMultiplier);
        
        if (!isSmokeActive)
        {
            smokeVFX.Play();
            isSmokeActive = true;
        }
    }

    void StopSmoke()
    {
        smokeVFX.SetFloat("SpawnRate", 0f);
        isSmokeActive = false;
    }
}