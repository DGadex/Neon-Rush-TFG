using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    //public GameObject mainMenuPanel;
    public GameObject countdownPanel;
    public GameObject raceUIPanel; // Muestra vueltas, tiempo
    public GameObject finishPanel; // Panel de fin de carrera

    [Header("Textos")]
    public TextMeshProUGUI lapsText;
    public TextMeshProUGUI timeText;
     public TextMeshProUGUI finalTimeText;

    void Start()
    {
        // Ejemplo: Ocultar todo excepto el menú principal al inicio
       // mainMenuPanel.SetActive(true);
        raceUIPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

    // Actualiza la UI durante la carrera
    public void UpdateRaceUI(int currentLap, int totalLaps, float time)
    {
        lapsText.text = $"Lap: {currentLap}/{totalLaps}";
        timeText.text = $"Time: {time:F2}s";
    }

    // Muestra el panel de fin de carrera
    public void ShowFinishPanel(float finalTime)
    {
        finishPanel.SetActive(true);
        raceUIPanel.SetActive(false);
        finalTimeText.text = $"Final Time: {finalTime:F2}s";
    }

    // Botones (vinculados desde el Inspector)
    public void OnRestartButton()
    {
        SceneLoaderManager.Instance.ReloadCurrentScene();
    }

    public void OnMenuButton()
    {
        SceneLoaderManager.Instance.LoadScene("Start");
    }
}