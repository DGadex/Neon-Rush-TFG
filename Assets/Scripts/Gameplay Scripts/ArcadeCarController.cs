using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;


[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    #region Configuración
    [Header("Referencias")]
    public Transform chassis;
    public WheelCollider[] allWheels;
    public Transform[] wheelVisuals;
    public bool[] isSteeringWheel;
    public bool[] isDriveWheel;
    public InputActionAsset inputActions;
    #endregion

    #region Aceleración
    [Header(" Aceleración")]
    public float maxMotorTorque = 12000f;
    public float brakeTorque = 8000f;
    public float maxSpeed = 180f;

    [HideInInspector]
    public bool canMove = true;
    private bool isReversing = false;
    public AnimationCurve torqueCurve = new AnimationCurve(
        new Keyframe(0, 1f),
        new Keyframe(0.5f, 0.8f),
        new Keyframe(1, 0.3f)
    );
    #endregion

    #region Dirección
    [Header(" Dirección")]
    public float maxSteeringAngle = 15f;
    public float steeringResponse = 10f;
    public float downforce = 50f;
    private float currentSteerAngle;
    #endregion

    #region Derrape
    [Header(" Derrape")]
    public float driftFriction = 0.3f;
    public float normalFriction = 2f;
    public float driftTiltAngle = 10f;
    public float driftTiltSpeed = 15f;
    public float driftAutoCorrect = 20f;
    public float minDriftSpeed = 15f;
    private bool isDrifting;
    #endregion

    #region Nitro
    [Header("Nitro")]
    public float nitroBoost = 2f;
    public float nitroDuration = 2f;
    public AudioSource nitroSound;
    private bool isNitroActive = false;
    #endregion

    #region Salto
    [Header("Salto")]
    public float jumpForce = 20f;
    public LayerMask rampLayer;
    #endregion

    #region Sonidos
    [Header("Sonidos")]
    public AudioSource driftSound;
    public AudioSource engineSound;
    #endregion

    #region Efectos Visuales
    [Header("VFX")]
    public VisualEffect nitroFlamesVFX;
    public VisualEffect[] brakeTrailsVFX;
    #endregion

    #region Privadas
    private Rigidbody rb;
    private InputAction throttleAction, brakeAction, steerAction, driftAction, nitroAction;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        var map = inputActions.FindActionMap("Driving");
        throttleAction = map.FindAction("Throttle");
        brakeAction = map.FindAction("Brake");
        steerAction = map.FindAction("Steering");
        driftAction = map.FindAction("Drift");
        nitroAction = map.FindAction("Nitro");
        throttleAction.Enable();
        brakeAction.Enable();
        steerAction.Enable();
        driftAction.Enable();
        nitroAction.Enable();

        if (nitroFlamesVFX != null) nitroFlamesVFX.Stop();
        if (brakeTrailsVFX != null)
        {
            foreach (var trail in brakeTrailsVFX)
            {
                trail.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        float throttle = throttleAction.ReadValue<float>();
        float brake = brakeAction.ReadValue<float>();
        float steer = steerAction.ReadValue<Vector2>().x;
        bool drift = driftAction.ReadValue<float>() > 0.5f;
        bool nitro = nitroAction.IsPressed();

        ApplyDownforce();
        ApplyMotor(throttle, brake);
        ApplySteering(steer);
        HandleDrift(drift, steer);
        LimitSpeed();
        HandleNitroInput();
        UpdateEngineSound();
    }

    void Update()
    {
        UpdateWheelVisuals();
    }

    void ApplyDownforce()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        rb.AddForce(-transform.up * downforce * speedFactor * rb.mass, ForceMode.Force);
    }

    void ApplyMotor(float throttle, float brake)
    {
        float speed = rb.linearVelocity.magnitude;
        float currentTorque = maxMotorTorque * torqueCurve.Evaluate(speed / maxSpeed);

        // Determinar si queremos entrar o salir del modo marcha atrás
        if (brake > 0.1f && speed < 1f && throttle < 0.1f)
        {
            isReversing = true;
        }
        else if (throttle > 0.1f)
        {
            isReversing = false;
        }

        for (int i = 0; i < allWheels.Length; i++)
        {
            if (!isDriveWheel[i]) continue;

            if (isReversing)
            {
                // Modo marcha atrás
                allWheels[i].motorTorque = -brake * currentTorque * 1.5f;
                allWheels[i].brakeTorque = 0f;
            }
            else if (throttle > 0.1f)
            {
                // Aceleración normal
                allWheels[i].motorTorque = throttle * currentTorque * (isNitroActive ? nitroBoost : 1f);
                allWheels[i].brakeTorque = 0f;
            }
            else if (brake > 0.1f)
            {
                // Freno normal (sin retroceso)
                allWheels[i].motorTorque = 0f;
                allWheels[i].brakeTorque = brake * brakeTorque;
            }
            else
            {
                // Freno automático por inercia
                allWheels[i].motorTorque = 0f;
                allWheels[i].brakeTorque = speed > 5f ? 2000f : 0f;
            }
        }
    }

    void ApplySteering(float steerInput)
    {
        float effectiveMaxSpeed = maxSpeed * (isNitroActive ? nitroBoost : 1f);
        float speedFactor = Mathf.Clamp(rb.linearVelocity.magnitude / (effectiveMaxSpeed * 0.3f), 0.3f, 1f);
        currentSteerAngle = Mathf.Lerp(
            currentSteerAngle,
            steerInput * maxSteeringAngle * speedFactor,
            steeringResponse * Time.fixedDeltaTime
        );

        for (int i = 0; i < allWheels.Length; i++)
        {
            if (!isSteeringWheel[i]) continue;
            allWheels[i].steerAngle = currentSteerAngle;
        }
    }

    void UpdateWheelVisuals()
    {
        for (int i = 0; i < allWheels.Length; i++)
        {
            allWheels[i].GetWorldPose(out Vector3 position, out Quaternion rotation);
            wheelVisuals[i].position = position;
            if(i == 1 || i == 3) // Rear wheels
            {
                rotation *= Quaternion.Euler(0, 180, 0); // Adjust rotation for rear wheels
            }
            wheelVisuals[i].rotation = rotation;
            
            if (isSteeringWheel[i])
            {
                float steerAngle = allWheels[i].steerAngle;
                wheelVisuals[i].localRotation = Quaternion.Euler(0, steerAngle, 0);
                wheelVisuals[i].rotation = rotation;
            }
        }
    }

    void HandleDrift(bool driftInput, float steerInput)
    {
        bool canDrift = rb.linearVelocity.magnitude > minDriftSpeed && Mathf.Abs(steerInput) > 0.3f;

        if (driftInput && canDrift)
        {
            isDrifting = true;
            SetWheelFriction(driftFriction);

            float targetTilt = -steerInput * driftTiltAngle;
            chassis.localRotation = Quaternion.Slerp(
                chassis.localRotation,
                Quaternion.Euler(0, 0, targetTilt),
                driftTiltSpeed * Time.fixedDeltaTime
            );

            if (!driftSound.isPlaying) driftSound.Play();
        }
        else if (isDrifting)
        {
            isDrifting = false;
            SetWheelFriction(normalFriction);

            chassis.localRotation = Quaternion.Slerp(
                chassis.localRotation,
                Quaternion.identity,
                driftTiltSpeed * 2f * Time.fixedDeltaTime
            );

            if (driftSound.isPlaying) driftSound.Stop();
        }
    }

    void HandleNitroInput()
    {
        if (nitroAction.IsPressed() && !isNitroActive)
        {
            StartCoroutine(ActivateNitro());
        }
    }

    public bool IsNitroActive()
    {
        return isNitroActive;
    }

    void LimitSpeed()
    {
        float max = maxSpeed * (isNitroActive ? nitroBoost : 1f);
        if (rb.linearVelocity.magnitude > max)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * max;
        }
    }

    void SetWheelFriction(float stiffness)
    {
        for (int i = 0; i < allWheels.Length; i++)
        {
            WheelFrictionCurve friction = allWheels[i].sidewaysFriction;
            friction.stiffness = isSteeringWheel[i] ? stiffness + 0.5f : stiffness;
            allWheels[i].sidewaysFriction = friction;
        }
    }

    void UpdateEngineSound()
    {
        if (!engineSound) return;
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        engineSound.pitch = 0.5f + speedFactor * 1.5f;
        engineSound.volume = 0.3f + speedFactor * 0.7f;
    }

    IEnumerator ActivateNitro()
    {
        isNitroActive = true;
        if (nitroFlamesVFX != null) nitroFlamesVFX.Play();
        if (brakeTrailsVFX != null)
        {
            foreach (var trail in brakeTrailsVFX) trail.Play();
        }
        if (nitroSound != null) nitroSound.Play();

        float originalMaxSpeed = maxSpeed;
        maxSpeed *= nitroBoost;

        yield return new WaitForSeconds(nitroDuration);

        if (nitroFlamesVFX != null) nitroFlamesVFX.Stop();
        if (brakeTrailsVFX != null)
        {
            foreach (var trail in brakeTrailsVFX) trail.Stop();
        }
        maxSpeed = originalMaxSpeed;
        isNitroActive = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & rampLayer) != 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
