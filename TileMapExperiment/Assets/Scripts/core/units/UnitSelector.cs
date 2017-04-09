
/// ---------------------------------------------------------------------------
/// UnitSelector.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 30th, 2017</date>
/// ---------------------------------------------------------------------------

using core.tilesys;
using core.units;
using UnityEngine;

public class UnitSelector
{
    private static UnitSelector instance;

    private GameObject currSelectedUnit;

    private GameObject selectionTile;

    /// <summary>
    /// Keeps track of the unit selection tile so we don't
    /// have to keep making new Vector3 all the time when
    /// we want to rescale the selection tile
    /// </summary>
    private Vector3 selectionScale;

    private UnitSelector()
    {
        selectionScale = new Vector3(1f, 1f, 1f);
    }

    public static UnitSelector GetInstance()
    {
        if (instance == null)
        {
            instance = new UnitSelector();
        }

        return instance;
    }

    public void SetSelectionTileGameObject(GameObject gameObject)
    {
        selectionTile = gameObject;
    }

    public GameObject GetCurrentlySelectedUnit()
    {
        return currSelectedUnit;
    }

    /// <summary>
    /// Returns true if a unit was found at that tile position.
    /// </summary>
    public bool TrySelectingUnitAt(Vector2 tilePos)
    {
        // Check if the tile is even occupied
        bool isTileOccupied = 
            MapController.GetInstance().IsTileOccupied((int)tilePos.x, (int)tilePos.y);

        // Bounce out if we're selecting an empty tile
        if (!isTileOccupied)
        {
            Deselect();
            return false;
        }

        GameObject unit = UnitController.GetInstance().GetUnitAtTile(tilePos);

        if (unit != null)
        {
            Debug.Log("Selected Unit: " + unit.name);
            currSelectedUnit = unit;
            MoveSelectionTileToUnit(currSelectedUnit);
        }
        else
        {
            Deselect();
        }

        return unit == null;
    }

    /// <summary>
    /// Moves the selection tile and sizes it to match the unit size
    /// </summary>
    public void MoveSelectionTileToUnit(GameObject unit)
    {
        GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();
        selectionScale.x = guc.sizeX;
        selectionScale.y = guc.sizeY;
        selectionTile.transform.localScale = selectionScale;

        selectionTile.SetActive(true);

        selectionTile.transform.position = unit.transform.position;
    }

    public void Deselect()
    {
        selectionTile.SetActive(false);
        currSelectedUnit = null;
    }

}
