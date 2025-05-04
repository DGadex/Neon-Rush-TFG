using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    #region Configuración Básica
    [Header("Referencias")]
    public Transform chassis; // Modelo 3D del chasis (para inclinación visual)
    public WheelCollider[] driveWheels; // Ruedas motrices (asignar en Inspector)
    public WheelCollider[] steeringWheels; // Ruedas direccionales
    public InputActionAsset inputActions; // Sistema de Input

    [Header("Aceleración/Frenado")]
    public float maxMotorTorque = 6000f;    // Fuerza inicial (ajustar según peso)
    public float brakeTorque = 4000f;       // Freno potente pero no instantáneo
    public float maxSpeed = 120f;           // Velocidad máxima en unidades/Unity
    public AnimationCurve torqueCurve = new AnimationCurve(
        new Keyframe(0, 1f),    // Máximo torque al inicio
        new Keyframe(0.8f, 0.4f), // Reduce torque al 80% de velocidad
        new Keyframe(1, 0.1f)   // Mínimo torque al tope
    );

    [Header("Dirección")]
    public float maxSteeringAngle = 25f;    // Ángulo máximo de giro
    #endregion

    #region Derrape
    [Header("Derrape")]
    public float driftFriction = 0.4f;
    public float normalFriction = 1.5f;
    public float driftTiltAngle = 15f;
    public float driftRotationSpeed = 5f;
    public float minDriftSpeed = 15f; // Velocidad mínima para iniciar derrape
    private bool isDrifting = false;
    public float tiltSmoothness = 5f;
    #endregion

    #region Nitro
    [Header("Nitro")]
    public float nitroBoost = 1.8f;
    public float nitroDuration = 2.5f;
    public AudioSource nitroSound;
    private bool isNitroActive = false;
    #endregion

    #region Salto
    [Header("Salto")]
    public float jumpForce = 15f;
    public LayerMask rampLayer;
    #endregion

    #region Sonidos
    [Header("Sonidos")]
    public AudioSource driftSound;
    public AudioSource engineSound;
    #endregion

    #region Variables Privadas
    private Rigidbody rb;
    private float currentSteerAngle;
    private float steeringSpeed = 2f;
    private InputAction throttleAction, brakeAction, steerAction, driftAction, nitroAction;
    #endregion

    // Inicialización
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Bajar centro de masa

        // Configurar Input System
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

        // Configurar curva de torque por defecto si no está asignada
        if (torqueCurve == null || torqueCurve.length == 0)
        {
            torqueCurve = new AnimationCurve(
                new Keyframe(0, 1f),
                new Keyframe(0.7f, 0.6f),
                new Keyframe(1, 0.1f)
            );
        }
    }

    // Físicas en FixedUpdate
    void FixedUpdate()
    {
        float throttle = throttleAction.ReadValue<float>();
        float brake = brakeAction.ReadValue<float>();
        float steer = steerAction.ReadValue<Vector2>().x;
        bool drift = driftAction.ReadValue<float>() > 0.5f;
        bool nitro = nitroAction.triggered;

        ApplyMotor(throttle, brake);
        ApplySteering(steer);
        HandleDrift(drift, steer);
        LimitSpeed();

        if (nitro && !isNitroActive) StartCoroutine(ActivateNitro());
    }

    #region Funciones de Físicas
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
                // Freno motor suave
                wheel.motorTorque = 0f;
                wheel.brakeTorque = rb.linearVelocity.magnitude > 5f ? 1000f : 0f;
            }
        }

        // Actualizar sonido del motor (pitch basado en velocidad)
        if (engineSound) engineSound.pitch = 0.5f + speedFactor * 1.5f;
    }

    void ApplySteering(float steerInput)
    {
        currentSteerAngle = Mathf.Lerp(
            currentSteerAngle,
            steerInput * maxSteeringAngle * (1 - (rb.linearVelocity.magnitude / maxSpeed) * 0.7f),
            steeringSpeed * Time.fixedDeltaTime
        );

        foreach (var wheel in steeringWheels)
        {
            wheel.steerAngle = steerInput * this.maxSteeringAngle;  // "this" asegura que usas la variable de clase
        }
    }

    void HandleDrift(bool driftInput, float steerInput)
    {
        bool canDrift = rb.linearVelocity.magnitude > minDriftSpeed && Mathf.Abs(steerInput) > 0.3f;

        if (driftInput && canDrift)
        {
            isDrifting = true;
            SetWheelFriction(driftFriction);

            // Inclinación más controlada
            float targetTilt = -steerInput * driftTiltAngle;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt);
            chassis.localRotation = Quaternion.Lerp(chassis.localRotation, targetRotation, tiltSmoothness * Time.fixedDeltaTime);

            // Rotación más suave
            rb.AddTorque(transform.up * steerInput * driftRotationSpeed * 0.1f, ForceMode.VelocityChange); // Reduje la fuerza

            if (driftSound && !driftSound.isPlaying) driftSound.Play();
        }
        else if (isDrifting)
        {
            isDrifting = false;
            SetWheelFriction(Mathf.Lerp(GetCurrentFriction(), normalFriction, Time.fixedDeltaTime * 5f));
            
            // Vuelta a posición neutral más rápida
            chassis.localRotation = Quaternion.Lerp(chassis.localRotation, Quaternion.identity, Time.fixedDeltaTime * 10f);
            
            if (driftSound && driftSound.isPlaying) driftSound.Stop();
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

    #region Nitro
    IEnumerator ActivateNitro()
    {
        isNitroActive = true;
        if (nitroSound) nitroSound.Play();
        yield return new WaitForSeconds(nitroDuration);
        isNitroActive = false;
    }
    #endregion

    #region Salto (Rampas)
    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & rampLayer) != 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    float GetCurrentFriction()
    {
        return driveWheels[0].sidewaysFriction.stiffness;
    }
    #endregion
}