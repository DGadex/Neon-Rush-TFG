using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpoilerSelector : MonoBehaviour
{
    public GameObject[] spoilersRear;  // Array de alerones
    public GameObject[] spoilersFront;  // Array de alerones
    private int currentRearIndex = 0;  // Índice del alerón seleccionado
    private int currentFrontIndex = 0;  // Índice del alerón seleccionado
    public Button confirmButton;   // Botón para confirmar la selección
    public Button backButton;      // Botón para cambiar alerón siguiente

    void Start()
    {
        currentRearIndex = PlayerPrefs.GetInt("SelectedSpoilerRear", 0);
        ShowSpoilerRear(currentRearIndex);
        currentFrontIndex = PlayerPrefs.GetInt("SelectedSpoilerFront", 0);
        ShowSpoilerFront(currentFrontIndex);

        // Asignar función al botón de confirmación
        confirmButton.onClick.AddListener(ConfirmSelection);
        backButton.onClick.AddListener(GoBack);
    }

    // Mostrar el alerón seleccionado y ocultar los demás
    public void ShowSpoilerRear(int index)
    {
        for (int i = 0; i < spoilersRear.Length; i++)
        {
            spoilersRear[i].SetActive(i == index);
        }
    }
    public void ShowSpoilerFront(int index)
    {
        for (int i = 0; i < spoilersFront.Length; i++)
        {
            spoilersFront[i].SetActive(i == index);
        }
    }

    // Función para cambiar al alerón siguiente
    public void NextSpoilerRear()
    {
        currentRearIndex = (currentRearIndex + 1) % spoilersRear.Length;
        ShowSpoilerRear(currentRearIndex);
        Debug.Log(currentRearIndex);
    }
    public void NextSpoilerFront()
    {
        currentFrontIndex = (currentFrontIndex + 1) % spoilersFront.Length;
        ShowSpoilerFront(currentFrontIndex);
        Debug.Log(currentFrontIndex);
    }

    // Función para cambiar al alerón anterior
    public void PreviousSpoilerRear()
    {
        currentRearIndex = (currentRearIndex - 1 + spoilersRear.Length) % spoilersRear.Length;
        ShowSpoilerRear(currentRearIndex);
        Debug.Log(currentRearIndex);
    }
    public void PreviousSpoilerFront()
    {
        currentFrontIndex = (currentFrontIndex - 1 + spoilersFront.Length) % spoilersFront.Length;
        ShowSpoilerFront(currentFrontIndex);
        Debug.Log(currentFrontIndex);
    }

    // Confirmar selección y guardar datos
    public void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedSpoilerRear", currentRearIndex);  // Guardar el índice seleccionado
        PlayerPrefs.SetInt("SelectedSpoilerFront", currentFrontIndex);  // Guardar el índice seleccionado
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Start"); // Cargar la escena principal
    }
}
