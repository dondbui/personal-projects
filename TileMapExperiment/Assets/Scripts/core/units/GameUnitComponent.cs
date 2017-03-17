/// ---------------------------------------------------------------------------
/// GameUnitComponent.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 16th, 2017</date>
/// ---------------------------------------------------------------------------

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

        public List<Vector2> pendingDestinations = new List<Vector2>();

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