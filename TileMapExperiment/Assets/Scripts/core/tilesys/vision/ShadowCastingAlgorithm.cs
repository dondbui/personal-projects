/// ---------------------------------------------------------------------------
/// ShadowCastingAlgorithm.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 27th, 2017</date>
/// ---------------------------------------------------------------------------

using core.units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.vision
{
    /// <summary>
    /// This is the classic shadow casting field of vision algorithm that is
    /// listed at: http://www.roguebasin.com/index.php?title=Shadow_casting
    /// 
    /// It divides up the vision into 8 octants and traverses the octant away
    /// from the source of sight. The key to the success of this approach is
    /// that is saves the current slope once you find a blocking tile. Using 
    /// that saved slope we traverse that slope and sweep across until we see
    /// light again. 
    /// 
    /// This specific example was heavily influenced by Andy Stobiski's 
    /// implementation and article here: 
    /// http://www.evilscience.co.uk/field-of-vision-using-recursive-shadow-casting-c-3-5-implementation/
    /// </summary>
    public class ShadowCastingAlgorithm
    {
        private static int MAX_DEPTH = 32;

        private const int OCTANT_NNW = 1;
        private const int OCTANT_NNE = 2;
        private const int OCTANT_ENE = 3;
        private const int OCTANT_ESE = 4;
        private const int OCTANT_SSE = 5;
        private const int OCTANT_SSW = 6;
        private const int OCTANT_WSW = 7;
        private const int OCTANT_WNW = 8;

        private const double SLOPE_DELTA = 0.5;

        private static List<GameObject> visibleObjects = new List<GameObject>();

        private static bool enableDebugLines = false;

        public static void Process(MapData currentMap, bool drawDebugLines)
        {
            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            visibleObjects.Clear();

            enableDebugLines = drawDebugLines;

            VisionController vc = VisionController.GetInstance();
            UnitController uc = UnitController.GetInstance();

            List<GameObject> playerUnits = uc.GetAllPlayerUnits();

            for (int i = 0, count = playerUnits.Count; i < count; i++)
            {
                GameObject gob = playerUnits[i];
                GameUnitComponent pGuc = gob.GetComponent<GameUnitComponent>();
                Vector2 startPos = pGuc.CurrentTilePos;

                // Could do it as a for loop but for ease of debugging, I call it out
                // individually. 
                CheckOctant(OCTANT_NNW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_NNE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_ENE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_ESE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_SSE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_SSW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_WSW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
                CheckOctant(OCTANT_WNW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);

                vc.SetUnitVisible(gob);
            }

            enableDebugLines = false;
        }

        /// <summary>
        /// Checks a given octant for vision pertaining to that octant
        /// 
        /// Octant data
        ///
        ///    \ 1 | 2 /
        ///   8 \  |  / 3
        ///   -----+-----
        ///   7 /  |  \ 4
        ///    / 6 | 5 \
        ///
        ///  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW
        /// </summary>
        private static void CheckOctant(int octant, int depth, Vector2 startPos, int mapWidth, int mapHeight, double startSlope, double endSlope)
        {
            MapController mc = MapController.GetInstance();
            VisionController vc = VisionController.GetInstance();
            int[,] lightMap = VisionController.GetInstance().GetLightMap();

            int x = 0;
            int y = 0;

            int pitch = Mathf.RoundToInt((float)startSlope * depth);

            int tileIncre = 1;
            bool checkHoriz = true;
            bool invert = false;
            
            double xHitDelta = SLOPE_DELTA;
            double yHitDelta = SLOPE_DELTA;
            double xClearDelta = SLOPE_DELTA;
            double yClearDelta = SLOPE_DELTA;
            bool checkGreater = true;
            switch (octant)
            {
                case OCTANT_NNW:
                    checkHoriz = true;
                    checkGreater = true;
                    invert = false;
                    tileIncre = 1;

                    xHitDelta = -SLOPE_DELTA;
                    yHitDelta = SLOPE_DELTA;
                    xClearDelta = -SLOPE_DELTA;
                    yClearDelta = -SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x - pitch);
                    y = Mathf.RoundToInt(startPos.y - depth);

                    if (x < 0)
                    {
                        x = 0;
                    }
                    if (y < 0)
                    {
                        return;
                    }
                    break;
                case OCTANT_NNE:
                    checkHoriz = true;
                    checkGreater = false;
                    invert = false;
                    tileIncre = -1;
                    xHitDelta = SLOPE_DELTA;
                    yHitDelta = SLOPE_DELTA;
                    xClearDelta = SLOPE_DELTA;
                    yClearDelta = -SLOPE_DELTA;
                    x = Mathf.RoundToInt(startPos.x + pitch);
                    y = Mathf.RoundToInt(startPos.y - depth);

                    if (x > mapWidth - 1)
                    {
                        x = mapWidth - 1;
                    }
                    if (y < 0)
                    {
                        return;
                    }
                    break;
                case OCTANT_ENE:
                    checkHoriz = false;
                    checkGreater = false;
                    invert = true;
                    tileIncre = 1;
                    xHitDelta = -SLOPE_DELTA;
                    yHitDelta = -SLOPE_DELTA;
                    xClearDelta = SLOPE_DELTA;
                    yClearDelta = -SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x + depth);
                    y = Mathf.RoundToInt(startPos.y - pitch);

                    if (x > mapWidth - 1)
                    {
                        return;
                    }
                    if (y < 0)
                    {
                        y = 0;
                    }
                    break;
                case OCTANT_ESE:
                    checkHoriz = false;
                    checkGreater = true;
                    invert = true;
                    tileIncre = -1;
                    xHitDelta = -SLOPE_DELTA;
                    yHitDelta = SLOPE_DELTA;
                    xClearDelta = SLOPE_DELTA;
                    yClearDelta = SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x + depth);
                    y = Mathf.RoundToInt(startPos.y + pitch);
                    if (x > mapWidth - 1)
                    {
                        return;
                    }
                    if (y > mapHeight - 1)
                    {
                        y = mapHeight - 1;
                    }
                    break;
                case OCTANT_SSE:
                    checkHoriz = true;
                    checkGreater = true;
                    invert = false;
                    tileIncre = -1;
                    xHitDelta = SLOPE_DELTA;
                    yHitDelta = -SLOPE_DELTA;
                    xClearDelta = SLOPE_DELTA;
                    yClearDelta = SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x + pitch);
                    y = Mathf.RoundToInt(startPos.y + depth);

                    if (x > mapWidth - 1)
                    {
                        x = mapWidth - 1;
                    }
                    if (y > mapHeight - 1)
                    {
                        return;
                    }
                    break;
                case OCTANT_SSW:
                    checkHoriz = true;
                    checkGreater = false;
                    invert = false;
                    tileIncre = 1;
                    xHitDelta = -SLOPE_DELTA;
                    yHitDelta = -SLOPE_DELTA;
                    xClearDelta = SLOPE_DELTA;
                    yClearDelta = SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x - pitch);
                    y = Mathf.RoundToInt(startPos.y + depth);

                    if (x < 0)
                    {
                        x = 0;
                    }
                    if (y > mapHeight - 1)
                    {
                        return;
                    }
                    break;
                case OCTANT_WSW:
                    checkHoriz = false;
                    checkGreater = false;
                    invert = true;
                    tileIncre = -1;
                    xHitDelta = SLOPE_DELTA;
                    yHitDelta = SLOPE_DELTA;
                    xClearDelta = -SLOPE_DELTA;
                    yClearDelta = SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x - depth);
                    y = Mathf.RoundToInt(startPos.y + pitch);

                    if (x < 0)
                    {
                        return;
                    }
                    if (y > mapHeight - 1)
                    {
                        y = mapHeight - 1;
                    }

                    break;
                case OCTANT_WNW:
                    checkHoriz = false;
                    checkGreater = true;
                    invert = true;
                    tileIncre = 1;
                    xHitDelta = SLOPE_DELTA;
                    yHitDelta = -SLOPE_DELTA;
                    xClearDelta = -SLOPE_DELTA;
                    yClearDelta = -SLOPE_DELTA;

                    x = Mathf.RoundToInt(startPos.x - depth);
                    y = Mathf.RoundToInt(startPos.y - pitch);
                    
                    if (x < 0)
                    {
                        return;
                    }
                    if (y < 0)
                    {
                        y = 0;
                    }
                    break;
            }

            if (checkHoriz)
            {
                // Check to make sure we aren't done sweeping the row. 
                while (IsValidSlope(x, y, startPos, invert, endSlope, checkGreater))
                {
                    // If the current tile is blocked then we need to flag this
                    // specific one as visible and then do the recursion check
                    // to see what else is visible. 
                    if (mc.IsTileBlockingVision(x, y)) //current cell blocked
                    {
                        // Find the edge block which happens to be when we run into
                        // a situation where a block tile is adjacent on the X-axis 
                        // to a visible tile.
                        //
                        // Also make sure we're still within bounds of the map 
                        if (x - tileIncre >= 0 && x - tileIncre <= mapWidth - 1 
                            && !mc.IsTileBlockingVision(x - tileIncre, y))
                        {
                            // Figure out what object we hit that is blocking vision
                            GameObject hitObject = UnitController.GetInstance().GetUnitAtTile(x, y);
                            if (hitObject != null)
                            {
                                // Keep track of the vision blocker for later so we can know what
                                // units are visible to player.
                                visibleObjects.Add(hitObject);
                            }

                            // Light this tile up since even though it is a blocking tile, it's
                            // the tile facing the vision unit and we can see this tile, we just
                            // can't see what's past this tile. 
                            lightMap[x, y] = VisionController.VISIBLE;

                            if (enableDebugLines)
                            {
                                vc.DrawMarkOnTile(x, y);
                            }

                            // Recursively check the slope cast by the vision blocking tile. 
                            CheckOctant(
                                octant, 
                                depth + 1, 
                                startPos, 
                                mapWidth, 
                                mapHeight, 
                                startSlope, 
                                GetSlope(x + xHitDelta, y + yHitDelta, startPos.x, startPos.y, invert));
                        }
                    }
                    // So we didn't hit anything so let's check to make sure we're not in the area of the
                    // shadow cast.
                    else
                    {
                        // Check to make sure we're still on the map
                        // and
                        // If we're adjacent to a blocking tile next to us
                        if (x - tileIncre >= 0 && x - tileIncre <= mapWidth - 1 && 
                            mc.IsTileBlockingVision(x - tileIncre, y))
                        {
                            // If we're adjacent to the blocking tile then that means we can see that
                            // blocking tile but not what's beyond it so light it up.
                            lightMap[x - tileIncre, y] = VisionController.VISIBLE;
                            if (enableDebugLines)
                            {
                                vc.DrawMarkOnTile(x - tileIncre, y);
                            }

                            // Recursively check the slop of the shadow cast
                            startSlope = GetSlope(x + xClearDelta, y + yClearDelta, startPos.x, startPos.y, invert);

                            // Depending on the octant we may need to flip the slope. 
                            if (HasNegativeSlopeOnHit(octant))
                            {
                                startSlope *= -1;
                            }
                        }

                        // Light up this tile since it's not a blocked one. 
                        lightMap[x, y] = VisionController.VISIBLE;

                        if (enableDebugLines)
                        {
                            vc.DrawMarkOnTile(x, y);
                        }
                    }

                    x += tileIncre;
                }
                x -= tileIncre;
            }
            else
            {
                // Check to make sure we haven't hit the finished sweeping the column
                while (IsValidSlope(x, y, startPos, invert, endSlope, checkGreater))
                {
                    // If the current tile is blocked then we need to flag this
                    // specific one as visible and then do the recursion check
                    // to see what else is visible. 
                    if (mc.IsTileBlockingVision(x, y)) //current cell blocked
                    {
                        // Find the edge block which happens to be when we run into
                        // a situation where a block tile is adjacent to a visible
                        // tile
                        //
                        // Also check to make sure we're still within the bounds of the map. 
                        if (y - tileIncre >= 0 && y - tileIncre <= mapHeight - 1 
                            && !mc.IsTileBlockingVision(x, y - tileIncre))
                        {
                            // Check to see if we hit a unit. 
                            GameObject hitObject = UnitController.GetInstance().GetUnitAtTile(x, y);
                            if (hitObject != null)
                            {
                                // Save this object for later so we can know which units this
                                // vision unit is able to see. 
                                visibleObjects.Add(hitObject);
                            }

                            // Technically this a vision blocking tile but it is still visible
                            // to the vision unit and not in the shadow cast. So light it up.
                            lightMap[x, y] = VisionController.VISIBLE;

                            if (enableDebugLines)
                            {
                                vc.DrawMarkOnTile(x, y);
                            }

                            // Recursively check the new slope since we don't want to light
                            // up the shadow cast area so no need to check them. 
                            CheckOctant(
                                octant,
                                depth + 1,
                                startPos,
                                mapWidth,
                                mapHeight,
                                startSlope,
                                GetSlope(x + xHitDelta, y + yHitDelta, startPos.x, startPos.y, invert));
                        }
                    }
                    else // So we didn't hit a blocking tile.
                    {
                        // Make sure we're in the map bounds
                        // 
                        // Also check if the adjacent tile on the Y-Axis is blocking.
                        // if it is then we're at the other side of the shadow cast.
                        if (y - tileIncre >= 0 && y - tileIncre <= mapHeight -1 
                            && mc.IsTileBlockingVision(x, y - tileIncre))
                        {
                            // Light up the adjacent tile since though it is on 
                            // a blocking tile, it's still on the vision ray cast
                            // of this vision unit. Which means it's not technically
                            // in the shadow cast but rather the one casting the 
                            // shadow. So light that guy up.
                            lightMap[x, y - tileIncre] = VisionController.VISIBLE;
                            if (enableDebugLines)
                            {
                                vc.DrawMarkOnTile(x, y - tileIncre);
                            }

                            // Get the new slope for this side of the shadow cast so 
                            // we don't check tile within the shadow cast area.
                            startSlope = GetSlope(x + xClearDelta, y + yClearDelta, startPos.x, startPos.y, invert);

                            // For certain octants we need to flip the slope to ensure
                            // we have the right slope.
                            if (HasNegativeSlopeOnHit(octant))
                            {
                                startSlope *= -1;
                            }
                        }

                        // Tile not blocking vision? Light it up.
                        lightMap[x, y] = VisionController.VISIBLE;

                        if (enableDebugLines)
                        {
                            vc.DrawMarkOnTile(x, y);
                        }
                    }

                    y += tileIncre;
                }
                y -= tileIncre;
            }

            // Clamp the X to the bounds of the map
            if (x < 0)
            {
                x = 0;
            }
            else if(x > mapWidth - 1)
            {
                x = mapWidth - 1;
            }

            // Clamp the Y to the bounds of the map
            if (y < 0)
            {
                y = 0;
            }
            else if (y > mapHeight - 1)
            {
                y = mapHeight;
            }

            // Assuming we haven't hit the distance limit and the current tile isn't blocked
            // then we should keep checking the next row/column
            if (depth < MAX_DEPTH && !mc.IsTileBlockingVision(x, y))
            {
                CheckOctant(octant, depth + 1, startPos, mapWidth, mapHeight, startSlope, endSlope);
            }
        }

        /// <summary>
        /// Get the slope of the given points
        /// </summary>
        private static double GetSlope(double x1, double y1, double x2, double y2, bool invert)
        {
            double slope;

            if (invert)
            {
                slope = Math.Round((y1 - y2) / (x1 - x2), 2);
            }
            else
            {
                slope = Math.Round((x1 - x2) / (y1 - y2), 2);
            }

            return slope;
        }

        /// <summary>
        /// Is our slope still valid enough for us to continue iterating over the tiles
        /// </summary>
        private static bool IsValidSlope(int x, int y, Vector2 startPos, bool invert, double endSlope, bool checkGreater)
        {
            double slope;

            if (checkGreater)
            {
                slope = GetSlope(x, y, startPos.x, startPos.y, invert);
                
                return slope >= endSlope;
            }

            slope = GetSlope(x, y, startPos.x, startPos.y, invert);

            return slope <= endSlope;
        }

        /// <summary>
        /// Should we flip the slope based on the octant?
        /// </summary>
        private static bool HasNegativeSlopeOnHit(int octant)
        {
            switch (octant)
            {
                case OCTANT_NNE:
                case OCTANT_ENE:
                case OCTANT_SSW:
                case OCTANT_WSW:
                    return true;
            }

            return false;
        }
    }
}