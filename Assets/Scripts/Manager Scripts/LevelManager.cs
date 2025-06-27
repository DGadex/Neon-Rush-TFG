using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject car;

    public GameObject countdownPanel;
    public GameObject racePanel;
    public TextMeshProUGUI countdownText;
    public ArcadeCarController carController;
    public GameManager gameManager;

    [Header("Configuración")]
    public float countdownDuration = 3f;

    void Start()
    {
    }

    public IEnumerator StartCountdown()
    {
        carController.enabled = false;
        countdownPanel.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "¡GO!";
        carController.enabled = true;
        yield return new WaitForSeconds(1);
        countdownPanel.SetActive(false);
        racePanel.SetActive(true);
    }

    public void SetupCar(GameObject carInstance)
    {
        this.car = carInstance;
        carController = carInstance.GetComponent<ArcadeCarController>();
        StartCoroutine(StartCountdown());
    }
    public void FinishLevel()
    {
        carController.enabled = false;
    }
}