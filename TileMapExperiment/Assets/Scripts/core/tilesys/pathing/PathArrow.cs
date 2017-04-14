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
    public class PathArrow
    {
        private int TILE_SIZE = 1;

        private GameObject arrowObj;
        private Mesh arrowMesh;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvArray = new List<Vector2>();
        List<int> triangles = new List<int>();

        public PathArrow(Vector2 startPos, Vector2 endPos)
        {
            arrowObj = new GameObject();
            arrowObj.transform.rotation = Quaternion.AngleAxis(180, Vector3.right);
            MeshFilter meshFilter = arrowObj.AddComponent<MeshFilter>();

            Material mat = Resources.Load<Material>("Materials/occupied");
            MeshRenderer mr = arrowObj.AddComponent<MeshRenderer>();
            mr.material = mat;

            arrowMesh = new Mesh();
            arrowMesh.MarkDynamic();

            arrowObj.name = "pathArrow";

            Update(startPos, endPos);

            meshFilter.mesh = arrowMesh;
        }

        public void Update(Vector2 startPos, Vector2 endPos)
        {
            Clear();

            int xDelta = Mathf.RoundToInt(endPos.x - startPos.x);
            int yDelta = Mathf.RoundToInt(endPos.y - startPos.y);

            int xIncre = xDelta > 0 ? 1 : -1;
            int yIncre = yDelta > 0 ? 1 : -1;

            int x = (int)startPos.x;
            int y = (int)startPos.y;

            // mark the mid point
            AddTile(x, y + yDelta, false);

            // draw the destination
            AddTile(x + xDelta, y + yDelta, true);

            // draw the vertical
            int end = Math.Abs(yDelta);
            for (int i = 0; i <= end; i++)
            {
                AddTile(x, y + i * yIncre, false);
            }

            end = Math.Abs(xDelta);
            for (int i = 0; i <= end; i++)
            {
                AddTile(x + i * xIncre, y + yDelta, false);
            }

            arrowMesh.vertices = vertices.ToArray();
            arrowMesh.triangles = triangles.ToArray();
        }

        public void Clear()
        {
            vertices.Clear();
            triangles.Clear();
            uvArray.Clear();

            arrowMesh.Clear(false);
        }

        private void AddTile(int x, int y, bool isFinal)
        {
            AddVerticesAtTilePos(x, y);
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
    }
}