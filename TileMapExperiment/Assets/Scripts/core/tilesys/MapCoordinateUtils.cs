/// ---------------------------------------------------------------------------
/// MapCoordinateUtils.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 15th, 2017</date>
/// ---------------------------------------------------------------------------

using UnityEngine;

namespace core.tilesys
{
    /// <summary>
    /// Contains utility methods for map coordinate calculations
    /// </summary>
    public class MapCoordinateUtils
    {
        /// <summary>
        /// The pixel width/height of the map tiles.
        /// </summary>
        public const int MAP_TILE_PIXEL_SIZE = 32;

        /// <summary>
        /// Given a Vector2 of the tile position, return the world space
        /// coordinates in a new Vector 2. 
        /// </summary>
        /// <param name="tilePos"></param>
        /// <returns></returns>
        public static Vector2 GetTileToWorldPosition(Vector2 tilePos)
        {
            return GetTileToWorldPosition((int)tilePos.x, (int)tilePos.y); ;
        }

        /// <summary>
        /// Converts the x,y coordinate to a new Vector2 in world space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 GetTileToWorldPosition(int x, int y)
        {
            Vector2 newPos = new Vector2();

            newPos.x = x + 0.5f;
            newPos.y = -(y + 0.5f);

            return newPos;
        }

        /// <summary>
        /// Given a click position this returns to you the position of the click in
        /// tile space. 
        /// </summary>
        /// <param name="clickPos"></param>
        /// <returns></returns>
        public static Vector2 GetTilePosFromClickPos(Vector3 clickPos)
        {
            Vector2 returnPos = new Vector2();

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(clickPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                // The raw position. 
                // Debug.Log(hit.collider.name + ": " + hit.point.x + ", " + hit.point.y);

                returnPos.x = Mathf.FloorToInt(hit.point.x);
                returnPos.y = -Mathf.CeilToInt(hit.point.y);
            }

            Debug.Log("returnPos: " + returnPos.ToString());
            return returnPos;
        }
    }
}
