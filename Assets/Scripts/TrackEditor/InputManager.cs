using System;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            onExit?.Invoke();
        if(Input.GetKeyDown(KeyCode.UpArrow))
            gridVisualization.transform.position += Vector3.up;
        if(Input.GetKeyDown(KeyCode.DownArrow))
            gridVisualization.transform.position += Vector3.down;
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
     {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
     }




}
