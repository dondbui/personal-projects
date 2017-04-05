/// ---------------------------------------------------------------------------
/// GameUnitComponent.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 16th, 2017</date>
/// ---------------------------------------------------------------------------

using core.tilesys;
using System.Collections.Generic;
using UnityEngine;

namespace core.units
{
    /// <summary>
    /// This monobehaviour is used to store major state date for a particular
    /// unit. 
    /// </summary>
    public class GameUnitComponent : MonoBehaviour
    {
        [SerializeField]
        public int sizeX;

        [SerializeField]
        public int sizeY;

        public bool blocksVision;

        private int[,] visionMap;

        public UnitType type = UnitType.ENVIRONMENT;

        /// <summary>
        /// The x offset needed to compensate for the center pivot on larger
        /// size unit while retaining center of 0.0 tile being the movement
        /// position
        /// </summary>
        public float tileOffsetX
        {
            get
            {
                return ((sizeX - 1) / 2f);
            }
        }

        /// <summary>
        /// The y offset needed to compensate for the center pivot on larger
        /// size unit while retaining center of 0.0 tile being the movement
        /// position
        /// </summary>
        public float tileOffsetY
        {
            get
            {
                return -((sizeY - 1) / 2f);
            }
        }

        /// <summary>
        /// List of destinations awaiting animation completion. 
        /// </summary>
        public List<Vector2> pendingDestinations = new List<Vector2>();

        public Vector2 CurrentTilePos
        {
            get
            {
                Vector2 currentPos = this.gameObject.transform.position;

                // Remove the tile size offset
                currentPos.x -= tileOffsetX;
                currentPos.y -= tileOffsetY;

                return MapCoordinateUtils.GetTilePosFromWorld(currentPos);
            }
        }

        public Rect GetCorners()
        {
            Vector2 currWorldPos = this.gameObject.transform.position;

            Rect corners = new Rect();
            corners.x = currWorldPos.x - (sizeX / 2);
            corners.y = -currWorldPos.y - (sizeY / 2);

            corners.width = sizeX;
            corners.height = sizeY;

            return corners;
        }

        public void InitializeVisionMap(int width, int height)
        {
            visionMap = new int[width, height];
        }

        public void ClearVisionMap()
        {
            int width = visionMap.GetLength(0);
            int height = visionMap.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; x < height; y++)
                {
                    // Set everything to darkness
                    visionMap[x, y] = 0;
                }
            }
        }

        /// <summary>
        /// Is this unit occupying the given tile point
        /// </summary>
        public bool IsUnitOnTile(int x, int y)
        {
            Vector2 currentPos = CurrentTilePos;

            // Check if we're in the x realm
            if (x < currentPos.x ||  // Check if we're less than the unit's starting x
                x > currentPos.x + (sizeX - 1)) // Check if they've overshot
            {
                return false;
            }

            if (y < currentPos.y ||  // Check if we're less than the unit's starting y
                y > currentPos.y + (sizeY - 1)) // Check if they've overshot
            {
                return false;
            }

            return true;
        }

        public bool HasPendingDestinations()
        {
            return pendingDestinations.Count > 0;
        }

        public Vector2 GetCurrentDestination()
        {
            return pendingDestinations[0];
        }

        public void RemoveCurrentDestination()
        {
            pendingDestinations.RemoveAt(0);
        }

        public void QueueDestination(Vector2 destination)
        {
            pendingDestinations.Add(destination);
        }

        public void ClearPendingDestinations()
        {
            pendingDestinations.Clear();
        }
    }
}