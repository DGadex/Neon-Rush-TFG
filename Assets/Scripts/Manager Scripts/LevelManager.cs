using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject countdownPanel; // Panel UI con el texto "3, 2, 1, ¡GO!"
    public GameObject racePanel; // Panel UI con el texto "3, 2, 1, ¡GO!"
    public TextMeshProUGUI countdownText;
    public ArcadeCarController carController; // Script del coche
    public GameManager gameManager;

    [Header("Configuración")]
    public float countdownDuration = 3f; // Duración en segundos

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    // Corrutina para la cuenta atrás
    public IEnumerator StartCountdown()
    {
        carController.enabled = false; // Bloquea controles
        countdownPanel.SetActive(true);

        // Cuenta regresiva
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "¡GO!";
        carController.enabled = true; // Libera controles
        yield return new WaitForSeconds(1);
        countdownPanel.SetActive(false);
        racePanel.SetActive(true);
    }

    // Llamado al completar el nivel
    public void FinishLevel()
    {
        carController.enabled = false; // Bloquea controles
        // (El UIManager se encargará de mostrar el panel de fin)
    }
}