using UnityEngine;

public class ApplySpoiler : MonoBehaviour
{
    public GameObject[] spoilersRear;
    public GameObject[] spoilersFront;

    void Start()
    {
        int selectedSpoilerRearIndex = PlayerPrefs.GetInt("SelectedSpoilerRear", 0);  // Cargar el alerón seleccionado

        for (int i = 0; i < spoilersRear.Length; i++)
        {
            spoilersRear[i].SetActive(i == selectedSpoilerRearIndex);
        }
        
        int selectedSpoilerFrontIndex = PlayerPrefs.GetInt("SelectedSpoilerFront", 0);  // Cargar el alerón seleccionado

        for (int i = 0; i < spoilersFront.Length; i++)
        {
            spoilersFront[i].SetActive(i == selectedSpoilerFrontIndex);
        }
    }
}
