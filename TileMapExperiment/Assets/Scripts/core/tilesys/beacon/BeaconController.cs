/// ---------------------------------------------------------------------------
/// BeaconController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>May 9th, 2017</date>
/// ---------------------------------------------------------------------------

using core.tilesys.vision;
using core.units;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.beacon
{
    /// <summary>
    /// The controller to handle the placement and refreshing of the beacons.
    /// </summary>
    public class BeaconController
    {
        private static BeaconController instance;

        private BeaconObjectPool pool;

        private BeaconController()
        {
        }

        public static BeaconController GetInstance()
        {
            if (instance == null)
            {
                instance = new BeaconController();
            }

            return instance;
        }

        public void PopulatePool()
        {
            pool = new BeaconObjectPool();
        }

        /// <summary>
        /// Returns all beacons back to the pool and then places them at the new non-visible
        /// enemy unit positions.
        /// </summary>
        public void RefreshBeacons()
        {
            pool.ClearAllBeacons();

            List<GameObject> enemies = UnitController.GetInstance().GetAllEnemyUnits();

            VisionController vc = VisionController.GetInstance();

            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                GameObject unit = enemies[i];
                GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();

                Vector2 currTilePos = guc.CurrentTilePos;

                int x = (int)currTilePos.x;
                int y = (int)currTilePos.y;

                // If visible then bounce out because the beacon is only for the void
                if (vc.isTileVisible(x, y))
                {
                    continue;
                }

                pool.PlaceBeacon(x, y);
            }

            enemies.Clear();
            enemies = null;
        }
    }
}