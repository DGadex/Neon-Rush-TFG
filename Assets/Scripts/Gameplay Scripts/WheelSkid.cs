using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelSkid : MonoBehaviour {


	[SerializeField]
	Rigidbody rb;
	public	Skidmarks skidmarksController;
	WheelCollider wheelCollider;
	WheelHit wheelHitInfo;

	const float SKID_FX_SPEED = 0.5f;
	const float MAX_SKID_INTENSITY = 20.0f;
	const float WHEEL_SLIP_MULTIPLIER = 10.0f;
	int lastSkid = -1;
	float lastFixedUpdateTime;
	protected void Awake() {
		wheelCollider = GetComponent<WheelCollider>();
		lastFixedUpdateTime = Time.time;
	}

	protected void FixedUpdate() {
		lastFixedUpdateTime = Time.time;
	}

	protected void LateUpdate() {
		if (wheelCollider.GetGroundHit(out wheelHitInfo))
		{

			Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
			float skidTotal = Mathf.Abs(localVelocity.x);

			float wheelAngularVelocity = wheelCollider.radius * ((2 * Mathf.PI * wheelCollider.rpm) / 60);
			float carForwardVel = Vector3.Dot(rb.linearVelocity, transform.forward);
			float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * WHEEL_SLIP_MULTIPLIER;

			wheelSpin = Mathf.Max(0, wheelSpin * (10 - Mathf.Abs(carForwardVel)));

			skidTotal += wheelSpin;

			if (skidTotal >= SKID_FX_SPEED) {
				float intensity = Mathf.Clamp01(skidTotal / MAX_SKID_INTENSITY);
				Vector3 skidPoint = wheelHitInfo.point + (rb.linearVelocity * (Time.time - lastFixedUpdateTime));
				lastSkid = skidmarksController.AddSkidMark(skidPoint, wheelHitInfo.normal, intensity, lastSkid);
			}
			else {
				lastSkid = -1;
			}
		}
		else {
			lastSkid = -1;
		}
	}
}
