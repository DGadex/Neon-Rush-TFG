using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
public class StartController : MonoBehaviour
{
    public Button playButton;
    public Button editCarButton;
    public Button editTrackButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(Play);
        editCarButton.onClick.AddListener(EditCar);
        editTrackButton.onClick.AddListener(EditTrack);
    }

     public void Play()
    {
        SceneManager.LoadScene("Play_Mode"); // Cargar la escena de juego
    }
    public void EditCar()
    {
        SceneManager.LoadScene("Car_Editor"); // Cargar la escena del editor de coches
    }
    public void EditTrack()
    {
        SceneManager.LoadScene("Track_Editor"); // Cargar la escena del editor de pistas
    }
}
