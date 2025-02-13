using UnityEngine;

public class ApplySpoiler : MonoBehaviour
{
    public GameObject[] spoilers;

    void Start()
    {
        int selectedSpoilerIndex = PlayerPrefs.GetInt("SelectedSpoiler", 0);  // Cargar el alerón seleccionado

        for (int i = 0; i < spoilers.Length; i++)
        {
            spoilers[i].SetActive(i == selectedSpoilerIndex);
        }
    }
}
