
using core.tilesys;
/// ---------------------------------------------------------------------------
/// AnimController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 13th, 2017</date>
/// ---------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace core.anim
{
    /// <summary>
    /// Handles the queueing and playing of movement animations. 
    /// </summary>
    public class AnimController
    {
        private const float SPEED = 8.5f;

        /// <summary>
        /// The highlander of this class. 
        /// </summary>
        private static AnimController instance;

        /// <summary>
        /// A map of game objects to their animation destinations
        /// </summary>
        private Dictionary<GameObject, List<Vector3>> pendingAnimations;

        private List<GameObject> finishedAnims;

        private AnimController()
        {
            pendingAnimations = new Dictionary<GameObject, List<Vector3>>();
            finishedAnims = new List<GameObject>();
        }

        /// <summary>
        /// Returns the highlander for AnimController.
        /// </summary>
        /// <returns></returns>
        public static AnimController GetInstance()
        {
            if (instance == null)
            {
                instance = new AnimController();
            }

            return instance;
        }

        /// <summary>
        /// Updates every frame and progresses the animations that are
        /// currently in progress or queued up. 
        /// </summary>
        public void Update()
        {
            foreach (KeyValuePair<GameObject, List<Vector3>> entry in pendingAnimations)
            {
                GameObject unit = entry.Key;
                List <Vector3> destinations = entry.Value;

                // If we don't have any destinations to animate towards then add
                // to the finishedAnims list to remove later.
                if (destinations == null || destinations.Count == 0)
                {
                    finishedAnims.Add(unit);
                    continue;
                }

                // Get the next destination we're supposed to animate towards
                // First in first out.
                Vector3 endPostion = destinations[0];

                // At this point we've made it so let's remove this destination
                // from the list of place we need to move this one.
                if (unit.transform.position == endPostion)
                {
                    destinations.RemoveAt(0);
                    continue;
                }

                // Lerp it to the location
                unit.transform.position = Vector3.LerpUnclamped(
                    unit.transform.position, endPostion, SPEED * Time.deltaTime);
            }

            // After we're done animating them we remove them from the list of
            // currently animating. 
            for (int i = 0, count = finishedAnims.Count; i < count; i++)
            {
                pendingAnimations.Remove(finishedAnims[i]);

                Debug.Log("Done Animating: " + finishedAnims[i]);

                if (pendingAnimations.Count < 1)
                {
                    Debug.Log("No Animations Pending");
                }
            }

            // Clear out the finished Anim keys since we've removed all of the 
            // things at this point
            finishedAnims.Clear();
        }

        public void ForceMoveUnitToTile(GameObject unit, Vector2 tilePos)
        {
            Vector2 endPos = MapCoordinateUtils.GetTileToWorldPosition(tilePos);
            List<Vector3> destinations;

            // if we don't have something already in there we'll make it.
            if (!pendingAnimations.ContainsKey(unit))
            {
                pendingAnimations[unit] = new List<Vector3>();
            }

            destinations = pendingAnimations[unit];
            destinations.Clear();

            destinations.Add(endPos);
        }

        public void QueueMoveUnitToTile(GameObject unit, Vector2 tilePos)
        {
            Vector2 endPos = MapCoordinateUtils.GetTileToWorldPosition(tilePos);
            List<Vector3> destinations;

            // if we don't have something already in there we'll make it.
            if (!pendingAnimations.ContainsKey(unit))
            {
                pendingAnimations[unit] = new List<Vector3>();
            }

            destinations = pendingAnimations[unit];

            destinations.Add(endPos);
        }
    }
}

