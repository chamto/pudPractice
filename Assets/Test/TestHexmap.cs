﻿using UnityEngine;
using System.Collections;

public class TestHexmap : MonoBehaviour
{

//ref : http://gamedev.stackexchange.com/questions/20742/how-can-i-implement-hexagonal-tilemap-picking-in-xna
//
//	             Y ^
//		           |
//		  [0]  ____|____  [1]
//			  /  b |   |\
//			 /     |   | \
//		[5]	/      |   |  \  [2]
//	    ---(-------+---+---)------>
//			\     O|   c  / a      X
//			 \     |     /
//		 [4]  \____|____/  [3]
//			       |

//	      /|
//    a' / |
//	    /  | b
//     /   |
//    (----ㅁ
//	     c

		//"a" is the radius of circumscribed that hexagon
		//"b" is the radius of inscribed that hexagon
		//"c" is the base or the top edge
		// if a == a' that  The regular hexagon  and  [a' : b : c = 2 : root3 : 1]
		public float hexCircumScr;	// "a"
		public float hexInScr;		// "b"
		public float hexBase ;		// "c"
		public float hexHypotenuse; //  a' 

		public Vector3 setPosHexagon;
		
		private GameObject m_hexagonMap = null;

		// Use this for initialization
		void Start ()
		{
			m_hexagonMap = GameObject.Find ("HexagonMap");
			if(null == m_hexagonMap)
				m_hexagonMap = new GameObject("HexagonMap");

			hexCircumScr = 2f;
			hexInScr = 1.72f;
			hexBase = 1.0f;
			hexHypotenuse = Mathf.Sqrt (hexBase * hexBase + hexInScr * hexInScr);

			//AddHexagon (Vector3.zero);

			//-----------------------------------------
			//setting hexagon map 
			//-----------------------------------------
			setPosHexagon = Vector3.zero;

			for (int y=0; y<10; y++) 
			{
				//setPosHexagon.y = hexInScr * y; 
				for(int x=0;x<10;x++)
				{
					setPosHexagon.y = hexInScr * (y*2 +(x%2)); 
					setPosHexagon.x = (hexCircumScr + hexBase) * x;
					AddHexagon(setPosHexagon);
				}
			}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		int AddHexagon (Vector3 setPos)
		{
			Vector3 pos = Vector3.zero;
			
			GameObject hex = new GameObject ("hex");
			LineRenderer lineRenderer = hex.AddComponent<LineRenderer> (); //not multiple add !!
			hex.transform.parent = m_hexagonMap.transform;
			//MonoBehaviour.Instantiate (lineRenderer.gameObject);
			if (null == lineRenderer)
						Debug.LogError ("lineRenderer is null");

			lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
			lineRenderer.SetColors (Color.blue, Color.blue);
			lineRenderer.SetWidth (0.2F, 0.2F);
			lineRenderer.SetVertexCount (7);

			pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
			lineRenderer.SetPosition (0,pos + setPos);

			pos.x = hexBase; pos.y = hexInScr; pos.z = 0;
			lineRenderer.SetPosition (1,pos + setPos);

			pos.x = hexCircumScr; pos.y = 0; pos.z = 0;
			lineRenderer.SetPosition (2,pos + setPos);

			pos.x = hexBase; pos.y = -hexInScr; pos.z = 0;
			lineRenderer.SetPosition (3,pos + setPos);
		
			pos.x = -hexBase; pos.y = -hexInScr; pos.z = 0;
			lineRenderer.SetPosition (4,pos + setPos);

			pos.x = -hexCircumScr; pos.y = 0; pos.z = 0;
			lineRenderer.SetPosition (5,pos + setPos);

			pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
			lineRenderer.SetPosition (6,pos + setPos);

			return lineRenderer.GetInstanceID ();
			
		}



	#if UNITY_EDITOR
	void OnGUI()
	{
		if (GUI.Button (new Rect (10, 10, 100, 70), "update")) 
		{

			hexHypotenuse = Mathf.Sqrt (hexBase * hexBase + hexInScr * hexInScr);
			


			Debug.Log ("Add hexagon that Id : " + AddHexagon (setPosHexagon));
		}
		

	}
	#endif

	
		/// <summary>
		/// test line render
		/// ref : http://www.andrewnoske.com/wiki/Unity_-_Mesh
		/// </summary>
		//	private Mesh mesh;
		//	private Material mat;
		//
		//	public Vector3 m_linePos;
		//	
		//	private Vector3 linePos
		//	{
		//		get
		//		{
		//			return m_linePos;
		//		}
		//
		//		set 
		//		{
		//
		//			if(null != mesh.vertices)
		//			{
		//				Vector3[] vertices = mesh.vertices;
		//				for(int i=0;i < mesh.vertexCount; i++)
		//				{
		//					//mesh.vertices.SetValue(Vector3.zero,i);
		//					//mesh.vertices[i] += value;
		//					vertices[i] += value;
		//					Debug.Log ("mesh.vertices "+i+": "+ mesh.vertices[i]);
		//				}
		//				mesh.vertices = vertices;
		//				
		//			}
		//		}
		//	}
		//	void Start2 ()
		//	{
		//		
		//		
		//		mat = new Material(Shader.Find("Self-Illumin/VertexLit"));
		//		mat.color = new Color(0.6f,0.3f,0);
		//		
		//		mesh = new Mesh();
		//		
		//		Vector3[] newVertices  = new Vector3[4];  // Setup vertices.
		//		newVertices[0] = new Vector3(0,0,0);
		//		newVertices[1] = new Vector3(0,5,0);
		//		newVertices[2] = new Vector3(5,5,0);
		//		newVertices[3] = new Vector3(5,0,0);
		//		
		//		//newVertices[3] = new Vector3(0,0,0);
		//		//newVertices[4] = new Vector3(5,5,0);
		//		//newVertices[5] = new Vector3(5,0,0);
		//		
		//		int[] newTriangles = new int[6];      // Map these 3 verticies into one triangle.
		//		newTriangles[0] = 0;
		//		newTriangles[1] = 1;
		//		newTriangles[2] = 2;
		//		
		//		newTriangles[3] = 0;
		//		newTriangles[4] = 2;
		//		newTriangles[5] = 3;
		//		
		//		//		newTriangles[3] = 3;
		//		//		newTriangles[4] = 4;
		//		//		newTriangles[5] = 5;
		//		
		//		mesh.vertices = newVertices;
		//		
		//		mesh.triangles = newTriangles;
		//		//mesh.uv       = newTextureCoord;  // For texture mapping using a Vector2[].
		//		//mesh.normals  = newNormal;        // For configuring normals using a Vector3[].
		//		//mesh.colors   = newColorVert;     // Vary color by vertex using a Color[].
		//		mesh.RecalculateBounds();
		//		mesh.RecalculateNormals();
		//		
		//	}
		//	void Update2()
		//	{
		//		
		//		
		//		Graphics.DrawMesh(mesh, transform.localToWorldMatrix, mat, 0);
		//	}

}
