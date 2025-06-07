using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine; // Necesario para Cinemachine

public class TestSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject carPrefab;
    public Transform spawnPoint;
    public Button spawnButton;

    [Header("Cámaras")]
    public GameObject freeLookCamera; // Cámara libre (WASD)
    public CinemachineVirtualCamera followCamera; // Cámara que sigue al coche

    private GameObject currentCar;
    private bool isCarSpawned = false;

    void Start()
    {
        spawnButton.onClick.AddListener(ToggleCar);
        
        // Configuración inicial de cámaras
        if (freeLookCamera != null) freeLookCamera.SetActive(true);
        if (followCamera != null) followCamera.gameObject.SetActive(false);
    }

    void ToggleCar()
    {
        if (isCarSpawned)
        {
            Destroy(currentCar);
            currentCar = null;
            isCarSpawned = false;
            
            // Volver a cámara libre
            if (freeLookCamera != null) freeLookCamera.SetActive(true);
            if (followCamera != null) followCamera.gameObject.SetActive(false);
            
            Debug.Log("🚗 Coche eliminado - Modo edición");
        }
        else
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            currentCar = Instantiate(carPrefab, spawnPosition, Quaternion.identity);
            isCarSpawned = true;
            
            // Configurar cámara de seguimiento
            if (followCamera != null)
            {
                followCamera.gameObject.SetActive(true);
                followCamera.Follow = currentCar.transform;
                followCamera.LookAt = currentCar.transform;
            }
            
            if (freeLookCamera != null) freeLookCamera.SetActive(false);
            
            Debug.Log("🚗 Coche creado - Modo prueba");
        }
    }

    void Update()
    {
        if (isCarSpawned) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentCar = Instantiate(carPrefab, hit.point + Vector3.up * 0.5f, Quaternion.identity);
                isCarSpawned = true;
                
                // Configurar cámara al spawnear con click
                if (followCamera != null)
                {
                    followCamera.gameObject.SetActive(true);
                    followCamera.Follow = currentCar.transform;
                    followCamera.LookAt = currentCar.transform;
                }
                
                if (freeLookCamera != null) freeLookCamera.SetActive(false);
            }
        }
    }
}