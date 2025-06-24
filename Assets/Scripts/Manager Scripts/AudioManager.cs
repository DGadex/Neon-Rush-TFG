using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Música")]
    public AudioSource menuMusic;
    public AudioSource playMusic;
    public AudioSource editorMusic;

    [Header("Efectos comunes")]
    public AudioSource buttonClick;
    public AudioSource confirmColor;
    public AudioSource changePart;
    public AudioSource crashSound;
    public AudioSource checkpointSound;
    public AudioSource finishLineSound;
    public AudioSource nitroSound;
    public AudioSource respawnSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistencia entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(AudioSource sfx)
    {
        if (sfx != null && !sfx.isPlaying)
            sfx.Play();
    }

    public void FadeOtherSounds(float targetVolume, float duration)
    {
        // Aquí puedes implementar un FadeOut temporal de otras pistas si quieres
        // Ej: durante el nitro
    }
}
