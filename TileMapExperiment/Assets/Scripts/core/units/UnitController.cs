/// ---------------------------------------------------------------------------
/// UnitController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 12th, 2017</date>
/// ---------------------------------------------------------------------------

using core.assets;
using core.tilesys;
using System.Collections.Generic;
using UnityEngine;

namespace core.units
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitController
    {
        private static UnitController instance;

        private Dictionary<string, GameObject> unitMap = new Dictionary<string, GameObject>();

        private UnitController()
        {
            
        }

        public static UnitController GetInstance()
        {
            if (instance == null)
            {
                instance = new UnitController();
            }

            return instance;
        }

        /// <summary>
        /// Handles updating state on units.
        /// </summary>
        public void Update()
        {

        }

        public GameObject GetUnitByID(string id)
        {
            if (unitMap.ContainsKey(id))
            {
                return unitMap[id];
            }

            return null;
        }

        /// <summary>
        /// Creates a new unit in the world with the given name and
        /// loads the given texture. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public GameObject PlaceNewUnit(string id, string assetID)
        {
            Vector2 pos = new Vector2(0, 0);

            return PlaceNewUnit(id, assetID, pos);
        }

        public GameObject PlaceNewUnit(string id, string assetID, Vector2 pos)
        {
            GameObject newUnit = new GameObject();
            newUnit.name = id;

            // Load the sprite for this unit.
            SpriteRenderer spriteRenderer = newUnit.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetManager.GetInstance().GetPreloadedSprite(assetID);
            spriteRenderer.sortingOrder = 5;

            GameUnitComponent guc = newUnit.AddComponent<GameUnitComponent>();

            // Determine the tile size based off of the sprite data
            guc.sizeX = Mathf.CeilToInt(spriteRenderer.sprite.textureRect.width / spriteRenderer.sprite.pixelsPerUnit);
            guc.sizeY = Mathf.CeilToInt(spriteRenderer.sprite.textureRect.height / spriteRenderer.sprite.pixelsPerUnit); ;

            // Position this new unit
            newUnit.transform.position = MapCoordinateUtils.GetTileToWorldPosition((int)pos.x, (int)pos.y);

            // Add it to the mapping of units
            unitMap[newUnit.name] = newUnit;

            MapController.GetInstance().PlaceUnit(newUnit, pos);

            return newUnit;
        }

        public void ReplaceAllUnits()
        {
            // Go through all of the units and place them down again. 
            foreach (KeyValuePair<string, GameObject> entry in unitMap)
            {
                GameUnitComponent guc = entry.Value.GetComponent<GameUnitComponent>();
                MapController.GetInstance().PlaceUnit(entry.Value, guc.CurrentTilePos);
            }
        }
    }
}