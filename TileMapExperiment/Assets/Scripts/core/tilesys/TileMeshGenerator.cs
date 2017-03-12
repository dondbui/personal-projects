/// ---------------------------------------------------------------------------
/// TileMeshGenerator.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 7th, 2017</date>
/// ---------------------------------------------------------------------------
 
using UnityEngine;

namespace core.tilesys
{
    /// <summary>
    /// 
    /// </summary>
    public class TileMeshGenerator
    {
        private int NUM_TILES_X = 2;
        private int NUM_TILES_Y = 2;

        private MapData mapData;

        public TileMeshGenerator(MapData mapData)
        {
            this.mapData = mapData;

            NUM_TILES_X = this.mapData.GetWidth();
            NUM_TILES_Y = this.mapData.GetHeight();

            GenerateMesh();
        }

        // method to generate the mesh
        private void GenerateMesh()
        {
            GameObject mapMesh = new GameObject();
            mapMesh.name = "MapMesh";

            mapMesh.transform.rotation = Quaternion.AngleAxis(180, Vector3.right);

            int numTiles = NUM_TILES_X * NUM_TILES_Y;
            int numTriangles = numTiles * 6;
            int numVerts = numTiles * 4;

            Vector3[] vertices = new Vector3[numVerts];
            Vector2[] UVArray = new Vector2[numVerts];

            int x, y, iVertCount = 0;
            for (x = 0; x < NUM_TILES_X; x++)
            {
                for (y = 0; y < NUM_TILES_Y; y++)
                {
                    vertices[iVertCount + 0] = new Vector3(x, y, 0); //  top left
                    vertices[iVertCount + 1] = new Vector3(x + 1, y, 0); // top right
                    vertices[iVertCount + 2] = new Vector3(x + 1, y + 1, 0); // bottom right
                    vertices[iVertCount + 3] = new Vector3(x, y + 1, 0); // bottom left
                    iVertCount += 4;
                }
            }

            int[] triangles = new int[numTriangles];

            int iIndexCount = 0; iVertCount = 0;
            for (int i = 0; i < numTiles; i++)
            {
                triangles[iIndexCount + 0] += (iVertCount + 0);
                triangles[iIndexCount + 1] += (iVertCount + 1);
                triangles[iIndexCount + 2] += (iVertCount + 2);
                triangles[iIndexCount + 3] += (iVertCount + 0);
                triangles[iIndexCount + 4] += (iVertCount + 2);
                triangles[iIndexCount + 5] += (iVertCount + 3);

                iVertCount += 4; iIndexCount += 6;
            }

            MeshFilter meshFilter = mapMesh.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;

            Material mat = Resources.Load<Material>("Materials/Tiles2");

            MeshRenderer mr = mapMesh.AddComponent<MeshRenderer>();
            mr.material = mat;


            iVertCount = 0;

            for (x = 0; x < NUM_TILES_X; x++)
            {
                for (y = 0; y < NUM_TILES_Y; y++)
                {
                    string tileID = mapData.GetTileAt(x, y);
                    int tileNum = int.Parse(tileID);

                    UVArray[iVertCount + 0] = new Vector2(tileNum/6f, 0); //Top left of tile in atlas
                    UVArray[iVertCount + 1] = new Vector2((tileNum + 1) / 6f, 0); //Top right of tile in atlas
                    UVArray[iVertCount + 2] = new Vector2((tileNum + 1) / 6f, 1); //Bottom right of tile in atlas
                    UVArray[iVertCount + 3] = new Vector2(tileNum / 6f, 1); //Bottom left of tile in atlas

                    iVertCount += 4;
                }
            }

            meshFilter.mesh.uv = UVArray;
        }
    }
}