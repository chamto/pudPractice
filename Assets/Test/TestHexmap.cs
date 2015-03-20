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
//
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
		//InitHexMap_Analysis_GetHex ();
		InitHexMap_Analysis_PointToHex ();
		//InitHexMap_Analysis_PositionXYToHex ();
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
		//PositionXYToHex (pos.x, pos.y, out HexPosX, out HexPosY);



		Debug.Log ("----->WorldPos : " + pos + "  HexPos : " + HexPosX + " " + HexPosY);
	}


	void InitHexMap_Analysis_PositionXYToHex()
	{
		GameObject obj;
		Vector3 pos = Vector3.zero;
		for (int y=-1; y<10; y++) 
		{
			//pos.y = hexInScr * y; 
			for(int x=-1;x<10;x++)
			{
				pos.x = (hexCircumScr + hexBase) * x;
				pos.y = hexInScr * (y*2 +(x%2)); 
				obj = AddHexagon(pos);
				obj = AddQuad_Center(pos,Color.cyan,(hexCircumScr+hexBase),hexInScr*2);
				Add3DText(obj.transform,x + "," + y, Color.white , Vector3.zero);
			}
		}
	}

	void InitHexMap_Analysis_GetHex()
	{
		GameObject obj;
		Vector3 pos = Vector3.zero;
		for (int y=-1; y<10; y++) 
		{
			//pos.y = hexInScr * y; 
			for(int x=-1;x<10;x++)
			{
				pos.x = (hexCircumScr + hexBase) * x;
				pos.y = hexInScr * (y*2 +(x%2)); 
				obj = AddHexagon(pos);
				//obj = AddQuadrangle(pos);
				Add3DText(obj.transform,x + "," + (y*2+(x%2)), Color.white , Vector3.zero);
			}
		}

		//----------------------------
		// GetHex GuidLine
		//----------------------------
		pos = Vector3.zero;
		for (int y=-2; y<10; y++) 
		{
			for (int x=-2; x<10; x++) 
			{
				pos.y = hexInScr * y;
				pos.x = (hexCircumScr + hexBase) * x;


				if (((x ^ y) & 1) == 0)
				{
					obj = AddQuad_LiftBottom(pos,Color.red,hexCircumScr+hexBase-0.5f, hexInScr-0.5f);
					//Debug.Log ("true-----column : " + x + " row : " + y);
				}
				else
				{
					obj = AddQuad_LiftBottom(pos,Color.white,hexCircumScr+hexBase, hexInScr);
					//Debug.Log ("-----column : " + x + " row : " + y);
				}

				Add3DText(obj.transform,x + "," + y, Color.red , new Vector3(0f , hexInScr/2f, 0f));
			}
				
		}

	}



	void InitHexMap_Analysis_PointToHex()
	{
		GameObject obj;
		Vector3 pos = Vector3.zero;
		Vector3 prevPos = Vector3.zero;
		for (int y=-2; y<10; y++) 
		{
			//pos.y = hexInScr * y; 
			for(int x=-2;x<10;x++)
			{
				prevPos = pos;
				pos.x = (hexCircumScr + hexBase) * x;
				pos.y = hexInScr * (y*2 +(x%2)); 
				obj = AddHexagon(pos);
				//obj = AddQuad_Center(pos,Color.cyan,(hexCircumScr+hexBase),hexInScr*2);
				//obj = AddQuadrangle(pos);
				Add3DText(obj.transform,x + "," + y, Color.white , Vector3.zero);

				float cx = pos.x / (hexCircumScr + hexBase);
				float cy =  pos.y / (hexInScr *2);
				float cz = -0.5f * cx - cy;
				cy = -0.5f * cx + cy;
				int ix = Mathf.FloorToInt(cx+0.5f);
				int iy = Mathf.FloorToInt(cy+0.5f);
				int iz = Mathf.FloorToInt(cz+0.5f);
				Add3DText(obj.transform,ix + "," + iy + "," + iz, Color.gray , new Vector3(0,-0.5f,0));

				//obj = AddLine(Color.red, prevPos , pos);
			}
		}
		
		//----------------------------
		// GuidLine
		//----------------------------
		pos = Vector3.zero;
		for (int y=-2; y<10; y++) 
		{
			for (int x=-2; x<10; x++) 
			{
				prevPos = pos;
				pos.x = (hexCircumScr + hexBase) * x;
				pos.y = hexInScr * 2 * y;
				pos.z = -0.5f * x - y;
				pos.y = -0.5f * pos.x + pos.y;
				//obj = AddQuad_LiftBottom(pos,Color.white,hexCircumScr+hexBase, hexInScr*2);
				//Debug.Log ("-----column : " + x + " row : " + y);

				
				//Add3DText(obj.transform,pos.ToString(), Color.red , new Vector3(0f , 0f, 0f));


				//obj = AddLine(Color.red, prevPos , pos);

			}

			
		}

		//----------------------------
		// GuidLine 2
		//----------------------------
		int idxFirst = 0;
		Vector3 textUp = new Vector3 (0, 0.5f, 0);
		pos = Vector3.zero;
		for (int iy=idxFirst; iy<10; iy++) 
		{

			for (int ix=idxFirst; ix<10; ix++) 
			{
				prevPos = pos;
				pos.x = ix * (hexCircumScr + hexBase);
				pos.y = hexInScr * ( 2 * iy + ix );

				if(idxFirst == ix) continue;
				obj = AddLine(Color.red, prevPos , pos);
				Add3DText(obj.transform,"ix:"+ix +" iy:"+iy+" iz", Color.red , pos-prevPos + textUp);
			}

		}

		pos = Vector3.zero;
		for (int iz=idxFirst; iz<10; iz++) 
		{
			
			for (int ix=idxFirst; ix<10; ix++) 
			{
				prevPos = pos;
				pos.x = ix * (hexCircumScr + hexBase);
				pos.y = hexInScr * ( 2 * iz + ix );
				pos.y *= -1f;

				if(idxFirst == ix) continue;
				obj = AddLine(Color.magenta, prevPos , pos);
				Add3DText(obj.transform,"ix:"+ix + " iy iz:"+iz, Color.red , pos-prevPos + textUp);
			}
		}
	}

	//ref : http://gamedev.stackexchange.com/questions/20742/how-can-i-implement-hexagonal-tilemap-picking-in-xna
	void GetHex(float x, float y, out int row, out int column)
	{
		float a = hexCircumScr;
		float b = hexInScr;
		float c = hexBase;

		// Find out which major row and column we are on:
		float adj_row = (y / b);
		float adj_col = (x / (a + c));
		row = adj_row < 0 ? (int)(adj_row + -1.0f) : (int)(adj_row); 
		column = adj_col < 0 ? (int)(adj_col + -1.0f) : (int)(adj_col); 
		//row = (int)(y / b);
		//column = (int)(x / (a + c));
		//Debug.Log ("GetHex << column : " +column+ " :  " +(x/(a+c))+" row : "+row +" :  "+ (y/b)); //chamto test
		
		// Compute the offset into these row and column:
		//  dy = y - "The point by liftBottom on quad : [value scope] row*b < row*(b+1)"
		float dy = y - (float)row * b;
		float dx = x - (float)column * (a + c);
		//dy = Mathf.Abs (dy);
		//dx = Mathf.Abs (dx);
		//Debug.Log ("GetHex << dx : " +dx+ " dy : "+dy); //chamto test

		// Are we on the left of the hexagon edge, or on the right?
		//The value of end bit 0 is number lists : 0   2   4   6   8 .... [even number]
		//The value of end bit 1 is number lists :   1   3   5   7   .... [odd number]
		if (((row ^ column) & 1) == 0) // condition : two value whole odd or even
			dy = b - dy;  //reversal value

		//int right = dy * (a - c) < b * (dx - c) ? 1 : 0;
		int right;
		if(dy * (a - c) < b * (dx - c))
			right =  1;
		else
			right =  0;
		
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
		int ix = Mathf.RoundToInt(x);
		int iy = Mathf.RoundToInt(y);
		int iz = Mathf.RoundToInt(z);
//		int ix = Mathf.FloorToInt(x+0.5f);
//		int iy = Mathf.FloorToInt(y+0.5f);
//		int iz = Mathf.FloorToInt(z+0.5f);
		int s = ix+iy+iz;
		Debug.Log ("PointToHex <<  s : " + s + " ix :" +ix+ " iy :"+iy+" iz :"+iz);
		if( 0 != s )
		{
			float abs_dx = Mathf.Abs(ix-x);
			float abs_dy = Mathf.Abs(iy-y);
			float abs_dz = Mathf.Abs(iz-z);
			Debug.Log ("PointToHex <<  abs_dx : " + abs_dx + " abs_dy : " +abs_dy+ " abs_dz : " +abs_dz);
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


	/// <summary>
	/// 3d x, y축을 기준으로 hexmap의 인덱스를 얻어온다
	/// 육각형 크기에 맞는 사각형으로 계산하는게 특징이다
	/// </summary>
	void PositionXYToHex(float x, float y, out int row, out int column)
	{
		//--------------------------------------
		// Reverse express : position to index
		//--------------------------------------
		//pos.x = (hexCircumScr + hexBase) * x;
		//pos.y = hexInScr * (y*2 +(x%2));
		//--------------------------------------
		row = Mathf.RoundToInt(x / (hexCircumScr + hexBase));
		column = Mathf.RoundToInt((y - hexInScr * (float)(row % 2)) / ( 2f * hexInScr));
	}

	
	GameObject AddLine (Color color, Vector3 setPos , Vector3 nextPos)
	{
			GameObject hex = new GameObject ("line");
			LineRenderer lineRenderer = hex.AddComponent<LineRenderer> (); //not multiple add !!
			hex.transform.parent = m_hexagonMap.transform;
			//MonoBehaviour.Instantiate (lineRenderer.gameObject);
			if (null == lineRenderer)
				Debug.LogError ("lineRenderer is null");
			
			lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
			lineRenderer.SetColors (Color.white, color);
			lineRenderer.SetWidth (0.1F, 0.3F);
			lineRenderer.SetVertexCount (2);
			lineRenderer.useWorldSpace = false;
			lineRenderer.transform.position = setPos;


			lineRenderer.SetPosition (0,Vector3.zero);

			lineRenderer.SetPosition (1,nextPos-setPos);

			return hex;
	}

	GameObject AddHexagon (Vector3 setPos)
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
		lineRenderer.useWorldSpace = false;
		lineRenderer.transform.position = setPos;

		pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (0,pos);

		pos.x = hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (1,pos);

		pos.x = hexCircumScr; pos.y = 0; pos.z = 0;
		lineRenderer.SetPosition (2,pos);

		pos.x = hexBase; pos.y = -hexInScr; pos.z = 0;
		lineRenderer.SetPosition (3,pos);
	
		pos.x = -hexBase; pos.y = -hexInScr; pos.z = 0;
		lineRenderer.SetPosition (4,pos);

		pos.x = -hexCircumScr; pos.y = 0; pos.z = 0;
		lineRenderer.SetPosition (5,pos);

		pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (6,pos);

		return hex;
		
	}


	GameObject AddQuad_LiftBottom(Vector3 liftBottomPos, Color color, float width, float height)
	{
		Vector3 pos = Vector3.zero;
		
		GameObject hex = new GameObject ("Quad2");
		LineRenderer lineRenderer = hex.AddComponent<LineRenderer> (); //not multiple add !!
		hex.transform.parent = m_hexagonMap.transform;
		//MonoBehaviour.Instantiate (lineRenderer.gameObject);
		if (null == lineRenderer)
			Debug.LogError ("lineRenderer is null");
		
		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		lineRenderer.SetColors (color, color);
		lineRenderer.SetWidth (0.2F, 0.2F);
		lineRenderer.SetVertexCount (5);
		lineRenderer.useWorldSpace = false;
		lineRenderer.transform.position = liftBottomPos;
		
		pos.x = 0; pos.y = 0; pos.z = 0;
		lineRenderer.SetPosition (0,pos);
		
		pos.x = 0; pos.y = height; pos.z = 0;
		lineRenderer.SetPosition (1,pos);
		
		pos.x = width; pos.y = height; pos.z = 0;
		lineRenderer.SetPosition (2,pos);
		
		pos.x = width; pos.y = 0; pos.z = 0;
		lineRenderer.SetPosition (3,pos);
		
		pos.x = 0; pos.y = 0; pos.z = 0;
		lineRenderer.SetPosition (4,pos);
		
		return hex;
	}

	GameObject AddQuad_Center (Vector3 setPos, Color color, float width, float height)
	{
		Vector3 pos = Vector3.zero;
		
		GameObject hex = new GameObject ("Quad");
		LineRenderer lineRenderer = hex.AddComponent<LineRenderer> (); //not multiple add !!
		hex.transform.parent = m_hexagonMap.transform;
		//MonoBehaviour.Instantiate (lineRenderer.gameObject);
		if (null == lineRenderer)
			Debug.LogError ("lineRenderer is null");
		
		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		lineRenderer.SetColors (color, color);
		lineRenderer.SetWidth (0.2F, 0.2F);
		lineRenderer.SetVertexCount (5);
		lineRenderer.useWorldSpace = false;
		lineRenderer.transform.position = setPos;
		
		pos.x = -width/2f; pos.y = height/2f; pos.z = 0;
		//pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (0,pos);
		
		pos.x = width/2f; pos.y = height/2f; pos.z = 0;
		//pos.x = hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (1,pos);
		
		pos.x = width/2f; pos.y = -height/2f; pos.z = 0;
		//pos.x = hexBase; pos.y = -hexInScr; pos.z = 0;
		lineRenderer.SetPosition (2,pos);
		
		pos.x = -width/2f; pos.y = -height/2; pos.z = 0;
		//pos.x = -hexBase; pos.y = -hexInScr; pos.z = 0;
		lineRenderer.SetPosition (3,pos);
		
		pos.x = -width/2f; pos.y = height/2; pos.z = 0;
		//pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (4,pos);
		
		return hex;
		
	}

	GameObject AddQuadrangle (Vector3 setPos)
	{
		Vector3 pos = Vector3.zero;
		
		GameObject hex = new GameObject ("Quad");
		LineRenderer lineRenderer = hex.AddComponent<LineRenderer> (); //not multiple add !!
		hex.transform.parent = m_hexagonMap.transform;
		//MonoBehaviour.Instantiate (lineRenderer.gameObject);
		if (null == lineRenderer)
			Debug.LogError ("lineRenderer is null");
		
		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		lineRenderer.SetColors (Color.green, Color.green);
		lineRenderer.SetWidth (0.2F, 0.2F);
		lineRenderer.SetVertexCount (5);
		lineRenderer.useWorldSpace = false;
		lineRenderer.transform.position = setPos;

		pos.x = -hexCircumScr; pos.y = hexInScr; pos.z = 0;
		//pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (0,pos);

		pos.x = hexBase; pos.y = hexInScr; pos.z = 0;
		//pos.x = hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (1,pos);

		pos.x = hexBase; pos.y = -hexInScr; pos.z = 0;
		//pos.x = hexBase; pos.y = -hexInScr; pos.z = 0;
		lineRenderer.SetPosition (2,pos);

		pos.x = -hexCircumScr; pos.y = -hexInScr; pos.z = 0;
		//pos.x = -hexBase; pos.y = -hexInScr; pos.z = 0;
		lineRenderer.SetPosition (3,pos);

		pos.x = -hexCircumScr; pos.y = hexInScr; pos.z = 0;
		//pos.x = -hexBase; pos.y = hexInScr; pos.z = 0;
		lineRenderer.SetPosition (4,pos);
		
		return hex;
		
	}

	//do not run ,, why not view ? 
	void AddGuiText(Transform parent , string text)
	{
		GameObject objGui = new GameObject ("GuiText");
		GUIText gui = objGui.AddComponent<GUIText> ();
		gui.text = text;
		gui.transform.parent = parent;
		gui.anchor = TextAnchor.MiddleCenter;
		gui.font = new Font ("Arial");
		gui.transform.position = new Vector3 (0.5f, 0.5f, 0);

	}

	void Add3DText(Transform parent, string text, Color color , Vector3 pos)
	{

		GameObject objGui = MonoBehaviour.Instantiate (Resources.Load("Prefab/3DText"),Vector3.zero,Quaternion.identity) as GameObject;
		TextMesh gui = objGui.GetComponent<TextMesh> ();
		objGui.transform.parent = parent;
		objGui.transform.localPosition = pos;
		gui.text = text;
		gui.color = color;
		gui.anchor = TextAnchor.UpperLeft;

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
