using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SnakeMesh : MonoBehaviour {

    [SerializeField]
    private float MeshWidth = 1.0f;
    [SerializeField]
    private int NumVertices = 1000;
    [SerializeField]
    private float UpdateFrequency = 0.25f;
	[SerializeField]
	private float TurnAngle = 0.5f;
	[SerializeField]
	private float DistanceFactor = 100.0f;
	[SerializeField]
	private float SnakeLength = 10.0f;

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
		if (Input.GetKey(KeyCode.A))
			MoveMesh(TurnAngle, DistanceFactor);
		else if (Input.GetKey(KeyCode.D))
			MoveMesh(-TurnAngle, DistanceFactor);
		else
			MoveMesh(0, DistanceFactor);
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

	void MoveMesh(float angle, float distance)
    {
		float moveDistance = 0;
        Vector3[] vertices = mesh.vertices;
		Vector3[] lastTwoFrozenVertices = GetLastTwoFrozenVertices();

		Vector3 midPoint = (lastTwoFrozenVertices[0] + lastTwoFrozenVertices[1]) / 2;

        for(int i=vertices.Length - 1; i > 1 + TimesExtended * 2; i-=2)
        {
			vertices[i-1] = RotatePointAroundPivot(vertices[i-1], midPoint, new Vector3(0, 0, angle));
			vertices[i] = RotatePointAroundPivot(vertices[i], midPoint, new Vector3(0, 0, angle));

			Vector3 pdir = Vector3.Normalize(vertices[i-1] - vertices[i]);
			Vector3 newPDir = Vector3.Normalize(Quaternion.Euler(0, 0, -90) * pdir);

			vertices[i-1] += newPDir / distance;
			vertices[i] += newPDir / distance;

			moveDistance = (newPDir / distance).magnitude;
        }

		MoveDistance += moveDistance;
        if (MoveDistance >= UpdateFrequency)
        {
            MoveDistance = 0;
            TimesExtended++;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

	Vector3[] GetLastTwoFrozenVertices() {
		if(TimesExtended * 2 + 1 < mesh.vertices.Length)
			return new Vector3[2]{ mesh.vertices[TimesExtended * 2], mesh.vertices[TimesExtended * 2 + 1] };
		else 
			return new Vector3[2]{ mesh.vertices[mesh.vertices.Length-1], mesh.vertices[mesh.vertices.Length-2] };
	}

	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angles) * dir;
		point = dir + pivot;
		return point;
	}

	float GetMeshLength() {
		Vector3[] vertices = mesh.vertices;
		float distance = 0;

		for(int i=0; i < vertices.Length - 2; i+=2)
			distance += Vector3.Distance(vertices[i], vertices[i+2]);

		return distance;
	}
}