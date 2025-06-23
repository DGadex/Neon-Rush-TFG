using UnityEngine;
using System.Collections;

public class SpawnDissolveController : MonoBehaviour
{
    public Renderer dissolveRenderer;
    public string dissolveProperty = "_Clip";
    public float dissolveDuration = 1.5f;

    private Material dissolveMaterial;

    void Awake()
    {
        if (dissolveRenderer != null)
        {
            // Asegurarse de usar una copia del material
            dissolveMaterial = dissolveRenderer.material;
        }
    }

    public void PlayDissolve()
    {
        if (dissolveMaterial != null)
        {
            StopAllCoroutines();
            StartCoroutine(DissolveCoroutine());
        }
    }

    private IEnumerator DissolveCoroutine()
    {
        float t = 0f;
        while (t < dissolveDuration)
        {
            float clipValue = Mathf.Lerp(0f, 1f, t / dissolveDuration);
            dissolveMaterial.SetFloat(dissolveProperty, clipValue);
            t += Time.deltaTime;
            yield return null;
        }

        dissolveMaterial.SetFloat(dissolveProperty, 1f); // Asegurar que termina en 1
    }
}
