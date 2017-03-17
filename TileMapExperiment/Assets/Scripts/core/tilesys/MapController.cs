/// ---------------------------------------------------------------------------
/// MapController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 16th, 2017</date>
/// ---------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
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
        private MapData currentMap;

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

            MapData currentMap = new MapData(rawData);

            // Setup the bounds of the world
            CAMERA_BOUND.x = -1;
            CAMERA_BOUND.y = -1 - currentMap.GetHeight();
            CAMERA_BOUND.width = currentMap.GetWidth();
            CAMERA_BOUND.height = currentMap.GetHeight();

            // Generate the tile mesh
            mapMesh = TileMeshGenerator.GetInstance().GenerateMesh(currentMap);

            gridOverlay = GridOverlayGenerator.GetInstance().GenerateMesh(currentMap);

        }

    }
}