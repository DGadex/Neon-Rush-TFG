using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

public class PostProcessManager : MonoBehaviour
{
    [Header("Referencias")]
    public Volume volume;
    public CinemachineCamera cinemachineCam;
    public ArcadeCarController car;

    [Header("FOV")]
    public float baseFOV = 55f;
    public float maxFOV = 70f;
    public float fovSpeedMultiplier = 0.2f;

    [Header("Lens Distortion")]
    public float maxDistortion = -0.3f;
    public float distortionSpeedMultiplier = 0.005f;

    [Header("Chromatic Aberration")]
    public float chromaticDuringNitro = 0.8f;

    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;

    private float targetFOV;
    private bool wasNitroActive = false;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out chromaticAberration);

        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
    }

    void Update()
    {
        if (car == null || cinemachineCam == null)
        {
            return;
        }
        else
        {
            float speed = car.GetComponent<Rigidbody>().linearVelocity.magnitude;

            targetFOV = baseFOV + Mathf.Clamp(speed * fovSpeedMultiplier, 0f, maxFOV - baseFOV);
            cinemachineCam.GetComponent<CinemachineCamera>().Lens.FieldOfView = Mathf.Lerp(
                cinemachineCam.GetComponent<CinemachineCamera>().Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * 5f
            );

            if (lensDistortion != null)
            {
                float distortion = Mathf.Clamp(speed * -distortionSpeedMultiplier, maxDistortion, 0f);
                lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, distortion, Time.deltaTime * 3f);
            }

            bool nitroNow = car.IsNitroActive();

            if (chromaticAberration != null)
            {
                float targetChromatic = nitroNow ? chromaticDuringNitro : 0f;
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, targetChromatic, Time.deltaTime * 5f);
            }

            if (nitroNow && !wasNitroActive && impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }

            wasNitroActive = nitroNow;
        }
    }

    public void Setup(GameObject carInstance)
    {
        car = carInstance.GetComponent<ArcadeCarController>();
        cinemachineCam = FindObjectOfType<CinemachineCamera>();
        impulseSource = cinemachineCam.GetComponent<CinemachineImpulseSource>();
        if (car == null)
        {
            return;
        }

        cinemachineCam.GetComponent<CinemachineCamera>().Lens.FieldOfView = baseFOV;

        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;

    }
}
