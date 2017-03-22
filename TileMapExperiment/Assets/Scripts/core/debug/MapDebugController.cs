/// ---------------------------------------------------------------------------
/// MapDebugController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 21, 2017</date>
/// ---------------------------------------------------------------------------

using UnityEngine;

namespace core.debug
{
    /// <summary>
    /// A class that contains methods to help us debug through issues on the map.
    /// </summary>
    public class MapDebugController
    {
        /// <summary>
        /// Used to help draw the debug lines. Keep this around
        /// so that we don't have to keep making new Vector2s all 
        /// time. 
        /// </summary>
        private static Vector2 startPos;

        /// <summary>
        /// Used to help draw the debug lines. Keep this around
        /// so that we don't have to keep making new Vector2s all 
        /// time. 
        /// </summary>
        private static Vector2 endPos;

        /// <summary>
        /// Draws a red X across the given tile coordinate
        /// </summary>
        /// <param name="pos"></param>
        public static void DrawTileOccupied(Vector2 pos)
        {
            /// make the \
            startPos.x = pos.x;
            startPos.y = -pos.y;
            endPos.x = pos.x + 1f;
            endPos.y = -pos.y - 1f;

            Debug.DrawLine(startPos, endPos, Color.red, 1f, false);

            // make the /
            startPos.x = pos.x + 1f;
            startPos.y = -pos.y;
            endPos.x = pos.x;
            endPos.y = -pos.y - 1f;

            Debug.DrawLine(startPos, endPos, Color.red, 1f, false);
        }

    }
}