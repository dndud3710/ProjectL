using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartMesh
{
    private List<Vector3> Vertices = new List<Vector3>();
    private List<Vector3> Normals = new List<Vector3>();
    private List<List<int>> Triangles = new List<List<int>>();
    private List<Vector2> UVs = new List<Vector2>();

    Bounds bounds = new Bounds();

    public void AddTriangle(int subMesh, Vector3 vert1, Vector3 vert2, Vector3 vert3,
                            Vector3 normal1, Vector3 normal2, Vector3 normal3,
                            Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        Triangles[subMesh].Add(Vertices.Count);
        Vertices.Add(vert1);
        Triangles[subMesh].Add(Vertices.Count);
        Vertices.Add(vert2);
        Triangles[subMesh].Add(Vertices.Count);
        Vertices.Add(vert3);

        Normals.Add(normal1);
        Normals.Add(normal2);
        Normals.Add(normal3);

        UVs.Add(uv1);
        UVs.Add(uv2);
        UVs.Add(uv3);
    }

    public void MakeObject()
    {
        GameObject obj = new GameObject();

        var mesh = new Mesh();

        mesh.vertices = Vertices.ToArray();
        mesh.normals = Normals.ToArray();
        mesh.uv = UVs.ToArray();

        for (int i = 0; i < Triangles.Count; i++)
        {
            mesh.SetTriangles(Triangles[i], i, true);
        }
        bounds = mesh.bounds;

        var renderer = obj.AddComponent<MeshRenderer>();
        // renderer.materials = original.GetComponent<MeshRenderer>().materials;

        var filter = obj.AddComponent<MeshFilter>();
        filter.mesh = mesh;

    }
    // revert commit을 위한 임시 주석
}
