using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

public class UIFlowManager : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera startCamera;
    public CinemachineCamera mainMenuCamera;
    public CinemachineCamera optionsCamera;

    [Header("Canvas Groups")]
    public CanvasGroup startCanvasGroup;

    [Header("Canvas Panels")]
    public GameObject mainMenuCanvas;
    public GameObject optionsCanvas;

    [Header("Fade Config")]
    public float fadeDuration = 1f;

    private CinemachineCamera currentCamera;

    void Start()
    {
        currentCamera = startCamera;

        // Estado inicial
        SetCameraPriority(startCamera, 20);
        SetCameraPriority(mainMenuCamera, 10);
        SetCameraPriority(optionsCamera, 5);

        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
    }

    #region --- Start Flow ---
    public void OnStartButtonPressed()
    {
        StartCoroutine(FadeOutAndSwitch(startCanvasGroup, mainMenuCamera, () =>
        {
            mainMenuCanvas.SetActive(true);
        }));
    }
    #endregion

    #region --- Options Flow ---
    public void OnOptionsButtonPressed()
    {
        SwitchCamera(optionsCamera);
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void OnBackFromOptions()
    {
        SwitchCamera(mainMenuCamera);
        optionsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    #endregion

    #region --- Utility ---
    private void SwitchCamera(CinemachineCamera targetCamera)
    {
        SetCameraPriority(currentCamera, 10);      // Baja prioridad
        SetCameraPriority(targetCamera, 20);       // Sube prioridad
        currentCamera = targetCamera;
    }

    private void SetCameraPriority(CinemachineCamera cam, int priority)
    {
        if (cam != null)
            cam.Priority = priority;
    }

    private IEnumerator FadeOutAndSwitch(CanvasGroup canvas, CinemachineCamera nextCamera, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        canvas.gameObject.SetActive(false);
        SwitchCamera(nextCamera);
        onComplete?.Invoke();
    }
    #endregion
}
