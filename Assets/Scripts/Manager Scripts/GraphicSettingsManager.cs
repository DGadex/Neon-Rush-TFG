using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro; // Si usas TMP para dropdowns

public class GraphicsSettingsManager : MonoBehaviour
{
    public Volume volume;

    private Bloom bloom;
    private MotionBlur motionBlur;
    private LensDistortion lensDistortion;

    void Start()
    {
        // Asumimos que todos los efectos ya están en el Volume Profile
        if (volume.profile.TryGet(out bloom))
            bloom.active = true;

        if (volume.profile.TryGet(out motionBlur))
            motionBlur.active = true;

        if (volume.profile.TryGet(out lensDistortion))
            lensDistortion.active = true;
    }

    public void SetLensFlareIntensity(int optionIndex)
    {
        // Simulamos "intensidad de lens flare" usando bloom
        switch (optionIndex)
        {
            case 0: bloom.intensity.value = 0f; break;       // OFF
            case 1: bloom.intensity.value = 0.3f; break;     // Low
            case 2: bloom.intensity.value = 0.7f; break;     // Medium
            case 3: bloom.intensity.value = 1.1f; break;     // High
        }
    }

    public void SetMotionBlur(int optionIndex)
    {
        if (motionBlur == null) return;

        switch (optionIndex)
        {
            case 0: motionBlur.intensity.value = 0f; break;     // OFF
            case 1: motionBlur.intensity.value = 0.3f; break;   // Low
            case 2: motionBlur.intensity.value = 0.6f; break;   // Medium
            case 3: motionBlur.intensity.value = 0.9f; break;   // High
        }
    }

    public void SetGraphicsQuality(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0:
                QualitySettings.SetQualityLevel(0); // Low
                break;
            case 1:
                QualitySettings.SetQualityLevel(2); // Medium
                break;
            case 2:
                QualitySettings.SetQualityLevel(5); // High
                break;
        }
    }
}
