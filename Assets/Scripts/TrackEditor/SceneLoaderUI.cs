using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class SceneLoaderUI : MonoBehaviour
{
    [SerializeField] private GuardarJSON guardarJSON;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject menuPanel;

    private string saveDirectory = "Assets/Resources/";

    private void Start()
    {
        PopulateMenu();
        menuPanel.SetActive(true);
    }

    private void PopulateMenu()
    {
        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);

        string[] files = Directory.GetFiles(saveDirectory, "*.json");

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            buttonObj.GetComponentInChildren<TMP_Text>().text = fileName;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => {
                LoadScene(fileName);
            });
        }
    }

    private void LoadScene(string fileName)
    {
        guardarJSON.LoadFromJson(fileName);
        menuPanel.SetActive(false);
    }

}
