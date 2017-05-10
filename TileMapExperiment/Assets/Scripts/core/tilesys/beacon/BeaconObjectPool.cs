/// ---------------------------------------------------------------------------
/// BeaconObjectPool.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>May 9th, 2017</date>
/// ---------------------------------------------------------------------------

using core.assets;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.beacon
{
    /// <summary>
    /// Handles the object pool of beacons
    /// </summary>
    public class BeaconObjectPool
    {
        private const int POOL_SIZE = 10;
        private const string PARENT_NAME = "BeaconContainer";

        /// <summary>
        /// Container game object purely there for scene organization
        /// </summary>
        GameObject parentObj;

        /// <summary>
        /// The pool of inactive beacons.
        /// </summary>
        private Queue<GameObject> pool;

        /// <summary>
        /// The list of active beacons. We keep track of this for ease of cleanup
        /// and if we need to reference them later.
        /// </summary>
        private List<GameObject> activeBeacons;

        public BeaconObjectPool()
        {
            activeBeacons = new List<GameObject>();

            // Make the parent object. Used mainly for keeping the scene organized
            parentObj = new GameObject();
            parentObj.name = PARENT_NAME;

            // Pre-populate the pool
            PopulatePool();
        }

        private void PopulatePool()
        {
            if (pool == null)
            {
                pool = new Queue<GameObject>();
            }

            pool.Clear();

            for (int i = 0; i < POOL_SIZE; i++)
            {
                GameObject gameObj = GetNewBeacon();

                pool.Enqueue(gameObj);
            }
        }

        /// <summary>
        /// Pull a beacon off the pool if possible otherwise make a new beacon
        /// and place it at the given tile coordinates.
        /// </summary>
        public void PlaceBeacon(int x, int y)
        {
            GameObject beacon;

            // Only make a new one if we've hit the limit. Keep this elastic
            if (pool.Count == 0)
            {
                beacon = GetNewBeacon();
            }
            else
            {
                beacon = pool.Dequeue();
            }

            beacon.SetActive(true);
            beacon.transform.position = MapCoordinateUtils.GetTileToWorldPosition(new Vector2(x, y));

            activeBeacons.Add(beacon);
        }

        /// <summary>
        /// Removes all active beacons from the world and returns them to the queue.
        /// </summary>
        public void ClearAllBeacons()
        {
            for (int i = 0, count = activeBeacons.Count; i < count; i++)
            { 
                GameObject beacon = activeBeacons[i];
                beacon.SetActive(false);
                pool.Enqueue(beacon);
            }

            activeBeacons.Clear();
        }

        /// <summary>
        /// Instantiates a new beacon but defaults it to inactive
        /// </summary>
        /// <returns></returns>
        private GameObject GetNewBeacon()
        {
            GameObject gameObj = AssetManager.GetInstance().InstantiatePreloadedPrefab(AssetConstants.PF_BEACON);
            gameObj.name = "Beacon";
            gameObj.transform.parent = parentObj.transform;

            gameObj.SetActive(false);
            return gameObj;
        }
    }
}