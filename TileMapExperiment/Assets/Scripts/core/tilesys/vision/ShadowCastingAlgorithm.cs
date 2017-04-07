
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
        private static int MAX_DEPTH = 64;

        private const int OCTANT_NNW = 1;
        private const int OCTANT_NNE = 2;
        private const int OCTANT_ENE = 3;
        private const int OCTANT_ESE = 4;
        private const int OCTANT_SSE = 5;
        private const int OCTANT_SSW = 6;
        private const int OCTANT_WSW = 7;
        private const int OCTANT_WNW = 8;

        private const double SLOPE_DELTA = 0.5;

        public static void Process(MapData currentMap, bool drawDebugLines)
        {
            int mapWidth = currentMap.GetWidth();
            int mapHeight = currentMap.GetHeight();

            UnitController uc = UnitController.GetInstance();
            GameObject gob = uc.GetUnitByID("player0");
            GameUnitComponent pGuc = gob.GetComponent<GameUnitComponent>();

            Vector2 startPos = pGuc.CurrentTilePos;

            CheckOctant(OCTANT_NNW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_NNE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_ENE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_ESE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_SSE, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_SSW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_WSW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
            CheckOctant(OCTANT_WNW, 1, startPos, mapWidth, mapHeight, 1.0, 0.0);
        }

        private static double GetSlope(double x1, double y1, double x2, double y2, bool invert)
        {
            if (invert)
                return (y1 - y2) / (x1 - x2);
            else
                return (x1 - x2) / (y1 - y2);
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

            int pitch = (int)Math.Round(startSlope * (double)depth);

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

                    x = (int)startPos.x - pitch;
                    y = (int)startPos.y - depth;

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
                    x = (int)startPos.x + pitch;
                    y = (int)startPos.y - depth;

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

                    x = (int)startPos.x + depth;
                    y = (int)startPos.y - pitch;

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

                    x = (int)startPos.x + depth;
                    y = (int)startPos.y + pitch;
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

                    x = (int)startPos.x + pitch;
                    y = (int)startPos.y + depth;

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

                    x = (int)startPos.x - pitch;
                    y = (int)startPos.y + depth;

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

                    x = (int)startPos.x - depth;
                    y = (int)startPos.y + pitch;

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

                    x = (int)startPos.x - depth;
                    y = (int)startPos.y - pitch;
                    
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
                while (IsValidSlope(x, y, startPos, invert, endSlope, checkGreater))
                {
                    //Debug.Log("Checking Tile: " + x + ", " + y);

                    // If the current tile is blocked then we need to flag this
                    // specific one as visible and then do the recursion check
                    // to see what else is visible. 
                    if (mc.IsTileBlockingVision(x, y)) //current cell blocked
                    {
                        // Find the edge block which happens to be when we run into
                        // a situation where a block tile is adjacent to a visible
                        // tile
                        if (x - tileIncre >= 0 && !mc.IsTileBlockingVision(x - tileIncre, y))
                        {
                            //lightMap[x, y] = VisionController.VISIBLE;
                            //vc.DrawMarkOnTile(x, y);

                            //prior cell within range AND open...
                            //...incremenet the depth, adjust the endslope and recurse
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
                    else
                    {
                        if (x - tileIncre >= 0 && mc.IsTileBlockingVision(x - tileIncre, y))
                        {
                            //prior cell within range AND open...
                            //..adjust the startslope
                            startSlope = GetSlope(x + xClearDelta, y + yClearDelta, startPos.x, startPos.y, invert);

                            if (HasNegativeSlopeOnHit(octant))
                            {
                                startSlope *= -1;
                            }
                        }
                        lightMap[x, y] = VisionController.VISIBLE;

                        vc.DrawMarkOnTile(x, y);
                    }

                    x += tileIncre;
                }
                x -= tileIncre;
            }
            else
            {
                while (IsValidSlope(x, y, startPos, invert, endSlope, checkGreater))
                {
                    //Debug.Log("Checking Tile: " + x + ", " + y);

                    // If the current tile is blocked then we need to flag this
                    // specific one as visible and then do the recursion check
                    // to see what else is visible. 
                    if (mc.IsTileBlockingVision(x, y)) //current cell blocked
                    {
                        // Find the edge block which happens to be when we run into
                        // a situation where a block tile is adjacent to a visible
                        // tile
                        if (y - tileIncre >= 0 && !mc.IsTileBlockingVision(x, y - tileIncre))
                        {
                            //lightMap[x, y] = VisionController.VISIBLE;
                            //vc.DrawMarkOnTile(x, y);

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
                    else
                    {
                        if (y >= 0 && mc.IsTileBlockingVision(x, y - tileIncre))
                        {
                            startSlope = GetSlope(x + xClearDelta, y + yClearDelta, startPos.x, startPos.y, invert);

                            if (HasNegativeSlopeOnHit(octant))
                            {
                                startSlope *= -1;
                            }
                        }
                        lightMap[x, y] = VisionController.VISIBLE;

                        vc.DrawMarkOnTile(x, y);
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
        /// Is our slope still valid enough for us to continue iterating over the tiles
        /// </summary>
        private static bool IsValidSlope(int x, int y, Vector2 startPos, bool invert, double endSlope, bool checkGreater)
        {
            if (checkGreater)
            {
                return GetSlope(x, y, startPos.x, startPos.y, invert) >= endSlope;
            }

            return GetSlope(x, y, startPos.x, startPos.y, invert) <= endSlope;
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

        private static void DrawLine(Vector2 startPos, Vector2 endPos)
        {
            Vector2 worldStart = MapCoordinateUtils.GetTileToWorldPosition(startPos);

            Debug.Log("Drawing from: " + startPos.ToString() + " ----> " + endPos.ToString());

            Debug.DrawLine(worldStart, endPos, Color.yellow, 3f, false);

            DrawX(endPos);
        }

        private static void DrawX(Vector2 pos)
        {
            Vector2 startPos = new Vector2();
            Vector2 endPos = new Vector2();

            float offset = 0.1f;

            /// make the \
            startPos.x = pos.x - offset;
            startPos.y = (pos.y - offset);
            endPos.x = pos.x + offset;
            endPos.y = (pos.y + offset);

            Debug.DrawLine(startPos, endPos, Color.yellow, 3f, false);

            // make the /
            startPos.x = pos.x + offset;
            startPos.y = (pos.y - offset);
            endPos.x = pos.x - offset;
            endPos.y = (pos.y + offset);

            Debug.DrawLine(startPos, endPos, Color.yellow, 3f, false);
        }
    }
}