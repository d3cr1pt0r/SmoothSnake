using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SnakeMesh : MonoBehaviour {

    [SerializeField]
    private float MeshWidth;
    [SerializeField]
    private int NumVertices;
    [SerializeField]
    private float UpdateFrequency;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> triangles;

    private int TimesExtended;
    private float MoveDistance;

	void Start () {
        TimesExtended = 0;
        GenerateInitialMesh(NumVertices);
    }

    void Update()
    {
		MoveMesh(new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), 0.01f, 0f));
    }

	void GenerateInitialMesh(int numVerticies)
	{
		mesh = new Mesh();
		vertices = new List<Vector3>(numVerticies);
		normals = new List<Vector3>(numVerticies);
		uvs = new List<Vector2>(numVerticies);
		triangles = new List<int>();

		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		// Create the first quad
		vertices.Add(new Vector3(0, 0, 0));
		vertices.Add(new Vector3(MeshWidth, 0, 0));
		vertices.Add(new Vector3(0, 0, 0));
		vertices.Add(new Vector3(MeshWidth, 0, 0));

		normals.Add(-Vector3.forward);
		normals.Add(-Vector3.forward);
		normals.Add(-Vector3.forward);
		normals.Add(-Vector3.forward);

		uvs.Add(new Vector2(0, 0));
		uvs.Add(new Vector2(1, 0));
		uvs.Add(new Vector2(0, 1 / NumVertices));
		uvs.Add(new Vector2(1, 1 / NumVertices));

		triangles.Add(0);
		triangles.Add(2);
		triangles.Add(1);

		triangles.Add(2);
		triangles.Add(3);
		triangles.Add(1);

		// Create vertex buffer in advance
		for (int i=0; i < numVerticies - 4; i++)
		{
			if (i < numVerticies / 2)
			{
				vertices.Add(new Vector3(0, 0, 0));
				vertices.Add(new Vector3(MeshWidth, 0, 0));

				normals.Add(-Vector3.forward);
				normals.Add(-Vector3.forward);

				uvs.Add(new Vector2(1, 1));
				uvs.Add(new Vector2(1, 1));

				triangles.Add(triangles[triangles.Count - 6] + 2);
				triangles.Add(triangles[triangles.Count - 6] + 2);
				triangles.Add(triangles[triangles.Count - 6] + 2);

				triangles.Add(triangles[triangles.Count - 6] + 2);
				triangles.Add(triangles[triangles.Count - 6] + 2);
				triangles.Add(triangles[triangles.Count - 6] + 2);
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
	}

    void MoveMesh(Vector3 dir)
    {
        Vector3[] vertices = mesh.vertices;

        for(int i=vertices.Length - 1; i > 3 + TimesExtended * 2; i--)
        {
            vertices[i] += dir;
        }

        MoveDistance += dir.magnitude;
        if (MoveDistance >= UpdateFrequency)
        {
            MoveDistance = 0;
            TimesExtended++;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}