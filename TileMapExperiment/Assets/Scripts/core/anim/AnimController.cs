﻿
using core.tilesys;
using core.tilesys.pathing;
using core.tilesys.vision;
using core.units;
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
        private const float SPEED = 16.5f;

        /// <summary>
        /// Determines if we should allow vision to refresh each frame or
        /// once the unit arrives at it's destination.
        /// </summary>
        private bool REFRESH_VISION_EACH_FRAME = false;

        /// <summary>
        /// The highlander of this class. 
        /// </summary>
        private static AnimController instance;

        private List<GameObject> animatingObjects;

        private List<GameObject> finishedAnims;

        private AnimController()
        {
            animatingObjects = new List<GameObject>();

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
            bool isVisionDirty = false;

            for (int i = 0, count = animatingObjects.Count; i < count; i++)
            {
                GameObject unit = animatingObjects[i];
                GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();

                // If we don't have any destinations to animate towards then add
                // to the finishedAnims list to remove later.
                if (!guc.HasPendingDestinations())
                {
                    finishedAnims.Add(unit);
                    continue;
                }

                // Get the next destination we're supposed to animate towards
                // First in first out.
                Vector3 endPostion = guc.GetCurrentDestination();

                // At this point we've made it so let's remove this destination
                // from the list of place we need to move this one.
                if (unit.transform.position == endPostion)
                {
                    guc.RemoveCurrentDestination();
                    continue;
                }

                // Lerp it to the location
                unit.transform.position = Vector3.LerpUnclamped(
                    unit.transform.position, endPostion, SPEED * Time.deltaTime);

                isVisionDirty = true;
            }

            // After we're done animating them we remove them from the list of
            // currently animating. 
            for (int i = 0, count = finishedAnims.Count; i < count; i++)
            {
                animatingObjects.Remove(finishedAnims[i]);

                Debug.Log("Done Animating: " + finishedAnims[i]);

                if (animatingObjects.Count < 1)
                {
                    Debug.Log("No Animations Pending");
                }

                // After everything is done animating we try a force a full refresh
                // of the occupied map
                MapController.GetInstance().ForceRefreshOccupiedMap();

                isVisionDirty = true;
            }

            // Clear out the finished Anim keys since we've removed all of the 
            // things at this point
            finishedAnims.Clear();

            if (isVisionDirty)
            {
                // If we want to refresh each frame while dirty then update
                //
                // otherwise only refresh the vision map once we're done animating
                if (REFRESH_VISION_EACH_FRAME || animatingObjects.Count == 0)
                {
                    VisionController.GetInstance().UpdateVisionTiles();
                }
            }
        }

        public void ForceMoveUnitToTile(GameObject unit, Vector2 endTile)
        {
            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();
            guc.ClearPendingDestinations();

            // Get the path from the pathing controller
            Vector2 startPos = guc.CurrentTilePos;

            PathingController pc = MapController.GetInstance().pathingController;

            // Get the full path
            List<PathGraphNode> path = pc.search.Calculate(startPos, endTile);

            // get the directional changes from the path.
            for (int i = path.Count - 1; i >= 0; i--)
            {
                PathGraphNode prev = null;
                PathGraphNode next = null;
                PathGraphNode curr = path[i];

                // Always set the final one as a destination
                if (i == 0)
                {
                    Vector2 endPos = MapCoordinateUtils.GetTileToWorldPosition(endTile);

                    // Apply tile size off set.
                    endPos.x = endPos.x + guc.tileOffsetX;
                    endPos.y = endPos.y + guc.tileOffsetY;
                    
                    guc.QueueDestination(endPos);
                    continue;
                }

                if (i < path.Count - 1)
                {
                    prev = path[i + 1];
                }

                if (i > 0)
                {
                    next = path[i - 1];
                }

                // Only if we have a turn do we need to add a destination.
                if (pc.IsPathNodeATurn(prev, curr, next))
                {
                    Vector2 turn = MapCoordinateUtils.GetTileToWorldPosition(curr.x, curr.y);
                    turn.x += guc.tileOffsetX;
                    turn.y +=guc.tileOffsetY;

                    guc.QueueDestination(turn);
                }
            }

            if (!animatingObjects.Contains(unit))
            {
                animatingObjects.Add(unit);
            }
        }

        public void QueueMoveUnitToTile(GameObject unit, Vector2 tilePos)
        {
            Vector2 endPos = MapCoordinateUtils.GetTileToWorldPosition(tilePos);
            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();

            // Apply tile size off set.
            endPos.x = endPos.x + guc.tileOffsetX;
            endPos.y = endPos.y + guc.tileOffsetY;

            guc.QueueDestination(endPos);

            if (!animatingObjects.Contains(unit))
            {
                animatingObjects.Add(unit);
            }
        }

    }
}

