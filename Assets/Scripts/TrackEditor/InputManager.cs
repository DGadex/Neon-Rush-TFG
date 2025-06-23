using System;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, onExit;
    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private FreeCameraController freeCameraController;

    [SerializeField]
    private GuardarJSON guardarJSON;

    [SerializeField]
    private Button guardarButton;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    private void Update()
    {
        if (!freeCameraController.probando)
        {
            if (Input.GetMouseButtonDown(0))
                OnClicked?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape))
                onExit?.Invoke();
            if (Input.GetKeyDown(KeyCode.UpArrow) && gridVisualization.transform.position.y <= 14*49)
                gridVisualization.transform.position += Vector3.up * 14;
            if (Input.GetKeyDown(KeyCode.DownArrow) && gridVisualization.transform.position.y >= 14)
                gridVisualization.transform.position += Vector3.down * 14;
        }
        Desactivate();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    public void Deselect()
    {
        onExit?.Invoke();
    }


    public void Desactivate()
    {
       if (string.IsNullOrWhiteSpace(guardarJSON.fileName.text) || !objectPlacer.escenarioProbado || !objectPlacer.hayMeta || !objectPlacer.hayCheckpoint)
        {
            guardarButton.interactable = false;
        }
        else
        {
            guardarButton.interactable = true;
        }

    }


}
