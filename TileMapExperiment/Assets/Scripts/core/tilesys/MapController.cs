/// ---------------------------------------------------------------------------
/// MapController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 16th, 2017</date>
/// ---------------------------------------------------------------------------

using core.debug;
using core.units;
using System.Text;
using UnityEngine;

namespace core.tilesys
{
    public class MapController
    {
        private static MapController instance;

        /// Stores the bounding box of the world. 
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
            CAMERA_BOUND.y = -1 - currentMap.GetHeight();
            CAMERA_BOUND.width = currentMap.GetWidth();
            CAMERA_BOUND.height = currentMap.GetHeight();

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

                    if (state == 1)
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
                    occupiedTileMap[x, y] = 0;
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

            for (int x = startingX, xEnd = startingX + width; x < xEnd; x++)
            {
                for (int y = startingY, yEnd = startingY + height; y < yEnd; y++)
                {
                    occupiedTileMap[x, y] = 1;
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
                    occupiedTileMap[x, y] = 0;
                }
            }

            // Tell the unit controller to update itelf. 
            UnitController.GetInstance().ReplaceAllUnits();
        }

    }
}