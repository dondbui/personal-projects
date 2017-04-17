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
        private const float DURATION = 2f;

        private PathArrow arrow;

        public PathingController()
        {

        }

        public void Update()
        {
            GameObject selectedUnit = UnitSelector.GetInstance().GetCurrentlySelectedUnit();

            if (selectedUnit != null)
            {
                Vector3 mousePos = Input.mousePosition;

                Vector2 tilePos = MapCoordinateUtils.GetTilePosFromClickPos(mousePos);

                PlotPath(selectedUnit, tilePos);
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
        /// Draws an X over a given tile coordinate
        /// </summary>
        public void DrawMarkOnTile(int x, int y, Color color)
        {
            Vector2 startPos = new Vector2();
            Vector2 endPos = new Vector2();

            /// make the \
            startPos.x = x;
            startPos.y = -y;
            endPos.x = x + 1f;
            endPos.y = -y - 1f;

            Debug.DrawLine(startPos, endPos, color, DURATION, false);

            // make the /
            startPos.x = x + 1f;
            startPos.y = -y;
            endPos.x = x;
            endPos.y = -y - 1f;

            Debug.DrawLine(startPos, endPos, color, DURATION, false);
        }
    }
}