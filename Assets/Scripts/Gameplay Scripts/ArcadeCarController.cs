using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    #region Configuración Básica
    [Header("Referencias")]
    public Transform chassis;
    public WheelCollider[] driveWheels;
    public WheelCollider[] steeringWheels;
    public InputActionAsset inputActions;
    #endregion

    #region Aceleración Brutal
    [Header("⚡ Aceleración")]
    public float maxMotorTorque = 12000f;
    public float brakeTorque = 8000f;
    public float maxSpeed = 180f;
    public AnimationCurve torqueCurve = new AnimationCurve(
        new Keyframe(0, 1f),
        new Keyframe(0.5f, 0.8f),
        new Keyframe(1, 0.3f)
    );
    #endregion

    #region Dirección Tipo F1
    [Header("🏎️ Dirección")]
    public float maxSteeringAngle = 15f;
    public float steeringResponse = 10f;
    public float downforce = 50f;
    private float currentSteerAngle;
    #endregion

    #region Derrape Épico
    [Header("💥 Derrape")]
    public float driftFriction = 0.2f;
    public float normalFriction = 2f;
    public float driftTiltAngle = 25f;
    public float driftTiltSpeed = 15f;
    public float driftAutoCorrect = 20f;
    public float minDriftSpeed = 15f;
    private bool isDrifting;
    #endregion

    #region Nitro
    [Header("🚀 Nitro")]
    public float nitroBoost = 2f;
    public float nitroDuration = 2f;
    public AudioSource nitroSound;
    private bool isNitroActive;
    #endregion

    #region Salto
    [Header("🌟 Salto")]
    public float jumpForce = 20f;
    public LayerMask rampLayer;
    #endregion

    #region Sonidos
    [Header("🔊 Sonidos")]
    public AudioSource driftSound;
    public AudioSource engineSound;
    #endregion

    #region Privadas
    private Rigidbody rb;
    private InputAction throttleAction, brakeAction, steerAction, driftAction, nitroAction;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.3f, 0); // Centro de masa más bajo

        // Configuración de inputs
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
    }

    void FixedUpdate()
    {
        float throttle = throttleAction.ReadValue<float>();
        float brake = brakeAction.ReadValue<float>();
        float steer = steerAction.ReadValue<Vector2>().x;
        bool drift = driftAction.ReadValue<float>() > 0.5f;
        bool nitro = nitroAction.triggered;

        ApplyDownforce();
        ApplyMotor(throttle, brake);
        ApplySteering(steer);
        HandleDrift(drift, steer);
        LimitSpeed();

        if (nitro && !isNitroActive) StartCoroutine(ActivateNitro());
        
        UpdateEngineSound();
    }

    #region Físicas Mejoradas
    void ApplyDownforce()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        rb.AddForce(-transform.up * downforce * speedFactor * rb.mass, ForceMode.Force);
    }

    void ApplyMotor(float throttle, float brake)
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        float currentTorque = maxMotorTorque * torqueCurve.Evaluate(speedFactor);

        foreach (var wheel in driveWheels)
        {
            if (throttle > 0.1f)
            {
                wheel.motorTorque = throttle * currentTorque * (isNitroActive ? nitroBoost : 1f);
                wheel.brakeTorque = 0f;
            }
            else if (brake > 0.1f)
            {
                wheel.brakeTorque = brake * brakeTorque;
                wheel.motorTorque = 0f;
            }
            else
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = rb.linearVelocity.magnitude > 5f ? 2000f : 0f;
            }
        }
    }

    void ApplySteering(float steerInput)
    {
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / (maxSpeed * 0.3f));
        currentSteerAngle = Mathf.Lerp(
            currentSteerAngle,
            steerInput * maxSteeringAngle * speedFactor,
            steeringResponse * Time.fixedDeltaTime
        );

        foreach (var wheel in steeringWheels)
        {
            wheel.steerAngle = currentSteerAngle;
        }
    }

    void HandleDrift(bool driftInput, float steerInput)
    {
        bool canDrift = rb.linearVelocity.magnitude > minDriftSpeed && Mathf.Abs(steerInput) > 0.3f;

        if (driftInput && canDrift)
        {
            isDrifting = true;
            SetWheelFriction(driftFriction);

            // Inclinación visual rápida
            float targetTilt = -steerInput * driftTiltAngle;
            chassis.localRotation = Quaternion.Slerp(
                chassis.localRotation,
                Quaternion.Euler(0, 0, targetTilt),
                driftTiltSpeed * Time.fixedDeltaTime
            );

            // Derrape con auto-corrección
            rb.AddTorque(
                transform.up * steerInput * driftAutoCorrect * rb.linearVelocity.magnitude * 0.01f,
                ForceMode.VelocityChange
            );

            if (!driftSound.isPlaying) driftSound.Play();
        }
        else if (isDrifting)
        {
            isDrifting = false;
            SetWheelFriction(normalFriction);
            
            // Vuelta a posición neutral rápida
            chassis.localRotation = Quaternion.Slerp(
                chassis.localRotation,
                Quaternion.identity,
                driftTiltSpeed * 2f * Time.fixedDeltaTime
            );
            
            if (driftSound.isPlaying) driftSound.Stop();
        }
    }

    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed * (isNitroActive ? nitroBoost : 1f))
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed * (isNitroActive ? nitroBoost : 1f);
        }
    }
    #endregion

    #region Helpers
    void SetWheelFriction(float stiffness)
    {
        foreach (var wheel in driveWheels)
        {
            WheelFrictionCurve friction = wheel.sidewaysFriction;
            friction.stiffness = stiffness;
            wheel.sidewaysFriction = friction;
        }
    }

    void UpdateEngineSound()
    {
        if (!engineSound) return;
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        engineSound.pitch = 0.5f + speedFactor * 1.5f;
        engineSound.volume = 0.3f + speedFactor * 0.7f;
    }
    #endregion

    #region Nitro
    IEnumerator ActivateNitro()
    {
        isNitroActive = true;
        if (nitroSound) nitroSound.Play();
        yield return new WaitForSeconds(nitroDuration);
        isNitroActive = false;
    }
    #endregion

    #region Salto
    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & rampLayer) != 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    #endregion
}