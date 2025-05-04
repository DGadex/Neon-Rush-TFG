using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform car; // Referencia al coche
    public Vector3 offset; // Offset de la cámara respecto al coche
    public float smoothSpeed = 0.125f; // Suavizado del movimiento de la cámara
    public float rotationSpeed = 5f; // Velocidad de rotación de la cámara

    void LateUpdate() {
        // Posición deseada de la cámara (coche + offset)
        Vector3 desiredPosition = car.position + offset;

        // Suavizar el movimiento de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Rotación deseada de la cámara (mirar hacia el coche)
        Quaternion desiredRotation = Quaternion.LookRotation(car.position - transform.position, car.up);

        // Suavizar la rotación de la cámara
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
    }
}