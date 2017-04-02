/// ---------------------------------------------------------------------------
/// VisionController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>April 1st, 2017</date>
/// ---------------------------------------------------------------------------

using core.units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.vision
{
    /// <summary>
    /// This approach to line of sight vision checking casts rays to
    /// all of the edge tiles and lights up tiles along the bresenham
    /// line until encountering a vision blocking tile. 
    /// </summary>
    public class EdgeTileCheck
    {
        public static void Process(MapData currentMap, bool drawDebugLines)
        {
            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            // Iterate through all of the player units and check their vision
            UnitController uc = UnitController.GetInstance();
            List<GameObject> playerUnits = uc.GetAllPlayerUnits();
            for (int i = 0, count = playerUnits.Count; i < count; i++)
            {
                GameUnitComponent guc = playerUnits[i].GetComponent<GameUnitComponent>();

                Vector2 shipCoord = guc.CurrentTilePos;

                // Do the ray cast to each of the border tiles
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector2 upperPos = new Vector2(x, 0);
                    DrawBresenhamLine(shipCoord, upperPos, drawDebugLines);

                    Vector2 lowerPos = new Vector2(x, mapHeight - 1);
                    DrawBresenhamLine(shipCoord, lowerPos, drawDebugLines);
                }

                for (int y = 0; y < mapHeight; y++)
                {
                    Vector2 leftPos = new Vector2(0, y);
                    DrawBresenhamLine(shipCoord, leftPos, drawDebugLines);

                    Vector2 rightPos = new Vector2(mapWidth - 1, y);
                    DrawBresenhamLine(shipCoord, rightPos, drawDebugLines);
                }
            }
        }

        /// <summary>
        /// Draws the bresenham line and updates the light map data along the path.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="drawDebugLines"></param>
        public static void DrawBresenhamLine(Vector2 startPos, Vector2 endPos, bool drawDebugLines)
        {
            float dX = endPos.x - startPos.x;
            float dY = endPos.y - startPos.y;

            int directionX = dX > 0 ? 1 : -1;
            int directionY = dY > 0 ? 1 : -1;

            // Is it more of a flat slope?
            bool checkHoriz = Math.Abs(dX) >= Math.Abs(dY);

            float slope;
            float pitch;

            int[,] lightMap = VisionController.GetInstance().GetLightMap();

            // We have a flatter slope so let's traverse across the X-axis
            if (checkHoriz)
            {
                slope = dY / dX;
                pitch = startPos.y - (slope * startPos.x);

                int x = Mathf.RoundToInt(startPos.x);
                int y = Mathf.RoundToInt(startPos.y);

                int count = Mathf.RoundToInt(Math.Abs(dX));

                // Go through each step along the X-Axis to see if we run into a
                // a blocking tile.
                for (int i = 0; i <= count; i++)
                {
                    y = Mathf.RoundToInt((x * slope) + pitch);

                    // If we run into a blocking tile we're done no longer do we
                    // need to continue checking the ray
                    if (MapController.GetInstance().IsTileBlockingVision(x, y))
                    {
                        if (x >= 0 && x < lightMap.GetLength(0) &&
                            y >= 0 && y < lightMap.GetLength(1))
                        {
                            lightMap[x, y] = 1;
                        }
                        break;
                    }

                    lightMap[x, y] = 1;

                    // Don't add anything at the last check since we don't want to
                    // overshoot our debug lines
                    if (i < count)
                    {
                        x += directionX;
                    }
                }

                if (drawDebugLines)
                {
                    VisionController.DrawLine(startPos, new Vector2(x, y));
                }
            }
            // A steeper sloper so we should traverse vertically for better accuracy
            else
            {
                slope = dX / dY;
                pitch = startPos.x - (slope * startPos.y);

                int y = Mathf.RoundToInt(startPos.y);
                int x = Mathf.RoundToInt(startPos.x);
                int count = Mathf.RoundToInt(Math.Abs(dY));
                // Go through each step along the Y-Axis to see if we run into a
                // blocking tile. 
                for (int i = 0; i <= count; i++)
                {
                    x = Mathf.RoundToInt((y * slope) + pitch);

                    // If we run into a blocking tile we're done no longer do we
                    // need to continue checking the ray
                    if (MapController.GetInstance().IsTileBlockingVision(x, y))
                    {
                        if (x >= 0 && x < lightMap.GetLength(0) &&
                            y >= 0 && y < lightMap.GetLength(1))
                        {
                            lightMap[x, y] = 1;
                        }
                        break;
                    }

                    lightMap[x, y] = 1;

                    // Don't add anything at the last check since we don't want to
                    // overshoot our debug lines
                    if (i < count)
                    {
                        y += directionY;
                    }
                }

                if (drawDebugLines)
                {
                    VisionController.DrawLine(startPos, new Vector2(x, y));
                }
            }
        }
    }
}