﻿/// ---------------------------------------------------------------------------
/// PathingController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>April 13th, 2017</date>
/// ---------------------------------------------------------------------------

using core.units;
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

        public PathingController()
        {

        }

        /// <summary>
        /// Plots out the path using debug lines
        /// </summary>
        public void PlotPath(GameObject unit, Vector2 tilePos)
        {
            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();

            Vector2 currPos = guc.CurrentTilePos;

            int xDelta = Mathf.RoundToInt(tilePos.x - currPos.x);
            int yDelta = Mathf.RoundToInt(tilePos.y - currPos.y);

            int xIncre = xDelta > 0 ? 1 : -1;
            int yIncre = yDelta > 0 ? 1 : -1;

            int x = (int)currPos.x;
            int y = (int)currPos.y;

            // mark the mid point
            DrawMarkOnTile(x, y + yDelta, Color.cyan);

            // draw the destination
            DrawMarkOnTile(x + xDelta, y + yDelta, Color.cyan);

            // draw the vertical
            for (int i = 0;  i != yDelta; i += yIncre)
            {
                DrawMarkOnTile(x, y + i, Color.yellow);
            }

            for (int i = xIncre; i != xDelta; i += xIncre)
            {
                DrawMarkOnTile(x + i, y + yDelta, Color.yellow);
            }
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