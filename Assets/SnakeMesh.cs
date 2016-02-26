using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SnakeMesh : MonoBehaviour {

    private MeshFilter meshFilter;
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> triangles;

	void Start () {
        mesh = new Mesh();
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 1, 0));

        Vector3 last_1 = vertices[vertices.Count - 1];
        Vector3 last_2 = vertices[vertices.Count - 2];

        normals.Add(-Vector3.forward);
        normals.Add(-Vector3.forward);
        normals.Add(-Vector3.forward);
        normals.Add(-Vector3.forward);
        normals.Add(-Vector3.forward);
        normals.Add(-Vector3.forward);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 1));

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(1);

        triangles.Add(2);
        triangles.Add(4);
        triangles.Add(3);
        triangles.Add(4);
        triangles.Add(5);
        triangles.Add(3);

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    void Update()
    {
        Vector3[] v = mesh.vertices;

        v[v.Length - 1] += new Vector3(0, 0.01f, 0);
        v[v.Length - 2] += new Vector3(0, 0.01f, 0);

        mesh.vertices = v;
        mesh.RecalculateBounds();

        // Add 2 more vertices to the mesh
        if (GetVertexDistance() >= 1.0f)
        {
            ExtendMesh();
        }
    }

    void ExtendMesh()
    {
        Vector3[] v = mesh.vertices;
        Vector3[] n = mesh.normals;
        Vector2[] u = mesh.uv;
        int[] t = mesh.triangles;

        Array.Resize(ref n, n.Length + 2);
        Array.Resize(ref u, u.Length + 2);
        Array.Resize(ref t, t.Length + 6);

        n[n.Length - 2] = -Vector3.forward;
        n[n.Length - 1] = -Vector3.forward;

        u[u.Length - 2] = new Vector2(1, 1);
        u[u.Length - 1] = new Vector2(1, 1);

        t[t.Length - 6] = t[t.Length - 13] + 2;
        t[t.Length - 5] = t[t.Length - 12] + 1;
        t[t.Length - 4] = t[t.Length - 11] + 2;
        t[t.Length - 3] = t[t.Length - 10] + 2;
        t[t.Length - 2] = t[t.Length - 9] + 1;
        t[t.Length - 1] = t[t.Length - 8] + 2;

        // vertices
        Vector3 v1 = v[v.Length - 1];
        Vector3 v2 = v[v.Length - 2];

        Array.Resize(ref v, v.Length + 2);

        v[v.Length - 2] = v1;
        v[v.Length - 1] = v2;

        mesh.vertices = v;
        mesh.normals = n;
        mesh.uv = u;
        mesh.triangles = t;
        mesh.RecalculateBounds();
    }

    float GetVertexDistance()
    {
        Vector3[] vertices = mesh.vertices;
        return Vector3.Distance(vertices[vertices.Length - 1], vertices[vertices.Length - 3]);
    }
}