using UnityEngine;
using UnityEngine.VFX;

public class CollisionSparks : MonoBehaviour
{
    public GameObject sparkVFXPrefab;
    public float minImpactForce = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < minImpactForce) return;

        if (collision.collider.CompareTag("Player")) return;

        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Quaternion rot = Quaternion.LookRotation(contact.normal);

        GameObject sparks = Instantiate(sparkVFXPrefab, hitPoint, rot);
        Destroy(sparks, 2f);
    }
}
