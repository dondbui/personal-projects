/// ---------------------------------------------------------------------------
/// UnitController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 12th, 2017</date>
/// ---------------------------------------------------------------------------

using core.assets;
using System.Collections.Generic;
using UnityEngine;

namespace core
{
    public class UnitController
    {
        private static UnitController instance;

        Dictionary<string, GameObject> unitMap = new Dictionary<string, GameObject>();

        Dictionary<string, Vector3> pendingAnimations = new Dictionary<string, Vector3>();

        List<string> finishedAnimKeys = new List<string>();

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

        public void Update()
        {
            // Go through all the units that may need to be updated.
            foreach (string key in pendingAnimations.Keys)
            {
                // Doesn't have it then we don't care.
                if (!unitMap.ContainsKey(key))
                {
                    finishedAnimKeys.Add(key);
                    continue;
                }

                GameObject unit = unitMap[key];

                Vector3 endPostion = pendingAnimations[key];

                // At this point we've made it so let's remove it from the list
                // for updating. 
                if (unit.transform.position == endPostion)
                {
                    finishedAnimKeys.Add(key);
                    continue;
                }

                unit.transform.position = Vector3.Lerp(unit.transform.position, endPostion, 2 * Time.deltaTime);
            }

            // After we're done animating them we remove them from the list of
            // currently animating. 
            for (int i = 0, count = finishedAnimKeys.Count; i < count; i++)
            {
                pendingAnimations.Remove(finishedAnimKeys[i]);

                Debug.Log("Removing: " + finishedAnimKeys[i]);
            }

            // Clear out the finished Anim keys since we've removed all of the 
            // things at this point
            finishedAnimKeys.Clear();
        }

        public GameObject PlaceNewUnit(string id, string assetID)
        {
            GameObject newUnit = new GameObject();
            SpriteRenderer spriteRenderer = newUnit.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetManager.GetInstance().GetPreloadedSprite(assetID);

            newUnit.name = id;

            newUnit.transform.position = GetTileToWorldPosition(0, 3);

            unitMap[newUnit.name] = newUnit;

            return newUnit;
        }

        public void MoveUnitToTile(string unitID, int x, int y)
        {
            Vector3 endPos = GetTileToWorldPosition(x, y);
            pendingAnimations[unitID] = endPos;
        }

        private Vector3 GetTileToWorldPosition(int x, int y)
        {
            Vector3 newPos = new Vector3();

            newPos.x = x + 0.5f;
            newPos.y = -(y + 0.5f); 

            return newPos;
        }
    }
}