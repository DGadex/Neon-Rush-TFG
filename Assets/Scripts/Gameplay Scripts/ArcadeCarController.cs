using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeCarController : MonoBehaviour
{
    // Parámetros del coche
    public float maxMotorTorque = 3000f; // Fuerza de aceleración
    public float maxSteeringAngle = 30f; // Ángulo de giro
    public float driftFriction = 0.6f; // Fricción lateral para derrapar
    public float nitroBoost = 2000f; // Aumento de velocidad con nitro
    public float nitroDuration = 2f; // Duración del nitro
    public float jumpForce = 10f; // Fuerza de salto en rampas
    public float driftTiltAngle = 10f; // Ángulo de inclinación durante el derrape
    public float maxSpeed = 60f; // Velocidad máxima
    public float maxDriftRotation = 45f; // Máxima rotación en Y durante el derrape
    public Transform chassis; // Modelo 3D del chasis

    // WheelColliders
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    // Modelos 3D de las ruedas (opcional)
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    // Input System
    public InputActionAsset inputActions;
    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction turnAction;
    private InputAction driftAction;
    private InputAction nitroAction;

    // Efectos de sonido
    public AudioSource driftSound;
    public AudioSource engineSound;

    private Rigidbody rigidbody;
    private bool isNitroActive = false;
    private bool isDrifting = false;
    private bool driftInitialBool = false;
    private float driftStartYRotation = 0f; // Guarda la rotación al iniciar el derrape

    void Start() {
        rigidbody = GetComponent<Rigidbody>();

        // Configurar Input System
        var gameplayActionMap = inputActions.FindActionMap("Driving");
        accelerateAction = gameplayActionMap.FindAction("Throttle");
        brakeAction = gameplayActionMap.FindAction("Brake");
        turnAction = gameplayActionMap.FindAction("Steering");
        driftAction = gameplayActionMap.FindAction("Drift");
        nitroAction = gameplayActionMap.FindAction("Nitro");

        accelerateAction.Enable();
        brakeAction.Enable();
        turnAction.Enable();
        driftAction.Enable();
        nitroAction.Enable();

        // Bajar el centro de masa para evitar vuelcos
        rigidbody.centerOfMass = new Vector3(0, -0.7f, 0);
    }

    void Update() {
        // Obtener inputs
        float accelerateInput = accelerateAction.ReadValue<float>();
        float brakeInput = brakeAction.ReadValue<float>();
        float turnInput = turnAction.ReadValue<Vector2>().x;
        isDrifting = driftAction.ReadValue<float>() > 0;
        bool isNitroPressed = nitroAction.triggered;

        // Aplicar motor y dirección
        float motorTorque = (accelerateInput - brakeInput) * maxMotorTorque;
        rearLeftWheel.motorTorque = motorTorque;
        rearRightWheel.motorTorque = motorTorque;

        float steeringAngle = turnInput * maxSteeringAngle;
        frontLeftWheel.steerAngle = steeringAngle;
        frontRightWheel.steerAngle = steeringAngle;

        // Limitar la velocidad máxima
        if (rigidbody.linearVelocity.magnitude > maxSpeed) {
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * maxSpeed;
        }

        // Drift
        if (isDrifting) {
            ActivateDrift(turnInput);
        } else {
            DeactivateDrift();
        }

        // Nitro
        if (isNitroPressed && !isNitroActive) {
            StartCoroutine(ActivateNitro());
        }

        // Sincronizar modelos 3D de las ruedas (opcional)
        UpdateWheelVisuals(frontLeftWheel, frontLeftMesh);
        UpdateWheelVisuals(frontRightWheel, frontRightMesh);
        UpdateWheelVisuals(rearLeftWheel, rearLeftMesh);
        UpdateWheelVisuals(rearRightWheel, rearRightMesh);
    }

 /*   void ActivateDrift(float turnInput) {
    // Reducir fricción lateral para derrapar
    WheelFrictionCurve sidewaysFriction = rearLeftWheel.sidewaysFriction;
    sidewaysFriction.stiffness = driftFriction;
    rearLeftWheel.sidewaysFriction = sidewaysFriction;
    rearRightWheel.sidewaysFriction = sidewaysFriction;

    // Calcular la rotación deseada en función del input y el límite máximo
    float targetRotationY = turnInput * maxDriftRotation;

    // Calcular la rotación deseada relativa a la orientación actual del coche
    Quaternion targetRotation = Quaternion.Euler(0, rigidbody.rotation.eulerAngles.y + targetRotationY, 0);

    // Limitar la rotación para que no supere maxDriftRotation
    float currentRotationY = rigidbody.rotation.eulerAngles.y;
    float deltaRotationY = Mathf.DeltaAngle(currentRotationY, targetRotation.eulerAngles.y);

    if (Mathf.Abs(deltaRotationY) > maxDriftRotation) {
        targetRotation = Quaternion.Euler(0, currentRotationY + Mathf.Sign(deltaRotationY) * maxDriftRotation, 0);
    }

    // Suavizar la transición hacia la rotación deseada
    rigidbody.rotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, Time.deltaTime * 5f);

    // Inclinar el chasis
    float tilt = -turnInput * driftTiltAngle;
    chassis.rotation = Quaternion.Euler(0, rigidbody.rotation.eulerAngles.y, tilt);

    // Reproducir sonido de derrape
    if (driftSound != null && !driftSound.isPlaying) {
        driftSound.Play();
    }
}*/
/*
    void ActivateDrift(float turnInput) {
        // Reducir fricción lateral para derrapar
        WheelFrictionCurve sidewaysFriction = rearLeftWheel.sidewaysFriction;
        sidewaysFriction.stiffness = driftFriction;
        rearLeftWheel.sidewaysFriction = sidewaysFriction;
        rearRightWheel.sidewaysFriction = sidewaysFriction;

        // Calcular la rotación deseada durante el derrape
        float targetRotationY = turnInput * maxDriftRotation; //* (rigidbody.velocity.magnitude / maxSpeed) OPCIÓN PARA REGULAR POR VELOCIDAD, MIRARLO MAS ADELANTE

        // Limitar la rotación en Y
        Quaternion targetRotation = Quaternion.Euler(0, targetRotationY , 0); //+ rigidbody.rotation.eulerAngles.y
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Inclinar el chasis
        float tilt = -turnInput * driftTiltAngle;
        chassis.rotation = Quaternion.Euler(0, transform.eulerAngles.y, tilt);

        // Reproducir sonido de derrape
        if (driftSound != null && !driftSound.isPlaying) {
            driftSound.Play();
        }
    }
*/

    void ActivateDrift(float turnInput)
    {
        // Reducir fricción lateral para derrapar
        WheelFrictionCurve sidewaysFriction = rearLeftWheel.sidewaysFriction;
        sidewaysFriction.stiffness = driftFriction;
        rearLeftWheel.sidewaysFriction = sidewaysFriction;
        rearRightWheel.sidewaysFriction = sidewaysFriction;

        // Si el derrape inicia, guardamos la rotación actual
        if (!driftInitialBool)
        {
            driftStartYRotation = transform.eulerAngles.y;
            Debug.Log("Drift Start Y Rotation: " + driftStartYRotation);
            driftInitialBool = true;
        }

        // Calcular el ángulo de giro relativo a la rotación inicial
        float targetRotationY = driftStartYRotation + (turnInput * maxDriftRotation);

        // Limitar el ángulo de derrape
        float currentRotationY = transform.eulerAngles.y;
        float deltaRotationY = Mathf.DeltaAngle(currentRotationY, targetRotationY);

        if (Mathf.Abs(deltaRotationY) > maxDriftRotation)
        {
            targetRotationY = driftStartYRotation + Mathf.Sign(deltaRotationY) * maxDriftRotation;
        }

        // Aplicar la rotación suavemente
        Quaternion targetRotation = Quaternion.Euler(0, targetRotationY, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Inclinar el chasis
        float tilt = -turnInput * driftTiltAngle;
        chassis.rotation = Quaternion.Euler(0, transform.eulerAngles.y, tilt);

        // Reproducir sonido de derrape
        if (driftSound != null && !driftSound.isPlaying)
        {
            driftSound.Play();
        }
    }
    void DeactivateDrift() {
        // Restaurar fricción lateral normal
        WheelFrictionCurve sidewaysFriction = rearLeftWheel.sidewaysFriction;
        sidewaysFriction.stiffness = 1f;
        rearLeftWheel.sidewaysFriction = sidewaysFriction;
        rearRightWheel.sidewaysFriction = sidewaysFriction;

        // Enderezar el coche y el chasis
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime * 5f);
        chassis.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        driftInitialBool = false;

        // Detener sonido de derrape
        if (driftSound != null && driftSound.isPlaying) {
            driftSound.Stop();
        }
    }

    IEnumerator ActivateNitro() {
        isNitroActive = true;
        maxMotorTorque += nitroBoost;
        yield return new WaitForSeconds(nitroDuration);
        maxMotorTorque -= nitroBoost;
        isNitroActive = false;
    }

    void OnCollisionEnter(Collision collision) {
        // Saltar en rampas
        if (collision.gameObject.CompareTag("Ramp")) {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void UpdateWheelVisuals(WheelCollider collider, Transform mesh) {
        if (collider == null || mesh == null) return;

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        mesh.position = position;
        mesh.rotation = rotation;
    }
}