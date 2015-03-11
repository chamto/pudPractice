using UnityEngine;
using System.Collections;

public class ExampleMesh : MonoBehaviour {


	//ref : http://docs.unity3d.com/ScriptReference/Mesh.html

	public Vector3[] newVertices;
	public Vector2[] newUV;
	public int[] newTriangles;

	void Start2() 
	{
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;

	}


	void Update() 
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		//Debug.Log(vertices.Length); //chamto test
		int i = 0;
		while (i < vertices.Length) {
			vertices[i] += normals[i] * Mathf.Sin(Time.time);
			//Debug.Log(vertices[i]); //chamto test
			i++;
		}
		mesh.vertices = vertices;
	}


}
