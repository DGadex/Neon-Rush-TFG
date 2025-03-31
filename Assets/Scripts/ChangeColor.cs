using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Renderer objectRenderer;  // Asigna el objeto con el material en Unity
    public Color newColor = Color.red; // Color deseado

    void Start()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }
}
