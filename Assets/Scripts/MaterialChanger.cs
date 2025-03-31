using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Material metalMaterial; // Material metálico
    public Material matteMaterial; // Material mate
    public Material satinMaterial; // Material satinado

    private Renderer objRenderer;

    private int materialIndex; // Índice del material actual

    private Color currentColor;

    void Start()
    {
        objRenderer = GetComponent<Renderer>(); // Obtiene el Renderer del objeto

        // Recuperar el índice del material almacenado
        materialIndex = PlayerPrefs.GetInt("SavedMaterialIndex", 0);

        ApplyMaterial(); // Aplicar el material guardado
    }

    private void ApplyMaterial()
    {
        switch (materialIndex)
        {
            case 0:
                objRenderer.material = matteMaterial;
                break;
            case 1:
                objRenderer.material = metalMaterial;
                break;
            case 2:
                objRenderer.material = satinMaterial;
                break;
        }
    }

        public void SetMatteMaterial()
    {
        currentColor = objRenderer.sharedMaterial.color;
        materialIndex = 0;
        objRenderer.sharedMaterial = matteMaterial;
        objRenderer.sharedMaterial.color = currentColor; // Mantener el color
        PlayerPrefs.SetInt("SavedMaterialIndex", materialIndex);
        PlayerPrefs.Save();
    }
    public void SetMetalMaterial()
    {
        currentColor = objRenderer.sharedMaterial.color;
        materialIndex = 1;
        objRenderer.sharedMaterial = metalMaterial;
        objRenderer.sharedMaterial.color = currentColor; // Mantener el color
        PlayerPrefs.SetInt("SavedMaterialIndex", materialIndex); // Guardar el índice
        PlayerPrefs.Save(); // Guardar los cambios
    }

    public void SetSatinMaterial()
    {
        currentColor = objRenderer.sharedMaterial.color;
        materialIndex = 2;
        objRenderer.sharedMaterial = satinMaterial;
        objRenderer.sharedMaterial.color = currentColor; // Mantener el color
        PlayerPrefs.SetInt("SavedMaterialIndex", materialIndex);
        PlayerPrefs.Save();
    }
}