//CODIGO ANTIGUO DEL SISTEMA DEL COCHE


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarBehaviour : MonoBehaviour
{
    [Header("Suspension Settings")]
    public Transform[] wheels;
    public float suspensionRestDistance = 0.5f;
    public float springStrength = 35000f;
    public float springDamper = 4500f;

    [Header("Steering Settings")]
    public float tireMass = 20f;
    public float tireGripFactor = 1.0f;

    [Header("Acceleration / Braking Settings")]
    public AnimationCurve powerCurve;
    public float carTopSpeed = 200f;
    public bool isFrontWheelDrive = true;

    private float accelerationInput;
    private float brakeInput;
    private float steeringInput;
    
    private Rigidbody carRigidBody;
    private CarControls carControls; 


    enum CarState { Accelerating, Braking, Neutral }
    CarState currentState = CarState.Neutral;

    void Awake()
    {
        carRigidBody = GetComponent<Rigidbody>();


        carControls = new CarControls();
        

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


        float baseGripFactor = 2.0f;
        float carSpeed = carRigidBody.linearVelocity.magnitude;
        float gripAdjustment = Mathf.Clamp(1 - (carSpeed / carTopSpeed), 0.5f, 1);
        tireGripFactor = baseGripFactor * gripAdjustment * (1 - Mathf.Abs(steeringInput) * 0.5f);

        foreach (var wheel in wheels)
        {
            ApplySuspension(wheel);
            ApplySteering(wheel);
            ApplyAccelerationAndBraking(wheel);
        }


        Vector3 lateralForce = Vector3.Cross(carRigidBody.linearVelocity, transform.up) * tireGripFactor;
        carRigidBody.AddForce(lateralForce);


        if (Mathf.Abs(carRigidBody.angularVelocity.y) > 0.5f)
        {
            Vector3 correctiveForce = -carRigidBody.angularVelocity * tireGripFactor * 0.1f;
            carRigidBody.AddTorque(correctiveForce);
        }
    }


    void UpdateState()
    {
        if (accelerationInput > 0)
            currentState = CarState.Accelerating;
        else if (brakeInput > 0)
            currentState = CarState.Braking;
        else
            currentState = CarState.Neutral;
    }


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


            Debug.DrawRay(wheel.position, -wheel.up * hit.distance, Color.green);
            Debug.DrawRay(wheel.position, springDir * force * 0.001f, Color.yellow);
        }
    }


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

         
            float desiredVelChange = -steeringVel * tireGripFactor;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

            
            float maxForce = tireMass * 100;
            float appliedForce = Mathf.Clamp(desiredAccel * tireMass, -maxForce, maxForce);

            carRigidBody.AddForceAtPosition(steeringDir * appliedForce, wheel.position);

            Debug.DrawRay(wheel.position, steeringDir * appliedForce * 0.001f, Color.cyan);
        }
    }

  
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

               
                if (accelerationInput > 0)
                {
                    carRigidBody.AddForceAtPosition(accelDir * availableTorque * 500, wheel.position);
                    Debug.DrawRay(wheel.position, accelDir * availableTorque * 500 * 0.001f, Color.red);

            
                if (brakeInput > 0.0f)
                {
                    float brakeForce = brakeInput * carRigidBody.mass * 10f; 
                    Vector3 brakeDirection = -carRigidBody.linearVelocity.normalized;
                    carRigidBody.AddForce(brakeDirection * brakeForce, ForceMode.Force);
                    Debug.DrawRay(wheel.position, brakeDirection * brakeForce * 0.001f, Color.magenta);
                }

              
                if (accelerationInput == 0 && brakeInput == 0)
                {
                    float engineBrakeForce = carRigidBody.mass * 2f;
                    Vector3 engineBrakeDirection = -carRigidBody.linearVelocity.normalized;
                    carRigidBody.AddForce(engineBrakeDirection * engineBrakeForce, ForceMode.Force);
                }
            }
        }
    }
}*/
