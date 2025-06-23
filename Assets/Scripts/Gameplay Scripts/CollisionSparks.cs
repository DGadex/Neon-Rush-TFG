using UnityEngine;
using UnityEngine.VFX;

public class CollisionSparks : MonoBehaviour
{
    public GameObject sparkVFXPrefab;
    public float minImpactForce = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        // Evita que se activen con el suelo o contactos menores
        if (collision.relativeVelocity.magnitude < minImpactForce) return;

        // Solo si es contra algo que no sea otro coche, opcional:
        if (collision.collider.CompareTag("Player")) return;

        // Obtener punto de impacto
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Quaternion rot = Quaternion.LookRotation(contact.normal);

        // Instanciar el sistema de chispas
        GameObject sparks = Instantiate(sparkVFXPrefab, hitPoint, rot);
        Destroy(sparks, 2f); // Autoeliminar después de 2s
    }
}
