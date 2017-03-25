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