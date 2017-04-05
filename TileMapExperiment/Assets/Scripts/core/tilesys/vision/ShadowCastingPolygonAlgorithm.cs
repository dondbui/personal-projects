
using core.units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.vision
{
    public class ShadowCastingPolygonAlgorithm
    {
        public static void Process(MapData currentMap, bool drawDebugLines)
        {
            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            UnitController uc = UnitController.GetInstance();

            GameObject gob = uc.GetUnitByID("player0");
            GameUnitComponent pGuc = gob.GetComponent<GameUnitComponent>();

            Vector2 startPos = gob.transform.position;
            Vector2 endPos = new Vector2();

            List<GameObject> visionBlockers = uc.GetAllVisionBlockingUnits();

            for (int i = 0, count = visionBlockers.Count; i < count; i++)
            {
                GameObject vb = visionBlockers[i];
                GameUnitComponent guc = vb.GetComponent<GameUnitComponent>();

                Rect corners = guc.GetCorners();

                // Raycast to the top left (5,5)
                endPos.x = corners.x;
                endPos.y = -corners.y;
                DrawRayEdge(vb, startPos, endPos);

                // Upper right (7, 5)
                endPos.x = corners.xMax;
                endPos.y = -corners.yMin;
                DrawRayEdge(vb, startPos, endPos); ;

                // lower left
                endPos.x = corners.x;
                endPos.y = -corners.yMax;
                DrawRayEdge(vb, startPos, endPos);

                // lower right
                endPos.x = corners.xMax;
                endPos.y = -corners.yMax;
                DrawRayEdge(vb, startPos, endPos);
            }

            

        }

        private static void DrawRayEdge(GameObject blocker, Vector2 startPos, Vector2 cornerPos)
        {
            GameUnitComponent guc = blocker.GetComponent<GameUnitComponent>();

            Rect corners = guc.GetCorners();

            float dX = cornerPos.x - startPos.x;
            float dY = cornerPos.y - startPos.y;

            int directionX = dX > 0 ? 1 : -1;
            int directionY = dY > 0 ? 1 : -1;

            bool isHoriz = Math.Abs(dX) >= Math.Abs(dY);
            bool isDirPosi = false;

            float slope = 0;
            float pitch = 0;

            if (isHoriz)
            {
                slope = dY / dX;
                pitch = startPos.y - (slope * startPos.x);

                isDirPosi = directionX > 0;
            }
            else
            {
                slope = dX / dY;
                pitch = startPos.x - (slope * startPos.y);

                isDirPosi = directionY < 0;
            }

            DrawLine(startPos, cornerPos);

            Vector2 interPoint = GetMapEdgeIntersection(isHoriz, isDirPosi, slope, pitch);

            DrawLine(startPos, interPoint);
        }

        private static Vector2 GetMapEdgeIntersection(bool isHoriz, bool isDirPosi, float slope, float pitch)
        {
            Vector2 intPoint = new Vector2();

            int xEdge = 0;
            int yEdge = 0;

            if (isDirPosi)
            {
                xEdge = 32;
                yEdge = -32;
            }


            if (isHoriz)
            {
                intPoint.x = xEdge;
                intPoint.y = (slope * intPoint.x) + pitch;
            }
            else
            {
                intPoint.y = yEdge;
                intPoint.x = (slope * intPoint.y) + pitch;
            }

            return intPoint;
        }

        private static void DrawLine(Vector2 startPos, Vector2 endPos)
        {
            //Vector2 worldStart = MapCoordinateUtils.GetTileToWorldPosition(startPos);

            Debug.Log("Drawing from: " + startPos.ToString() + " ----> " + endPos.ToString());

            Debug.DrawLine(startPos, endPos, Color.yellow, 3f, false);

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