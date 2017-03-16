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

namespace core
{
    public class UnitController
    {
        private static UnitController instance;

        Dictionary<string, GameObject> unitMap = new Dictionary<string, GameObject>();

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
            GameObject newUnit = new GameObject();
            SpriteRenderer spriteRenderer = newUnit.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetManager.GetInstance().GetPreloadedSprite(assetID);
            spriteRenderer.sortingOrder = 5;

            newUnit.name = id;

            newUnit.transform.position = MapCoordinateUtils.GetTileToWorldPosition(0, 0);

            unitMap[newUnit.name] = newUnit;

            return newUnit;
        }
    }
}