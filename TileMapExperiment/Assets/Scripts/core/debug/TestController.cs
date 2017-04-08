/// ---------------------------------------------------------------------------
/// TestController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 26th, 2017</date>
/// ---------------------------------------------------------------------------

using core.tilesys;
using core.tilesys.vision;
using core.units;
using UnityEngine;

namespace core.debug
{
    /// <summary>
    /// Holds all of the test specific code so that I don't have to 
    /// clutter up the main loop or other areas of the code. 
    /// </summary>
    public class TestController
    {
        private static TestController instance;

        private TestController()
        {

        }

        public static TestController GetInstance()
        {
            if (instance == null)
            {
                instance = new TestController();
            }

            return instance;
        }

        /// <summary>
        /// Runs a series of tests
        /// </summary>
        public void RunTests()
        {
            PlacePlayerUnits();

            //RandomlyPlaceEnemies();
            RandomlyPlaceAsteroids();

            VisionController.GetInstance().UpdateVisionTiles();
        }

        /// <summary>
        /// Place all of the player units onto the game board.
        /// </summary>
        private void PlacePlayerUnits()
        {
            Vector2 pos = new Vector2();

            int width = MapController.GetInstance().currentMap.GetWidth();
            int height = MapController.GetInstance().currentMap.GetHeight();

            for (int i = 0; i < 1; i++)
            {
                int randX = UnityEngine.Random.Range(0, width);
                int randY = UnityEngine.Random.Range(0, height);

                pos.x = randX;
                pos.y = randY;

                if (i == 0)
                {
                    pos.x = 15;
                    pos.y = 15;
                }

                GameObject go = UnitController.GetInstance().PlaceNewUnit("player" + i, "shipAssets_77", pos, false);
                GameUnitComponent guc = go.GetComponent<GameUnitComponent>();
                guc.type = UnitType.PLAYER;
            }
        }

        /// <summary>
        /// Randomly place enemy units onto the board.
        /// </summary>
        private void RandomlyPlaceEnemies()
        {
            Vector2 pos = new Vector2();

            int width = MapController.GetInstance().currentMap.GetWidth();
            int height = MapController.GetInstance().currentMap.GetHeight();

            for (int i = 0; i < 10; i++)
            {
                int randX = UnityEngine.Random.Range(0, width);
                int randY = UnityEngine.Random.Range(0, height);

                pos.x = randX;
                pos.y = randY;

                if (MapController.GetInstance().IsTileRangeOccupied(randX, randY, 1, 1))
                {
                    continue;
                }

                GameObject go = UnitController.GetInstance().PlaceNewUnit("enemy" + i, "shipAssets_4", pos, false);
                GameUnitComponent guc = go.GetComponent<GameUnitComponent>();
                guc.type = UnitType.ENEMY;
            }
        }

        /// <summary>
        /// Randomly place asteroids all over the board to help test with occupied
        /// tiles and vision blocking. 
        /// </summary>
        private void RandomlyPlaceAsteroids()
        {
            Vector2 pos = new Vector2();

            int width = MapController.GetInstance().currentMap.GetWidth();
            int height = MapController.GetInstance().currentMap.GetHeight();

            for (int i = 0; i < 40; i++)
            {
                int randX = UnityEngine.Random.Range(0, width);
                int randY = UnityEngine.Random.Range(0, height);

                pos.x = randX;
                pos.y = randY;

                if (i == 0)
                {
                    pos.x = 14;
                    pos.y = 12;
                }

                if (MapController.GetInstance().IsTileRangeOccupied(randX, randY, 2, 2))
                {
                    continue;
                }

                UnitController.GetInstance().PlaceNewUnit("asteroid" + i, "asteroid", pos, true);
            }
        }
    }
}