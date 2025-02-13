using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpoilerSelector : MonoBehaviour
{
    public GameObject[] spoilers;  // Array de alerones
    private int currentIndex = 0;  // Índice del alerón seleccionado

    public Text spoilerNameText;   // Texto para mostrar el nombre del alerón
    public Button confirmButton;   // Botón para confirmar la selección

    void Start()
    {
        ShowSpoiler(currentIndex);

        // Asignar función al botón de confirmación
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    // Mostrar el alerón seleccionado y ocultar los demás
    public void ShowSpoiler(int index)
    {
        for (int i = 0; i < spoilers.Length; i++)
        {
            spoilers[i].SetActive(i == index);
        }

        // Actualizar el texto con el nombre del alerón
        spoilerNameText.text = "Alerón: " + spoilers[index].name;
    }

    // Función para cambiar al alerón siguiente
    public void NextSpoiler()
    {
        currentIndex = (currentIndex + 1) % spoilers.Length;
        ShowSpoiler(currentIndex);
        Debug.Log(currentIndex);
    }

    // Función para cambiar al alerón anterior
    public void PreviousSpoiler()
    {
        currentIndex = (currentIndex - 1 + spoilers.Length) % spoilers.Length;
        ShowSpoiler(currentIndex);
        Debug.Log(currentIndex);
    }

    // Confirmar selección y guardar datos
    public void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedSpoiler", currentIndex);  // Guardar el índice seleccionado
        SceneManager.LoadScene("SampleScene");  // Cargar la escena de la carrera
    }
}
