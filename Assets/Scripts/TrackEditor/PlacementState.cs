using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData extraData;
    GridData trackData;
    ObjectPlacer objectPlacer;

    private Quaternion currentRotation = Quaternion.identity;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.extraData = floorData;
        this.trackData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        objectPlacer.placementSound?.Play();

        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), currentRotation, database.objectsData[selectedObjectIndex].ID);

        GridData selectedData = database.objectsData[selectedObjectIndex].ID < 0 ? extraData : trackData; //Menor que 0 para qu el objeto se pueda colocar encima de otros trozos de pista
        selectedData.AddObjectAt(gridPosition,
                    database.objectsData[selectedObjectIndex].Size,
                    database.objectsData[selectedObjectIndex].ID,
                    index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID < 0 ? extraData : trackData; //Menor que 0 para qu el objeto se pueda colocar encima de otros trozos de pista
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {  

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
    
    public void RotatePreview()
    {
        currentRotation *= Quaternion.Euler(0, 90, 0);
        previewSystem.ApplyRotation(currentRotation);
    }
}
