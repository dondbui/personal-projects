/// ---------------------------------------------------------------------------
/// PathArrow.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>April 13th, 2017</date>
/// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys.pathing
{
    /// <summary>
    /// Handles the creation/updating of the arrow indicating the path
    /// of a unit's movement. 
    /// </summary>
    public class PathArrow
    {
        private const int TILE_SIZE = 1;

        /// <summary>
        /// The width/height pixel size of each tile in the path arrow spritesheet
        /// 
        /// TODO: Move this to config file. 
        /// </summary>
        private const int ARROW_SPRITE_SIZE = 32;

        private const int SPRITE_STR = 0;
        private const int SPRITE_TtR = 1;
        private const int SPRITE_TtL = 2;
        private const int SPRITE_END = 3;
        private const int SPRITE_BtR = 4;
        private const int SPRITE_BtL = 5;

        private float numSpritesX = 0;
        private float numSpritesY = 0;

        private GameObject arrowObj;
        private Mesh arrowMesh;
        private MeshFilter meshFilter;

        /// <summary>
        /// Keeps tracks of the vertices used to create the mesh
        /// </summary>
        List<Vector3> vertices = new List<Vector3>();

        /// <summary>
        /// Keeps track of the triangle vert order for the mesh
        /// </summary>
        List<int> triangles = new List<int>();

        /// <summary>
        /// Keeps track of the UV coordinates for the mesh
        /// </summary>
        List<Vector2> uvArray = new List<Vector2>();

        LinkedList<PathArrowTileNode> pathNodes = new LinkedList<PathArrowTileNode>();

        public PathArrow(Vector2 startPos, Vector2 endPos)
        {
            arrowObj = new GameObject();
            arrowObj.transform.rotation = Quaternion.AngleAxis(180, Vector3.right);
            meshFilter = arrowObj.AddComponent<MeshFilter>();

            Material mat = Resources.Load<Material>("Materials/pathingArrows");
            MeshRenderer mr = arrowObj.AddComponent<MeshRenderer>();
            mr.material = mat;

            int texWidth = mat.mainTexture.width;
            int texHeight = mat.mainTexture.height;

            // Figure out how many sprites the sprite sheet has
            // then cache it so we don't need to calculate this often
            numSpritesX = texWidth / ARROW_SPRITE_SIZE;
            numSpritesY = texHeight / ARROW_SPRITE_SIZE;

            arrowMesh = new Mesh();
            arrowMesh.MarkDynamic();

            arrowObj.name = "pathArrow";

            Update(startPos, endPos);
        }

        public void Update(Vector2 startPos, Vector2 endPos)
        {
            Clear();

            int xDelta = Mathf.RoundToInt(endPos.x - startPos.x);
            int yDelta = Mathf.RoundToInt(endPos.y - startPos.y);

            if (xDelta == 0 && yDelta == 0)
            {
                return;
            }

            int xIncre = xDelta > 0 ? 1 : -1;
            int yIncre = yDelta > 0 ? 1 : -1;

            int x = (int)startPos.x;
            int y = (int)startPos.y;
            
            // draw the vertical
            int end = Math.Abs(yDelta);
            for (int i = 0; i < end; i++)
            {
                AddTile(x, y + i * yIncre, false, true);
            }

            end = Math.Abs(xDelta);
            for (int i = 0; i <= end; i++)
            {
                AddTile(x + i * xIncre, y + yDelta, false, false);
            }

            CreateUVCoordinates();

            arrowMesh.vertices = vertices.ToArray();
            arrowMesh.triangles = triangles.ToArray();

            meshFilter.mesh = arrowMesh;
            meshFilter.mesh.uv = uvArray.ToArray();
        }

        public void Clear()
        {
            // Clear out the path nodes before we create new ones
            pathNodes.Clear();

            // Clear out everything
            vertices.Clear();
            triangles.Clear();
            uvArray.Clear();
            arrowMesh.Clear(false);

            // Set everything again to update the mesh
            arrowMesh.vertices = vertices.ToArray();
            arrowMesh.triangles = triangles.ToArray();
            meshFilter.mesh = arrowMesh;
            meshFilter.mesh.uv = uvArray.ToArray();
        }

        /// <summary>
        /// Creates the UV coordinates for the pathing arrow mesh by iterating 
        /// through the path and determining what texture segment to use based
        /// on what is the movement delta between the current, previous and next
        /// tile.
        /// </summary>
        private void CreateUVCoordinates()
        {
            LinkedListNode<PathArrowTileNode> node = pathNodes.First;
            while (node != null)
            {
                // Check the previous and the next one
                PathArrowTileNode prev = node.Previous != null ? node.Previous.Value : null;
                PathArrowTileNode next = node.Next != null ? node.Next.Value : null;
                
                // Determine what type of tile to display
                PathArrowTileEnum tileType = GetArrowTileType(node.Value, prev, next);

                AddUVForTile(tileType);

                // set the node to the next one.
                node = node.Next;
            }
        }

        private void AddTile(int x, int y, bool isFinal, bool isVert)
        {
            AddVerticesAtTilePos(x, y);
            pathNodes.AddLast(new PathArrowTileNode(x, y));
        }

        private void AddVerticesAtTilePos(int x, int y)
        {
            vertices.Add(new Vector3(x, y, 0)); //  top left
            vertices.Add(new Vector3(x + TILE_SIZE, y, 0)); // top right
            vertices.Add(new Vector3(x + TILE_SIZE, y + TILE_SIZE, 0)); // bottom right
            vertices.Add(new Vector3(x, y + TILE_SIZE, 0)); // bottom left

            int topLeft = vertices.Count - 4;
            int topRight = topLeft + 1;
            int bottomRight = topRight + 1;
            int bottomLeft = bottomRight + 1;

            // Add top triangle verts
            triangles.Add(topLeft);
            triangles.Add(topRight);
            triangles.Add(bottomRight);

            // Add bottom triangle verts
            triangles.Add(topLeft);
            triangles.Add(bottomRight);
            triangles.Add(bottomLeft);
        }

        private void AddUVForTile(PathArrowTileEnum type)
        {
            switch (type)
            {
                case PathArrowTileEnum.Horizontal:
                    AddStraight(false);
                    break;
                case PathArrowTileEnum.Vertical:
                    AddStraight(true);
                    break;
                case PathArrowTileEnum.TopToRight:
                    AddElbow(SPRITE_TtR);
                    break;
                case PathArrowTileEnum.TopToLeft:
                    AddElbow(SPRITE_TtL);
                    break;
                case PathArrowTileEnum.BottomToRight:
                    AddElbow(SPRITE_BtR);
                    break;
                case PathArrowTileEnum.BottomToLeft:
                    AddElbow(SPRITE_BtL);
                    break;
                case PathArrowTileEnum.EndArrowUp:
                case PathArrowTileEnum.EndArrowDown:
                case PathArrowTileEnum.EndArrowLeft:
                case PathArrowTileEnum.EndArrowRight:
                    AddEndArrow(type);
                    break;
            }
        }

        /// <summary>
        /// Add the UV data for the tile. UV coordinates start with 0,0
        /// at the lower left and 1,1 being the upper right. 
        /// 
        /// See diagram here: 
        /// http://www.aclockworkberry.com/wp-content/uploads/2015/08/Max-Unity-UVs.png
        /// </summary>
        private void AddUVForTile(int x, int y, bool isFinal, bool isVert)
        {
            int tileNum = 0;

            AddStraight(isVert);
        }

        /// <summary>
        /// Adds a straight piece though we need to rotate the texture
        /// if it's a horizontal piece. 
        /// </summary>
        private void AddStraight(bool isVert)
        {
            Vector2 topLeft = new Vector2(SPRITE_STR / numSpritesX, 1f);
            Vector2 topRight = new Vector2(SPRITE_STR + 1f / numSpritesX, 1f);
            Vector2 bottomRight = new Vector2(SPRITE_STR + 1f / numSpritesX, 1 / 2f);
            Vector2 bottomLeft = new Vector2(SPRITE_STR / numSpritesX, 1 / 2f);

            // If it's vertical we use the correct UV coords
            if (isVert)
            {
                // Top left of tile in atlas
                uvArray.Add(topLeft);

                // Top right of tile in atlas
                uvArray.Add(topRight);

                // Bottom right of tile in atlas
                uvArray.Add(bottomRight);

                //Bottom left of tile in atlas
                uvArray.Add(bottomLeft);
            }
            else // If it's horizontal we gotta rotate the straigt piece
            {
                //Bottom left of tile in atlas
                uvArray.Add(bottomLeft);

                // Top left of tile in atlas
                uvArray.Add(topLeft);

                // Top right of tile in atlas
                uvArray.Add(topRight);

                // Bottom right of tile in atlas
                uvArray.Add(bottomRight);
            }
        }

        /// <summary>
        /// Adds an elbow piece of the arrow path sprite since these
        /// are very specific, we don't have to worry about orientation
        /// </summary>
        private void AddElbow(int spriteIndex)
        {
            float xSlot = spriteIndex % numSpritesX;
            float ySlot = numSpritesY - ((int)(spriteIndex / numSpritesX));

            Vector2 topLeft = new Vector2(xSlot / numSpritesX, ySlot / numSpritesY);
            Vector2 topRight = new Vector2((xSlot + 1f) / numSpritesX, ySlot / numSpritesY);
            Vector2 bottomRight = new Vector2((xSlot + 1f) / numSpritesX, (ySlot - 1f) / numSpritesY);
            Vector2 bottomLeft = new Vector2(xSlot / numSpritesX, (ySlot - 1f) / numSpritesY);

            // Top left of tile in atlas
            uvArray.Add(topLeft);

            // Top right of tile in atlas
            uvArray.Add(topRight);

            // Bottom right of tile in atlas
            uvArray.Add(bottomRight);

            //Bottom left of tile in atlas
            uvArray.Add(bottomLeft);
        }

        /// <summary>
        /// Adds an end arrow UV coordinate set but rotates the vertex
        /// order based on what orientation the arrow is meant to be. 
        /// </summary>
        /// <param name="type"></param>
        private void AddEndArrow(PathArrowTileEnum type)
        {
            float xSlot = SPRITE_END % numSpritesX;
            float ySlot = numSpritesY - ((int)(SPRITE_END / numSpritesX));

            Vector2 topLeft = new Vector2(xSlot / numSpritesX, ySlot / numSpritesY);
            Vector2 topRight = new Vector2((xSlot + 1f) / numSpritesX, ySlot / numSpritesY);
            Vector2 bottomRight = new Vector2((xSlot + 1f) / numSpritesX, (ySlot - 1f) / numSpritesY);
            Vector2 bottomLeft = new Vector2(xSlot / numSpritesX, (ySlot - 1f) / numSpritesY);

            if (type == PathArrowTileEnum.EndArrowDown)
            {
                uvArray.Add(topLeft);
                uvArray.Add(topRight);
                uvArray.Add(bottomRight);
                uvArray.Add(bottomLeft);
                return;
            }

            if (type == PathArrowTileEnum.EndArrowUp)
            {
                uvArray.Add(bottomRight);
                uvArray.Add(bottomLeft);
                uvArray.Add(topLeft);
                uvArray.Add(topRight);
                return;
            }

            if (type == PathArrowTileEnum.EndArrowLeft)
            {
                uvArray.Add(bottomLeft);
                uvArray.Add(topLeft);
                uvArray.Add(topRight);
                uvArray.Add(bottomRight);
                return;
            }

            uvArray.Add(topRight);
            uvArray.Add(bottomRight);
            uvArray.Add(bottomLeft);
            uvArray.Add(topLeft);
        }

        /// <summary>
        /// Determines what pathing arrow sprite we should use based on the
        /// context of the current position, previous, and next. 
        /// </summary>
        private PathArrowTileEnum GetArrowTileType(
            PathArrowTileNode curr, PathArrowTileNode prev, PathArrowTileNode next)
        {
            if (next == null)
            {
                // Vertical
                if (prev.x == curr.x)
                {
                    // Downward movement
                    if (curr.y > prev.y)
                    {
                        return PathArrowTileEnum.EndArrowDown;
                    }

                    // otherwise assume up
                    return PathArrowTileEnum.EndArrowUp;
                }
                // Horizontal
                if (curr.x > prev.x)
                {
                    return PathArrowTileEnum.EndArrowRight;
                }

                return PathArrowTileEnum.EndArrowLeft;
            }

            // No previous? then we don't need to worry about elbow pieces
            if (prev == null)
            {
                // No delta on X? then it's a vertical
                if (curr.x == next.x)
                {
                    return PathArrowTileEnum.Vertical;
                }

                return PathArrowTileEnum.Horizontal;
            }

            // No delta on X? then it's a vertical
            if (curr.x == next.x && curr.x == prev.x)
            {
                return PathArrowTileEnum.Vertical;
            }

            // All the Y's line up then we got a horiz
            if (curr.y == next.y && curr.y == prev.y)
            {
                return PathArrowTileEnum.Horizontal;
            }

            //Handle the vertical to horizontal transitions
            if (curr.x == prev.x && // Make sure it's vertical
                next.y == curr.y)   // Make sure it's horizontal
            {
                // Handle the going up and then turning
                if (curr.y < prev.y) 
                {
                    // If it's going to the right
                    if (next.x > curr.x)
                    {
                        return PathArrowTileEnum.TopToRight;
                    }

                    // otherwise assume we're going to the left
                    return PathArrowTileEnum.TopToLeft;
                }
                // Handle the going south and turning
                // If it's going to the right
                if (next.x > curr.x)
                {
                    return PathArrowTileEnum.BottomToRight;
                }
                // otherwise assume we're going to the left
                return PathArrowTileEnum.BottomToLeft;
            }

            // Handle the horizontal to vertical transitions
            if (curr.y == prev.y && 
                curr.x == next.x)
            {
                // Handle it going to the right
                if (curr.x > prev.x)
                {
                    // It's turned upward
                    if (next.y > curr.y)
                    {
                        return PathArrowTileEnum.BottomToLeft;
                    }

                    return PathArrowTileEnum.TopToLeft;
                }
                // Assume it's going to the left
                // It's turned upward
                if (next.y > curr.y)
                {
                    return PathArrowTileEnum.BottomToRight;
                }
                return PathArrowTileEnum.TopToRight;
            }

            return PathArrowTileEnum.Horizontal;
        }

    }
}