﻿/// ---------------------------------------------------------------------------
/// VisionController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 23rd, 2017</date>
/// ---------------------------------------------------------------------------

using core.assets;
using core.tilesys.beacon;
using core.units;
using System;
using UnityEngine;

namespace core.tilesys.vision
{
    public class VisionController
    {
        public const int VISIBLE = 1;
        public const int NOT_VISIBLE = 0;

        private const float DURATION = 3f;

        private static VisionController instance;

        private int[,] lightMap;

        private GameObject mapMesh;

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

        public void UpdateVisionTiles()
        {
            UpdateVisionTiles(false);
            BeaconController.GetInstance().RefreshBeacons();
        }

        public void UpdateVisionTiles(bool drawDebugLines)
        {
            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            // If the current map doesn't exist or hasn't been created yet. 
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

            // Run the algorithm for updating the light map array.
            //EdgeTileRaycastAlgorithm.Process(currentMap, drawDebugLines);
            ShadowCastingAlgorithm.Process(currentMap, drawDebugLines);

            // Now based off of the light map array data we update
            // the texture used to visualize the light map. 
            UpdateLightTexture();
        }

        public int[,] GetLightMap()
        {
            return lightMap;
        }

        public bool isTileVisible(int x, int y)
        {
            return lightMap[x, y] == VISIBLE;
        }

        private void GenerateShadowMesh()
        {
            mapMesh = new GameObject();
            mapMesh.name = "lightMap";

            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            int numTriangles = 6;
            int numVerts = 4;

            Vector3[] vertices = new Vector3[numVerts];
            Vector2[] UVArray = new Vector2[numVerts];


            vertices[0] = new Vector3(0, 0, 0); //  top left
            vertices[1] = new Vector3(0, mapHeight, 0); // bottom left
            vertices[2] = new Vector3(mapWidth, mapHeight, 0); // bottom right
            vertices[3] = new Vector3(mapWidth, 0, 0); // top right

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

            Material mat = Resources.Load<Material>(AssetConstants.MAT_SELECTED_TILE);

            MeshRenderer mr = mapMesh.AddComponent<MeshRenderer>();
            mr.material = mat;
            mr.material.mainTexture = lightTexture;

            mr.sortingOrder = 50;

            // Top left of tile in atlas
            UVArray[0] = new Vector2(0, 0);

            //Bottom left of tile in atlas
            UVArray[1] = new Vector2(0, 1);

            // Bottom right of tile in atlas
            UVArray[2] = new Vector2(1, 1);

            // Top right of tile in atlas
            UVArray[3] = new Vector2(1, 0);

            meshFilter.mesh.uv = UVArray;
        }

        /// <summary>
        /// Clears the light map data back to all darkness
        /// </summary>
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
                    lightMap[x, y] = NOT_VISIBLE;
                    //lightMap[x, y] = VISIBLE;
                }
            }
        }

        /// <summary>
        /// Updates the light map texture based off of the lightmap
        /// which contains which tiles are visible.
        /// </summary>
        private void UpdateLightTexture()
        {
            MapController mapCon = MapController.GetInstance();

            MapData currentMap = mapCon.currentMap;

            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            // Iterate throught he lightMap and update the light texture
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

        public void SetUnitVisible(GameObject visibleUnit)
        {
            GameUnitComponent guc = visibleUnit.GetComponent<GameUnitComponent>();
            Vector2 currPos = guc.CurrentTilePos;

            int startingX = Mathf.RoundToInt(currPos.x);
            int startingY = Mathf.RoundToInt(currPos.y);

            int width = guc.sizeX;
            int height = guc.sizeY;

            int xEnd = Math.Min(startingX + width, MapController.GetInstance().currentMap.GetWidth());
            int yEnd = Math.Min(startingY + height, MapController.GetInstance().currentMap.GetHeight());

            for (int x = startingX; x < xEnd; x++)
            {
                for (int y = startingY; y < yEnd; y++)
                {
                    lightMap[x, y] = VisionController.VISIBLE;
                }
            }
        }

        /// <summary>
        /// Draws a debug line from one tile location to another.
        /// </summary>
        public static void DrawLine(Vector2 startPos, Vector2 endPos)
        {
            Vector2 worldStart = MapCoordinateUtils.GetTileToWorldPosition(startPos);
            Vector2 worldEnd = MapCoordinateUtils.GetTileToWorldPosition(endPos);

            Debug.DrawLine(worldStart, worldEnd, Color.yellow, DURATION, false);
        }

        /// <summary>
        /// Draws an X over a given tile coordinate
        /// </summary>
        public void DrawMarkOnTile(int x, int y)
        {
            Vector2 startPos = new Vector2();
            Vector2 endPos = new Vector2();

            /// make the \
            startPos.x = x;
            startPos.y = y;
            endPos.x = x + 1f;
            endPos.y = y + 1f;

            Debug.DrawLine(startPos, endPos, Color.yellow, DURATION, false);

            // make the /
            startPos.x = x + 1f;
            startPos.y = y;
            endPos.x = x;
            endPos.y = y + 1f;

            Debug.DrawLine(startPos, endPos, Color.yellow, DURATION, false);
        }
    }
}