using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform car;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float rotationSpeed = 5f;

    void LateUpdate() {
        Vector3 desiredPosition = car.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        Quaternion desiredRotation = Quaternion.LookRotation(car.position - transform.position, car.up);

        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
    }
}