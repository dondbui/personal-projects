/// ---------------------------------------------------------------------------
/// MapController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 16th, 2017</date>
/// ---------------------------------------------------------------------------

using core.debug;
using core.tilesys.pathing;
using core.units;
using System;
using System.Text;
using UnityEngine;

namespace core.tilesys
{
    public class MapController
    {
        private const int OCCUPIED = 1;
        private const int VACANT = 0;
        private const int OCCUPIED_BLOCKS = 2;

        private static MapController instance;

        /// <summary>
        /// Handles the pathing logic and visualization
        /// </summary>
        public PathingController pathingController;

        /// <summary>
        /// Stores the bounding box of the world. 
        /// </summary>
        public static Rect CAMERA_BOUND = new Rect();

        /// <summary>
        /// The current map that we are viewing.
        /// </summary>
        public MapData currentMap { get; private set; } 

        /// <summary>
        /// The game object that contains the textured mesh of the map
        /// </summary>
        private GameObject mapMesh;

        private GameObject gridOverlay;

        /// <summary>
        /// Keeps track of what tiles are currently occupied. We only really
        /// need to keep track of ones and zeroes to keep things light
        /// </summary>
        private int[,] occupiedTileMap;

        private MapController()
        {
            pathingController = new PathingController();
        }

        /// <summary>
        /// Gets the only instance of the MapController.
        /// </summary>
        /// <returns></returns>
        public static MapController GetInstance()
        {
            // No instance? No problem, let's make him.
            if (instance == null)
            {
                instance = new MapController();
            }

            return instance;
        }

        public void LoadMapData(string csvPath)
        {
            TiledCSVParser csvParser = TiledCSVParser.GetInstance();
            string[,] rawData = csvParser.ReadTiledCSVFile(csvPath);

            currentMap = new MapData(rawData);

            // Setup the bounds of the world
            CAMERA_BOUND.x = -1;
            CAMERA_BOUND.y = -1;
            CAMERA_BOUND.width = currentMap.GetWidth() + 1;
            CAMERA_BOUND.height = currentMap.GetHeight() + 1;

            // Generate the tile mesh
            mapMesh = TileMeshGenerator.GetInstance().GenerateMesh(currentMap);

            gridOverlay = GridOverlayGenerator.GetInstance().GenerateMesh(currentMap);

            occupiedTileMap = new int[currentMap.GetWidth(), currentMap.GetHeight()];
        }

        public void PrintOutOccupiedTiles()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Occupied: \n");

            Vector2 tilePos = new Vector2();

            for (int y = 0, yCount = occupiedTileMap.GetLength(0); y < yCount; y++)
            {
                for (int x = 0, xCount = occupiedTileMap.GetLength(1); x < xCount; x++)
                {
                    int state = occupiedTileMap[x, y];

                    sb.Append(state.ToString() + ",");

                    if (state != VACANT)
                    {
                        tilePos.x = x;
                        tilePos.y = y;
                        MapDebugController.DrawTileOccupied(tilePos);
                    }
                }
                sb.Append("\n");
            }

            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// Returns true if the given unit is placeable at the given
        /// tile position.
        /// </summary>
        public bool IsUnitPlaceableAt(GameObject unit, int x, int y)
        {
            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();

            int width = currentMap.GetWidth();
            int height = currentMap.GetHeight();

            // Make sure unit is in bounds
            if (x < 0 || x + guc.sizeX - 1 > width - 1)
            {
                return false;
            }

            if (y < 0 || y + guc.sizeY - 1 > height - 1)
            {
                return false;
            }

            // Check to see if any of the tiles are already occupied
            for (int i = y; i <= guc.sizeY; i++)
            {
                for (int j = x; j <= guc.sizeX; j++)
                {
                    if (IsTileOccupied(i, j))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns if the given tile coordinates are already occupied. 
        /// </summary>
        public bool IsTileOccupied(int x, int y)
        {
            return occupiedTileMap[x, y] != VACANT;
        }

        /// <summary>
        /// Given a starting point which is upper left, and the size of the 
        /// rect we want to check, check to see if any tile in that range
        /// is already occupied. 
        /// </summary>
        public bool IsTileRangeOccupied(int x, int y, int sizeX, int sizeY)
        {
            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            // If we're out of bounds no need to check each tile
            if (x < 0 || x + sizeX - 1 > mapWidth - 1 ||
                y < 0 || y + sizeY - 1 > mapHeight - 1)
            {
                return true;
            }

            // Check each tile in the range. 
            for (int yCounter = y, endY = y + sizeY; yCounter < endY; yCounter++)
            {
                for (int xCounter = x, endX = x + sizeX; xCounter < endX; xCounter++)
                {
                    if (IsTileOccupied(xCounter, yCounter))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Does the given tile contain a unit that blocks vision?
        /// </summary>
        public bool IsTileBlockingVision(int x, int y)
        {
            if (x < 0 || x >= currentMap.GetWidth())
            {
                return true;
            }

            if (y < 0 || y >= currentMap.GetHeight())
            {
                return true;
            }

            return occupiedTileMap[x, y] == OCCUPIED_BLOCKS;
        }

        public void LiftUnit(GameObject unit, Vector2 pos)
        {
            int startingX = Mathf.RoundToInt(pos.x);
            int startingY = Mathf.RoundToInt(pos.y);

            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();
            int width = guc.sizeX;
            int height = guc.sizeY;

            for (int x = startingX, xEnd = startingX + width; x < xEnd; x++)
            {
                for (int y = startingY, yEnd = startingY + height; y < yEnd; y++)
                {
                    occupiedTileMap[x, y] = VACANT;
                }
            }
        }

        public void PlaceUnit(GameObject unit, Vector2 pos)
        {
            int startingX = Mathf.RoundToInt(pos.x);
            int startingY = Mathf.RoundToInt(pos.y);

            GameUnitComponent guc = unit.GetComponent<GameUnitComponent>();
            int width = guc.sizeX;
            int height = guc.sizeY;

            int xEnd = Math.Min(startingX + width, MapController.GetInstance().currentMap.GetWidth());
            int yEnd = Math.Min(startingY + height, MapController.GetInstance().currentMap.GetHeight());

            for (int x = startingX; x < xEnd; x++)
            {
                for (int y = startingY; y < yEnd; y++)
                {
                    occupiedTileMap[x, y] = guc.blocksVision ? OCCUPIED_BLOCKS : OCCUPIED;
                }
            }
        }

        /// <summary>
        /// Clears the occupied map and makes the unit controller place down all
        /// the units once again. 
        /// </summary>
        public void ForceRefreshOccupiedMap()
        {
            // Clears the occupied map
            for (int x = 0, xEnd = currentMap.GetWidth(); x < xEnd; x++)
            {
                for (int y = 0, yEnd = currentMap.GetHeight(); y < yEnd; y++)
                {
                    occupiedTileMap[x, y] = VACANT;
                }
            }

            // Tell the unit controller to update itelf. 
            UnitController.GetInstance().ReplaceAllUnits();
        }

    }
}