using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Material metalMaterial; // Material metálico
    public Material matteMaterial; // Material mate
    public Material satinMaterial; // Material satinado

    private Renderer objRenderer;

    void Start()
    {
        objRenderer = GetComponent<Renderer>(); // Obtiene el Renderer del objeto
        objRenderer.material = matteMaterial; // Establece el material mate por defecto
    }

    public void SetMetalMaterial()
    {
        objRenderer.material = metalMaterial; // Cambia al material metálico
    }

    public void SetMatteMaterial()
    {
        objRenderer.material = matteMaterial; // Cambia al material mate
    }

    public void SetSatinMaterial()
    {
        objRenderer.material = satinMaterial; // Cambia al material mate
    }
}
