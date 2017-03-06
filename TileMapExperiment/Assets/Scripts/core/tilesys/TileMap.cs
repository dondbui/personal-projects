/**
 * TileMap.cs
 * 
 * <author>Don Duy Bui</author>
 * <date>March 5th, 2017</date>
 */

using UnityEngine;

/// <summary>
/// Handles the initialization of a tile map
/// </summary>
public class TileMap : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("Create Tile Map");
        BuildMesh();
    }

    private void BuildMesh()
    {
        // Create a new mesh
        Vector3[] vertices = new Vector3[4];

        // 2 triangle which have 3 verts each.
        int[] triangles = new int[2 * 3];

        Vector3[] normals = new Vector3[4];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 0, -1);
        vertices[3] = new Vector3(1, 0, -1);

        triangles[0] = 0;
        triangles[1] = 3;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 1;
        triangles[5] = 3;

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshColider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
    }

}
