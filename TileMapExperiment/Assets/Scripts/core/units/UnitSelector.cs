
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

    private UnitSelector()
    {

    }

    public static UnitSelector GetInstance()
    {
        if (instance == null)
        {
            instance = new UnitSelector();
        }

        return instance;
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
            return false;
        }

        GameObject unit = UnitController.GetInstance().GetUnitAtTile(tilePos);

        if (unit != null)
        {
            Debug.Log("Selected Unit: " + unit.name);
            currSelectedUnit = unit;
        }

        return unit == null;
    }

}
