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
        private const float DURATION = 1f;

        private static VisionController instance;

        private int[,] lightMap;

        private Texture2D lightTexture;

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

            if (currentMap == null)
            {
                return;
            }

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            // Only create the light map if needed
            if (lightMap == null)
            {
                lightMap = new int[mapWidth, mapHeight];
            }

            if (lightTexture == null)
            {
                lightTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32, false);
                lightTexture.filterMode = FilterMode.Trilinear;
                lightTexture.wrapMode = TextureWrapMode.Clamp;
                                
                GenerateShadowMesh();
            }

            // clear the light map
            ClearLightMap();
            
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
                    DrawBresenhamLine(shipCoord, new Vector2(x, 0));
                    DrawBresenhamLine(shipCoord, new Vector2(x, mapHeight - 1));
                }

                for (int y = 0; y < mapHeight; y++)
                {
                    DrawBresenhamLine(shipCoord, new Vector2(0, y));
                    DrawBresenhamLine(shipCoord, new Vector2(mapWidth - 1, y));
                }
            }

            UpdateLightTexture();
        }

        private void GenerateShadowMesh()
        {
            GameObject mapMesh = new GameObject();
            mapMesh.name = "lightMap";

            mapMesh.transform.rotation = Quaternion.AngleAxis(180, Vector3.right);

            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            int numTriangles = 6;
            int numVerts = 4;

            Vector3[] vertices = new Vector3[numVerts];
            Vector2[] UVArray = new Vector2[numVerts];

            vertices[0] = new Vector3(0, 0, 0); //  top left
            vertices[1] = new Vector3(mapWidth, 0, 0); // top right
            vertices[2] = new Vector3(mapWidth, mapHeight, 0); // bottom right
            vertices[3] = new Vector3(0, mapHeight, 0); // bottom left

            int[] triangles = new int[numTriangles];

            triangles[0] += 0;
            triangles[1] += 1;
            triangles[2] += 2;
            triangles[3] += 0;
            triangles[4] += 2;
            triangles[5] += 3;

            MeshFilter meshFilter = mapMesh.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;

            Material mat = Resources.Load<Material>("Materials/occupied");

            MeshRenderer mr = mapMesh.AddComponent<MeshRenderer>();
            mr.material = mat;
            mr.material.mainTexture = lightTexture;

            mr.sortingOrder = 50;

            // Top left of tile in atlas
            UVArray[0] = new Vector2(0, 0);

            // Top right of tile in atlas
            UVArray[1] = new Vector2(1, 0);

            // Bottom right of tile in atlas
            UVArray[2] = new Vector2(1, 1);

            //Bottom left of tile in atlas
            UVArray[3] = new Vector2(0, 1);

            meshFilter.mesh.uv = UVArray;
        }

        private void ClearLightMap()
        {
            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    lightMap[x, y] = 0;
                }
            }
        }

        private void UpdateLightTexture()
        {
            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int sightValue = lightMap[x, y];

                    if (sightValue > 0)
                    {
                        lightTexture.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        lightTexture.SetPixel(x, y, Color.black);
                    }
                }
            }

            lightTexture.Apply();
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

                    bool isDone = false;

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

                    //DrawMarkOnTile(x, y);
                    lightMap[x, y] = 1;
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
                        if (x >= 0 && x < lightMap.GetLength(0) &&
                            y >= 0 && y < lightMap.GetLength(1))
                        {
                            lightMap[x, y] = 1;
                        }
                        break;
                    }

                    //DrawMarkOnTile(x, y);
                    lightMap[x, y] = 1;

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