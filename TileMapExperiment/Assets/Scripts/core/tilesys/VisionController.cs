/// ---------------------------------------------------------------------------
/// VisionController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 23rd, 2017</date>
/// ---------------------------------------------------------------------------

using core.units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys
{
    public class VisionController
    {
        private const float DURATION = 5f;

        private static VisionController instance;

        private int[,] lightMap;

        private VisionController()
        {

        }

        public static VisionController GetInstance()
        {
            if (instance == null)
            {
                instance = new VisionController();
            }

            return instance;
        }

        public void DebugVisionTiles()
        {
            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            // Only create the light map if needed
            if (lightMap == null)
            {
                lightMap = new int[mapWidth, mapHeight];
            }

            // Get the main ship
            GameObject mainShip = UnitController.GetInstance().GetUnitByID("ship");

            Vector2 mainShipTileCoord = mainShip.GetComponent<GameUnitComponent>().CurrentTilePos;

            Vector2 startPos = new Vector2(mainShip.transform.position.x, mainShip.transform.position.y);
            Vector2 endPos = new Vector2();

            // Get all the vision blockers so we don't have to check everything.
            List<GameObject> blockers = UnitController.GetInstance().GetAllVisionBlockingUnits();

            
            // Do the ray cast to each of the border tiles
            for (int x = 0; x < mapWidth; x++)
            {
                DrawBresenhamLine(mainShipTileCoord, new Vector2(x, 0));
                DrawBresenhamLine(mainShipTileCoord, new Vector2(x, mapHeight-1));
            }

            for (int y = 0; y < mapHeight; y++)
            {
                DrawBresenhamLine(mainShipTileCoord, new Vector2(0, y));
                DrawBresenhamLine(mainShipTileCoord, new Vector2(mapWidth-1, y));
            }
        }

        private void DrawBresenhamLine(Vector2 startPos, Vector2 endPos)
        {
            float dX = endPos.x - startPos.x;
            float dY = endPos.y - startPos.y;

            int directionX = dX > 0 ? 1 : -1;
            int directionY = dY > 0 ? 1 : -1;

            // Is it more of a flat slope?
            bool checkHoriz = Math.Abs(dX) >= Math.Abs(dY);

            float slope;
            float pitch;

            // We have a flatter slope so let's traverse across the X-axis
            if (checkHoriz)
            {
                slope = dY / dX;
                pitch = startPos.y - (slope * startPos.x);

                int x = (int)startPos.x;

                // Go through each step along the X-Axis to see if we run into a
                // a blocking tile.
                for (int i= 0; i <= Math.Abs(dX); i++)
                {
                    int y = Mathf.RoundToInt((x * slope) + pitch);

                    // If we run into a blocking tile we're done no longer do we
                    // need to continue checking the ray
                    if (MapController.GetInstance().IsTileBlockingVision(x, y))
                    {
                        break;
                    }

                    DrawMarkOnTile(x, y);
                    x += directionX;
                }
            }
            // A steeper sloper so we should traverse vertically for better accuracy
            else
            {
                slope = dX/ dY;
                pitch = startPos.x - (slope * startPos.y);

                int y = (int)startPos.y;

                // Go through each step along the Y-Axis to see if we run into a
                // blocking tile. 
                for (int i = 0; i <= Math.Abs(dY); i++)
                {
                    int x = Mathf.RoundToInt((y * slope) + pitch);

                    // If we run into a blocking tile we're done no longer do we
                    // need to continue checking the ray
                    if (MapController.GetInstance().IsTileBlockingVision(x, y))
                    {
                        break;
                    }

                    DrawMarkOnTile(x, y);
                    y += directionY;
                }
            }
        }

        private void DrawMarkOnTile(int x, int y)
        {
            Vector2 startPos = new Vector2();
            Vector2 endPos = new Vector2();

            /// make the \
            startPos.x = x;
            startPos.y = -y;
            endPos.x = x + 1f;
            endPos.y = -y - 1f;

            Debug.DrawLine(startPos, endPos, Color.yellow, DURATION, false);

            // make the /
            startPos.x = x + 1f;
            startPos.y = -y;
            endPos.x = x;
            endPos.y = -y - 1f;

            Debug.DrawLine(startPos, endPos, Color.yellow, DURATION, false);
        }
    }
}