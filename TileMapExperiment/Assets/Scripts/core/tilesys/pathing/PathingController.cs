/// ---------------------------------------------------------------------------
/// PathingController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>April 13th, 2017</date>
/// ---------------------------------------------------------------------------

using core.units;
using System;
using UnityEngine;

namespace core.tilesys.pathing
{
    /// <summary>
    /// Handles the logic behind pathing as well as the visualization of the 
    /// movement arrows on the board. 
    /// </summary>
    public class PathingController
    {
        private const float DURATION = 10f;

        private PathArrow arrow;

        public BreadthFirstSearch search;

        private Vector2 lastTilePos = new Vector2();

        public PathingController()
        {
            search = new BreadthFirstSearch();
        }

        public void Update()
        {
            GameObject selectedUnit = UnitSelector.GetInstance().GetCurrentlySelectedUnit();

            if (selectedUnit != null)
            {
                Vector3 mousePos = Input.mousePosition;

                Vector2 tilePos = MapCoordinateUtils.GetTilePosFromClickPos(mousePos);

                int x = Mathf.RoundToInt(tilePos.x);
                int y = Mathf.RoundToInt(tilePos.y);

                int lastX = Mathf.RoundToInt(lastTilePos.x);
                int lastY = Mathf.RoundToInt(lastTilePos.y);

                // If there's a delta then plat the path.
                if (x != lastX || y != lastY)
                {
                    lastTilePos = tilePos;

                    //Debug.Log("replotting: " + x + "," + y + " | " + lastX + ", " + lastY);
                    PlotPath(selectedUnit, tilePos);
                }
            }
        }

        /// <summary>
        /// Plots out the path using debug lines
        /// </summary>
        public void PlotPath(GameObject unit, Vector2 tilePos)
        {
            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();

            Vector2 currPos = guc.CurrentTilePos;

            if (arrow == null)
            {
                arrow = new PathArrow(currPos, tilePos);
            }
            else
            {
                arrow.Update(currPos, tilePos);
            }
        }

        public void ClearArrow()
        {
            if (arrow == null)
            {
                return;
            }

            arrow.Clear();
            
        }

        /// <summary>
        /// Given the current node and it's neighbors in the path list, is this node a turn?
        /// 
        /// This is very useful for queueing up the movement destinations.
        /// </summary>
        public bool IsPathNodeATurn(PathGraphNode prev, PathGraphNode curr, PathGraphNode next)
        {
            if (prev == null)
            {
                return false;
            }

            //Handle the vertical to horizontal transitions
            if (curr.x == prev.x && // Make sure it's vertical
                next.y == curr.y)   // Make sure it's horizontal
            {
                if (next.x != curr.x)
                {
                    return true;
                }

                if (prev.y != curr.y)
                {
                    return true;
                }
            }
                // Handle the horizontal to vertical transitions
            if (curr.y == prev.y &&
                curr.x == next.x)
            {
                if (curr.y != next.y)
                {
                    return true;
                }

                if (prev.x != curr.x)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Draws an X over a given tile coordinate
        /// </summary>
        public void DrawMarkOnTile(int x, int y, Color color)
        {
            Vector2 startPos = new Vector2();
            Vector2 endPos = new Vector2();

            /// make the \
            startPos.x = x;
            startPos.y = y;
            endPos.x = x + 1f;
            endPos.y = y + 1f;

            Debug.DrawLine(startPos, endPos, color, DURATION, false);

            // make the /
            startPos.x = x + 1f;
            startPos.y = y;
            endPos.x = x;
            endPos.y = y + 1f;

            Debug.DrawLine(startPos, endPos, color, DURATION, false);
        }
    }
}