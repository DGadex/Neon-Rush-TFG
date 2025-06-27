using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    //public GameObject mainMenuPanel;
    public GameObject countdownPanel;
    public GameObject raceUIPanel;
    public GameObject finishPanel;

    [Header("Textos")]
    public TextMeshProUGUI lapsText;
    public TextMeshProUGUI timeText;
     public TextMeshProUGUI finalTimeText;

    void Start()
    {

       // mainMenuPanel.SetActive(true);
        raceUIPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

    public void UpdateRaceUI(int currentLap, int totalLaps, float time)
    {
        lapsText.text = $"Lap: {currentLap}/{totalLaps}";
        timeText.text = $"Time: {time:F2}s";
    }

    public void ShowFinishPanel(float finalTime)
    {
        finishPanel.SetActive(true);
        raceUIPanel.SetActive(false);
        finalTimeText.text = $"Final Time: {finalTime:F2}s";
    }

    public void OnRestartButton()
    {
        SceneLoaderManager.Instance.ReloadCurrentScene();
    }

    public void OnMenuButton()
    {
        SceneLoaderManager.Instance.LoadScene("Start");
    }
}