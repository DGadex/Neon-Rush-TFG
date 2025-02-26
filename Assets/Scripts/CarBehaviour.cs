using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarBehaviour : MonoBehaviour
{
    [Header("Suspension Settings")]
    public Transform[] wheels; // Array de transformaciones de las ruedas
    public float suspensionRestDistance = 0.5f; // Distancia de reposo de la suspensión
    public float springStrength = 35000f; // Fuerza del muelle de la suspensión
    public float springDamper = 4500f; // Amortiguador de la suspensión

    [Header("Steering Settings")]
    public float tireMass = 20f; // Masa de cada neumático
    public float tireGripFactor = 1.0f; // Factor de agarre del neumático

    [Header("Acceleration / Braking Settings")]
    public AnimationCurve powerCurve; // Curva de potencia del coche
    public float carTopSpeed = 200f; // Velocidad máxima del coche
    public bool isFrontWheelDrive = true; // Indica si el coche es de tracción delantera

    private float accelerationInput; // Entrada de aceleración
    private float brakeInput; // Entrada de frenado
    private float steeringInput; // Entrada de dirección
    
    private Rigidbody carRigidBody; // Componente Rigidbody del coche
    private CarControls carControls; // Controles del coche

    // Estados del coche para gestionar aceleración y frenado
    enum CarState { Accelerating, Braking, Neutral }
    CarState currentState = CarState.Neutral;

    void Awake()
    {
        carRigidBody = GetComponent<Rigidbody>();

        // Inicializar el sistema de inputs
        carControls = new CarControls();
        
        // Conectar las acciones del Input System a nuestras variables
        carControls.Driving.Throttle.performed += ctx => accelerationInput = ctx.ReadValue<float>();
        carControls.Driving.Brake.performed += ctx => brakeInput = ctx.ReadValue<float>();
        carControls.Driving.Steering.performed += ctx => 
        {
            Vector2 steering = ctx.ReadValue<Vector2>();
            steeringInput = steering.x;
        };
    }

    void OnEnable()
    {
        carControls.Enable();
    }

    void OnDisable()
    {
        carControls.Disable();
    }

    void FixedUpdate()
    {
        UpdateState();

        // Calcula el grip base
        float baseGripFactor = 2.0f; // Ajusta este valor según sea necesario
        float carSpeed = carRigidBody.linearVelocity.magnitude;
        float gripAdjustment = Mathf.Clamp(1 - (carSpeed / carTopSpeed), 0.5f, 1);
        tireGripFactor = baseGripFactor * gripAdjustment * (1 - Mathf.Abs(steeringInput) * 0.5f);

        foreach (var wheel in wheels)
        {
            ApplySuspension(wheel);
            ApplySteering(wheel);
            ApplyAccelerationAndBraking(wheel);
        }

        // Añadir fuerzas laterales para mejorar la tracción en curvas
        Vector3 lateralForce = Vector3.Cross(carRigidBody.linearVelocity, transform.up) * tireGripFactor;
        carRigidBody.AddForce(lateralForce);

        // Control de estabilidad
        if (Mathf.Abs(carRigidBody.angularVelocity.y) > 0.5f)
        {
            Vector3 correctiveForce = -carRigidBody.angularVelocity * tireGripFactor * 0.1f; // Reduce el factor correctivo
            carRigidBody.AddTorque(correctiveForce);
        }
    }

    // Actualiza el estado del coche basado en las entradas de aceleración y frenado
    void UpdateState()
    {
        if (accelerationInput > 0)
            currentState = CarState.Accelerating;
        else if (brakeInput > 0)
            currentState = CarState.Braking;
        else
            currentState = CarState.Neutral;
    }

    // Aplica la suspensión a cada rueda
    void ApplySuspension(Transform wheel)
    {
        RaycastHit hit;
        if (Physics.Raycast(wheel.position, -wheel.up, out hit, suspensionRestDistance))
        {
            float offset = suspensionRestDistance - hit.distance;
            Vector3 springDir = wheel.up;
            Vector3 tireWorldVel = carRigidBody.GetPointVelocity(wheel.position);
            float velocity = Vector3.Dot(springDir, tireWorldVel);
            float force = (offset * springStrength) - (velocity * springDamper);
            carRigidBody.AddForceAtPosition(springDir * force, wheel.position);

            // Debug Ray for Suspension
            Debug.DrawRay(wheel.position, -wheel.up * hit.distance, Color.green);  // Raycast de suspensión
            Debug.DrawRay(wheel.position, springDir * force * 0.001f, Color.yellow);  // Fuerza de suspensión
        }
    }

    // Aplica la dirección a las ruedas delanteras
    void ApplySteering(Transform wheel)
    {
        RaycastHit hit;
        if (Physics.Raycast(wheel.position, -wheel.up, out hit, suspensionRestDistance))
        {
            bool isFrontWheel = (wheel == wheels[0] || wheel == wheels[1]);
            if (!isFrontWheel) return;

            float carSpeed = carRigidBody.linearVelocity.magnitude;
            float maxSteeringAngle = Mathf.Lerp(30f, 10f, carSpeed / carTopSpeed);
            float steeringAngle = -steeringInput * maxSteeringAngle;

            Vector3 steeringDir = Quaternion.AngleAxis(steeringAngle, transform.up) * transform.forward;
            Vector3 tireWorldVel = carRigidBody.GetPointVelocity(wheel.position);
            float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

            // Cambiar el cálculo de desiredVelChange para ser más suave
            float desiredVelChange = -steeringVel * tireGripFactor;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

            // Asegúrate de que no se aplique una fuerza excesiva
            float maxForce = tireMass * 100; // Ajusta este valor si es necesario
            float appliedForce = Mathf.Clamp(desiredAccel * tireMass, -maxForce, maxForce);

            carRigidBody.AddForceAtPosition(steeringDir * appliedForce, wheel.position);

            Debug.DrawRay(wheel.position, steeringDir * appliedForce * 0.001f, Color.cyan);
        }
    }

    // Aplica la aceleración y el frenado a las ruedas
    void ApplyAccelerationAndBraking(Transform wheel)
    {
        RaycastHit hit;
        if (Physics.Raycast(wheel.position, -wheel.up, out hit, suspensionRestDistance))
        {
            Vector3 accelDir = wheel.forward;
            bool applyForce = (isFrontWheelDrive && (wheel == wheels[0] || wheel == wheels[1])) ||
                              (!isFrontWheelDrive && (wheel == wheels[2] || wheel == wheels[3]));

            if (applyForce)
            {
                float carSpeed = Vector3.Dot(transform.forward, carRigidBody.linearVelocity);
                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
                float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelerationInput;

                // Aplicar fuerza de aceleración solo si hay input de aceleración
                if (accelerationInput > 0)
                {
                    carRigidBody.AddForceAtPosition(accelDir * availableTorque * 500, wheel.position);
                    Debug.DrawRay(wheel.position, accelDir * availableTorque * 500 * 0.001f, Color.red); // Debug de aceleración
                }

                // Freno
                if (brakeInput > 0.0f)
                {
                    float brakeForce = brakeInput * carRigidBody.mass * 10f; // Ajusta el multiplicador según sea necesario
                    Vector3 brakeDirection = -carRigidBody.linearVelocity.normalized;
                    carRigidBody.AddForce(brakeDirection * brakeForce, ForceMode.Force);
                    Debug.DrawRay(wheel.position, brakeDirection * brakeForce * 0.001f, Color.magenta); // Debug de frenado
                }

                // Freno motor (reducción de velocidad cuando no hay aceleración)
                if (accelerationInput == 0 && brakeInput == 0)
                {
                    float engineBrakeForce = carRigidBody.mass * 2f; // Ajusta este valor según sea necesario
                    Vector3 engineBrakeDirection = -carRigidBody.linearVelocity.normalized;
                    carRigidBody.AddForce(engineBrakeDirection * engineBrakeForce, ForceMode.Force);
                }
            }
        }
    }
}
