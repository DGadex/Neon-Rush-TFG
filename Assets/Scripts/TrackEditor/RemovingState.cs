using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData extraData;
    GridData trackData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData extraData,
                         GridData trackData,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.extraData = extraData;
        this.trackData = trackData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if(trackData.CanPlaceObjectAt(gridPosition, Vector3Int.one) == false)
        {
            selectedData = trackData;
        }
        else if(extraData.CanPlaceObjectAt(gridPosition, Vector3Int.one) == false)
        {
            selectedData = extraData;
        }
        if(selectedData == null)
        {
            //sonido de error
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if(gameObjectIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfValidPosition(gridPosition));
    }

    private bool CheckIfValidPosition(Vector3Int gridPosition)
    {
        return !(trackData.CanPlaceObjectAt(gridPosition, Vector3Int.one) && extraData.CanPlaceObjectAt(gridPosition, Vector3Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfValidPosition(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
