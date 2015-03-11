using UnityEngine;
using System.Collections;


public class TestHexmap : MonoBehaviour 
{

//ref : http://gamedev.stackexchange.com/questions/20742/how-can-i-implement-hexagonal-tilemap-picking-in-xna
//
//	             Y ^
//		           |
//			   ____|____
//			  /  b |   |\
//			 /     |   | \
//			/      |   |  \
//	    ---(-------+---+---)------>
//			\     O|   c  / a      X
//			 \     |     /
//			  \____|____/
//			       |

//	      /|
//     a / |
//	    /  | b
//     /   |
//    (----ㅁ
//	     c

	//"a" is the radius of circumscribed that hexagon
	//"b" is the radius of inscribed that hexagon
	//"c" is the base or the top edge
	public float hexCircumScr;	// "a"
	public float hexInScr;		// "b"
	public float hexBase;		// "c"


	// Use this for initialization
	void Start1 () 
	{
	
	}
	
	// Update is called once per frame
	void Update1 () 
	{
	
	}


	/// <summary>
	/// test line render
	/// ref : http://www.andrewnoske.com/wiki/Unity_-_Mesh
	/// </summary>
	private Mesh mesh;
	private Material mat;

	public Vector3 m_linePos;
	
	private Vector3 linePos
	{
		get
		{
			return m_linePos;
		}

		set 
		{

			if(null != mesh.vertices)
			{
				Vector3[] vertices = mesh.vertices;
				for(int i=0;i < mesh.vertexCount; i++)
				{
					//mesh.vertices.SetValue(Vector3.zero,i);
					//mesh.vertices[i] += value;
					vertices[i] += value;
					Debug.Log ("mesh.vertices "+i+": "+ mesh.vertices[i]);
				}
				mesh.vertices = vertices;
				
			}
		}
	}

	
	void Start ()
	{
		mat = new Material(Shader.Find("Self-Illumin/VertexLit"));
		mat.color = new Color(0.6f,0.3f,0);
		
		mesh = new Mesh();
		
		Vector3[] newVertices  = new Vector3[4];  // Setup vertices.
		newVertices[0] = new Vector3(0,0,0);
		newVertices[1] = new Vector3(0,5,0);
		newVertices[2] = new Vector3(5,5,0);
		newVertices[3] = new Vector3(5,0,0);

		//newVertices[3] = new Vector3(0,0,0);
		//newVertices[4] = new Vector3(5,5,0);
		//newVertices[5] = new Vector3(5,0,0);
		
		int[] newTriangles = new int[6];      // Map these 3 verticies into one triangle.
		newTriangles[0] = 0;
		newTriangles[1] = 1;
		newTriangles[2] = 2;

		newTriangles[3] = 0;
		newTriangles[4] = 2;
		newTriangles[5] = 3;
		
//		newTriangles[3] = 3;
//		newTriangles[4] = 4;
//		newTriangles[5] = 5;
		
		mesh.vertices = newVertices;

		mesh.triangles = newTriangles;
		//mesh.uv       = newTextureCoord;  // For texture mapping using a Vector2[].
		//mesh.normals  = newNormal;        // For configuring normals using a Vector3[].
		//mesh.colors   = newColorVert;     // Vary color by vertex using a Color[].
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

	}
	
	
	void Update()
	{


		Graphics.DrawMesh(mesh, transform.localToWorldMatrix, mat, 0);
	}

	#if UNITY_EDITOR
	void OnGUI()
	{
		if (GUI.Button (new Rect (10, 10, 100, 70), "update")) 
		{
			linePos = m_linePos;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();

			Debug.Log ("pos update!!!");
		}
		

	}
	#endif
}
