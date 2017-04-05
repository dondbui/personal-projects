
using core.units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.vision
{
    public class ShadowCastingAlgorithm
    {
        public static void Process(MapData currentMap, bool drawDebugLines)
        {
            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            // Iterate through all of the player units and check their vision
            UnitController uc = UnitController.GetInstance();
            //List<GameObject> playerUnits = uc.GetAllPlayerUnits();
            //for (int i = 0, count = playerUnits.Count; i < count; i++)
            //{

            //}

            GameObject gob = uc.GetUnitByID("player0");
            GameUnitComponent pGuc = gob.GetComponent<GameUnitComponent>();

            Vector2 startPos = pGuc.CurrentTilePos;

            List<GameObject> visionBlockers = uc.GetAllVisionBlockingUnits();
            Vector2 endPos = new Vector2();
            for (int i = 0, count = visionBlockers.Count; i < count; i++)
            {
                GameObject vb = visionBlockers[i];
                GameUnitComponent guc = vb.GetComponent<GameUnitComponent>();

                Rect corners = guc.GetCorners();

                // Raycast to the top left (5,5)
                endPos.x = corners.x;
                endPos.y = corners.y;
                Debug.Log("Upper Left: " + endPos.ToString());
                DrawBresenhamLine(startPos, endPos, drawDebugLines);

                // Upper right (7, 5)
                endPos.x = corners.xMax;
                endPos.y = corners.yMin;
                Debug.Log("Upper Right: " + endPos.ToString());
                DrawBresenhamLine(startPos, endPos, drawDebugLines);

                //// lower left
                endPos.x = corners.x;
                endPos.y = corners.yMax;
                Debug.Log("Lower Left: " + endPos.ToString());
                DrawBresenhamLine(startPos, endPos, drawDebugLines);

                //// lower right
                endPos.x = corners.xMax;
                endPos.y = corners.yMax;
                Debug.Log("Lower Right: " + endPos.ToString());
                DrawBresenhamLine(startPos, endPos, drawDebugLines);
            }

            // Draw lines to the four corners
            endPos.x = 0;
            endPos.y = 0;
            DrawBresenhamLine(startPos, endPos, drawDebugLines);

            endPos.x = mapWidth - 1;
            endPos.y = 0;
            DrawBresenhamLine(startPos, endPos, drawDebugLines);

            endPos.x = mapWidth - 1;
            endPos.y = mapHeight - 1;
            DrawBresenhamLine(startPos, endPos, drawDebugLines);

            endPos.x = 0;
            endPos.y = mapHeight - 1;
            DrawBresenhamLine(startPos, endPos, drawDebugLines);
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

            float increment = 1f;

            float directionX = dX > 0 ? increment : -increment;
            float directionY = dY > 0 ? increment : -increment;

            // Is it more of a flat slope?
            bool checkHoriz = Math.Abs(dX) >= Math.Abs(dY);

            float slope;
            float pitch;

            int[,] lightMap = VisionController.GetInstance().GetLightMap();

            int width = lightMap.GetLength(0);
            int height = lightMap.GetLength(1);

            Debug.Log("Check Horiz: " + checkHoriz);

            Vector2 debugEndPoint = new Vector2(startPos.x, startPos.y);

            bool hitSomething = false;

            // We have a flatter slope so let's traverse across the X-axis
            if (checkHoriz)
            {
                slope = dY / dX;
                pitch = startPos.y - (slope * startPos.x);

                float x = startPos.x;
                float y = startPos.y;

                int count = Mathf.RoundToInt(Math.Abs(dX));

                int tileX = 0;
                int tileY = 0;

                // Go through each step along the X-Axis to see if we run into a
                // a blocking tile.
                while(x >= 0 && x <= width)
                {
                    y = (x * slope) + pitch;

                    tileX = Mathf.RoundToInt(x);
                    tileY = Mathf.RoundToInt(y);

                    // If out of bounds bounce out
                    if (tileX < 0 || tileX >= width ||
                        tileY < 0 || tileY >= height)
                    {
                        break;
                    }

                    if (!hitSomething)
                    {
                        debugEndPoint.x = x;
                        debugEndPoint.y = -y;
                    }

                    if (x == endPos.x && y == endPos.y)
                    {
                        Debug.Log("Hit corner: " + endPos.ToString());
                    }

                    // If we run into a blocking tile we're done no longer do we
                    // need to continue checking the ray
                    if (!hitSomething && !MapController.GetInstance().IsTileBlockingVision(tileX, tileY))
                    {
                        lightMap[tileX, tileY] = 1;
                    }
                    else
                    {
                        lightMap[tileX, tileY] = 0;

                        if (!hitSomething)
                        {
                            lightMap[tileX, tileY] = 1;
                        }

                        hitSomething = true;
                    }

                    // Don't add anything at the last check since we don't want to
                    // overshoot our debug lines
                    x += directionX;
                }

                if (drawDebugLines)
                {
                    DrawLine(startPos, new Vector2(x, -y));

                    if (hitSomething)
                    {
                        DrawLine(startPos, debugEndPoint);
                    }
                    else
                    {
                        DrawLine(startPos, new Vector2(endPos.x, -endPos.y));
                    }
                    
                }
            }
            // A steeper sloper so we should traverse vertically for better accuracy
            else
            {
                slope = dX / dY;
                pitch = startPos.x - (slope * startPos.y);

                float x = startPos.x;
                float y = startPos.y;

                int count = Mathf.RoundToInt(Math.Abs(dY));

                int tileX = 0;
                int tileY = 0;

                // Go through each step along the Y-Axis to see if we run into a
                // block tile. WE keep checking until the edge of the map
                while (y >= 0 && y <= height)
                {
                    x = (y * slope) + pitch;

                    tileX = Mathf.RoundToInt(x);
                    tileY = Mathf.RoundToInt(y);

                    // If out of bounds bounce out
                    if (tileX < 0 || tileX >= width ||
                        tileY < 0 || tileY >= height)
                    {
                        break;
                    }

                    if (!hitSomething)
                    {
                        debugEndPoint.x = x;
                        debugEndPoint.y = -y;
                    }

                    // If we run into a blocking tile we're done no longer do we
                    // need to continue checking the ray
                    if (!hitSomething && !MapController.GetInstance().IsTileBlockingVision(tileX, tileY))
                    {
                        lightMap[tileX, tileY] = 1;
                    }
                    else
                    {
                        lightMap[tileX, tileY] = 0;
                        if (!hitSomething)
                        {
                            lightMap[tileX, tileY] = 1;
                        }
                    }

                    y += directionY;
                    
                }

                if (drawDebugLines)
                {
                    DrawLine(startPos, new Vector2(x, -(y)));

                    if (hitSomething)
                    {
                        DrawLine(startPos, debugEndPoint);
                    }
                    else
                    {
                        DrawLine(startPos, new Vector2(endPos.x, -endPos.y));
                    }
                }
            }
        }

        private static void DrawLine(Vector2 startPos, Vector2 endPos)
        {
            Vector2 worldStart = MapCoordinateUtils.GetTileToWorldPosition(startPos);

            Debug.Log("Drawing from: " + startPos.ToString() + " ----> " + endPos.ToString());

            Debug.DrawLine(worldStart, endPos, Color.yellow, 3f, false);

            DrawX(endPos);
        }

        private static void DrawX(Vector2 pos)
        {
            Vector2 startPos = new Vector2();
            Vector2 endPos = new Vector2();

            float offset = 0.1f;

            /// make the \
            startPos.x = pos.x - offset;
            startPos.y = (pos.y - offset);
            endPos.x = pos.x + offset;
            endPos.y = (pos.y + offset);

            Debug.DrawLine(startPos, endPos, Color.yellow, 3f, false);

            // make the /
            startPos.x = pos.x + offset;
            startPos.y = (pos.y - offset);
            endPos.x = pos.x - offset;
            endPos.y = (pos.y + offset);

            Debug.DrawLine(startPos, endPos, Color.yellow, 3f, false);
        }
    }
}