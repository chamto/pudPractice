using UnityEngine;
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

			if(Input.GetMouseButtonUp(0))
			//if (Input.touchCount > 0) 
			{
				//if (Input.GetTouch (0).phase == TouchPhase.Ended) 
				{
					CallBackTouchEnded();
				}
			}
		}

		void CallBackTouchEnded()
		{
			//Vector3 pos =  Camera.main.ScreenToWorldPoint (Input.GetTouch(0).position);
			Vector3 pos =  Camera.main.ScreenToWorldPoint (Input.mousePosition);
			
			int HexPosX = 0, HexPosY = 0;
			
			
			//GetHex (pos.x, pos.y, out HexPosY, out HexPosX);
			PointToHex (pos.x, pos.y, out HexPosX, out HexPosY);

			Debug.Log ("WorldPos : " + pos + "  HexPos : " + HexPosX + " " + HexPosY);
		}

		//ref : http://gamedev.stackexchange.com/questions/20742/how-can-i-implement-hexagonal-tilemap-picking-in-xna
		void GetHex(float x, float y, out int row, out int column)
		{
			float a = hexCircumScr;
			float b = hexInScr;
			float c = hexBase;

			// Find out which major row and column we are on:
			row = (int)(y / b);
			column = (int)(x / (a + c));
			
			// Compute the offset into these row and column:
			float dy = y - (float)row * b;
			float dx = x - (float)column * (a + c);
			
			// Are we on the left of the hexagon edge, or on the right?
			if (((row ^ column) & 1) == 0)
				dy = b - dy;
			int right = dy * (a - c) < b * (dx - c) ? 1 : 0;
			
			// Now we have all the information we need, just fine-tune row and column.
			row += (column ^ row ^ right) & 1;
			column += right;
		}

		//ref : http://www-cs-students.stanford.edu/~amitp/Articles/GridToHex.html
		void PointToHex( float xp, float yp ,out int hexX , out int hexY)
		{

			// NOTE:  HexCoord(0,0)'s x() and y() just define the origin
			//        for the coordinate system; replace with your own
			//        constants.  (HexCoord(0,0) is the origin in the hex
			//        coordinate system, but it may be offset in the x/y
			//        system; that's why I subtract.)
			float x = xp / (hexCircumScr + hexBase);
			float y =  yp / (hexInScr *2);
			float z = -0.5f * x - y;
			y = -0.5f * x + y;
			int ix = Mathf.FloorToInt(x+0.5f);
			int iy = Mathf.FloorToInt(y+0.5f);
			int iz = Mathf.FloorToInt(z+0.5f);
			int s = ix+iy+iz;
			if( 0 != s )
			{
				float abs_dx = Mathf.Abs(ix-x);
				float abs_dy = Mathf.Abs(iy-y);
				float abs_dz = Mathf.Abs(iz-z);
				if( abs_dx >= abs_dy && abs_dx >= abs_dz )
					ix -= s;
				else if( abs_dy >= abs_dx && abs_dy >= abs_dz )
					iy -= s;
				else
					iz -= s;
			}
			hexX = ix;
			hexY = (iy - iz + (1-ix%2) ) / 2;
			//return HexCoord( ix, ( iy - iz + (1-ix%2) ) / 2 );
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
			lineRenderer.SetWidth (0.1F, 0.1F);
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
