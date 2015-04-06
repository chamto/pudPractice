
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PuzzAndBidurgi;

public class MonoDrop : MonoBehaviour 
{
	//==============: mono member variables :========================================================================================

	public TextMesh			m_textMesh_Index2 = null;
	public TextMesh			m_textMesh_LocalIdx = null;
	//public Index2			testIndex2;

	//spr : sprite , rdr : renderer
	private SpriteRenderer 	m_sprRdr = null;
	private BoxCollider2D	m_boxCollider2D = null;

	//==============: member variables :========================================================================================

	private int 			m_id;
	private	Index2			m_index2D;
	public eResKind 		m_eKind = eResKind.None;

	/// <summary>
	/// goto position the rolling drop
	/// </summary>
	private Vector3			m_gotoLocalPosition;


	//==============: property definition :========================================================================================

	public int id
	{
		get 
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}  
	public Index2 	index2D
	{
		get 
		{
			return m_index2D;
		}
		set
		{
			m_index2D = value;
		}
	}


	public eResKind 		dropKind
	{
		get
		{
			return m_eKind;
		}
	}
	public eResKind 		setDropKind
	{

		set //chamto 20150130 fixme - 단순 대입코드로 오해 할수 있음. 행위코드라고 명시적으로 뽑아내기 
		{
			if(null == m_sprRdr) 
			{
				CDefine.DebugLogError(string.Format("SpriteRenderer is null"));
				return;
			}
			m_eKind = value;
			m_sprRdr.sprite = Single.ResMgr.LoadSprites(value,SResDefine.spINDEX_ZERO);
		}
	}


	public Vector3			gotoLocalPosition
	{
		get { return m_gotoLocalPosition; }
		set { m_gotoLocalPosition = value; }	
	}
	public Vector3			gotoWorldPosition
	{
		get { return m_gotoLocalPosition + transform.parent.position; }
	}


	//==============: get,set method :========================================================================================

	public void SetColor(Color color)
	{
		if(null == m_sprRdr) 
		{
			CDefine.DebugLogError(string.Format("SpriteRenderer is null"));
			return;
		}

		m_sprRdr.color = color;
	}

	public BoxCollider2D GetBoxCollider2D()
	{
		if (null == m_boxCollider2D) 
		{
			CDefine.DebugLogError(string.Format("m_boxCollider is null"));
			return null;
		}
		return m_boxCollider2D;
	}

	
	/// <summary>
	/// 유니티Collider를 AABB로 전환하여 반환한다.
	/// </summary>
	/// <returns>The AAB box.</returns>
	public ML.AABBox  GetAABBox()
	{
		
		ML.AABBox aabb = new ML.AABBox ();
		aabb.mMinima.x = this.collider.bounds.min.x;
		aabb.mMinima.y = this.collider.bounds.min.y;
		aabb.mMinima.z = this.collider.bounds.min.z;
		aabb.mMaxima.x = this.collider.bounds.max.x;
		aabb.mMaxima.y = this.collider.bounds.max.y;
		aabb.mMaxima.z = this.collider.bounds.max.z;
		
		return aabb;
	}


	/// <summary>
	/// Sets the index.
	/// 놓여질 위치의 이전monoDrop도 같은 인덱스값을 가리키기 때문에, 반환되는 prevPlacedMonoDrop에 대한 추가처리를 해주어야 한다
	/// </summary>
	/// <param name="placedIxy">Placed ixy.</param>
	public MonoDrop SetIndex(Index2 placedIxy )
	{
		MonoDrop prevPlacedDrop = Single.DropMgr.mapDrop.GetMonoDropByIndex2 (placedIxy);

		//Put the null in the previous index location
		MonoDrop currentDrop = Single.DropMgr.mapDrop.GetMonoDropByIndex2(m_index2D);
		if (currentDrop == this) //blocking that put the null , the default index value (0,0)
				Single.DropMgr.mapDrop.SetValue (m_index2D, null);
		//else if(currentDrop != null) // mapIndex is pointing value ,the currentDrop is not same
			//Debug.Log ("id: "+this.id+" SetIndex :" + placedIxy + " currentDrop != this && currentDrop != null :" + currentDrop + " : " + this + " / "); //chamto test

		//Put the this MonoDrop in the new index location
		m_index2D = placedIxy;
		Single.DropMgr.mapDrop.SetValue (placedIxy, this);

		//20150403 chamto test
		Single.DropMgr.Update_DebugMap ();

		return prevPlacedDrop;
	}
	
	public void SwapIndex(Index2 dstIxy)
	{
		MonoDrop prevPlacedDrop = Single.DropMgr.mapDrop.GetMonoDropByIndex2 (dstIxy);
		if (null != prevPlacedDrop) 
		{
			//Debug.Log("1----id: "+this.id+" SwapIndex :" + dstIxy + "  thisIndex :"+this.index2D); //chamto test
			prevPlacedDrop.SetIndex(this.index2D);
			//Debug.Log("2----");
			this.SetIndex(dstIxy);
		}

	}

	public void SwapgotoLocalPosition (MonoDrop dstDrop)
	{
		if (null != dstDrop) 
		{ 
			Vector3 dstLocalPos = dstDrop.gotoLocalPosition;
			dstDrop.gotoLocalPosition = this.gotoLocalPosition;
			this.gotoLocalPosition = dstLocalPos;
		}
	}

	public void UpdategotoLocalPosition()
	{
		this.gotoLocalPosition = Single.DropMgr.boardInfo.GetIndex2DToPosition (this.index2D);
	}

	public void ApplygotoLocalPosition()
	{
		this.transform.localPosition = this.gotoLocalPosition;
	}

	public void MoveToIndex(Index2 dstIxy)
	{
		this.SetIndex (dstIxy);
		this.UpdategotoLocalPosition ();

		//this.ApplygotoLocalPosition ();
		this.MovingAni (this.gotoLocalPosition);
	}

	//==============: Constructor definition :========================================================================================
	
	public MonoDrop()
	{
		//m_id = Single.ResMgr.GetSequenceId ();
	}

	//==============: Initialization method :========================================================================================




	//==============: class callback method :========================================================================================

	void Start()
	{
		//m_dropInfo = DropInfo.Create ();
	}

	void Awake()
	{
		m_sprRdr = gameObject.GetComponentInChildren<SpriteRenderer>();
		m_boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();

	}



	//==============: Touch method :========================================================================================

	void TouchDrag()
	{
		Vector2 touchPos = Input_Unity.GetTouchPos();
		Vector3 scrSpace = Camera.main.WorldToScreenPoint (transform.position);
//		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touchPos.x, touchPos.y, scrSpace.z));

			
		Vector3 curScreenSpace = new Vector3 (touchPos.x, touchPos.y, scrSpace.z);
//		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
//			transform.position = curPosition;


		transform.position = Camera.main.ScreenToWorldPoint(curScreenSpace);
	}

	//cortn : coroutine
	IEnumerator cortnMouseDrag()
	{
		Vector2 touchPos = Input_Unity.GetTouchPos();
		Vector3 scrSpace = Camera.main.WorldToScreenPoint (transform.position);
		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touchPos.x, touchPos.y, scrSpace.z));
		//CDefine.DebugLog(offset + "---"); //chamto test

		Vector3 curScreenSpace;
		Vector3 curPosition;
		while (Input_Unity.IsTouch())
		//while(Input.GetTouch(0).phase != TouchPhase.Ended)
		//while(Input.touchCount > 0)
		{
			//CDefine.DebugLog("IsTouchCount : " + Input.touchCount); //chamto test
			touchPos = Input_Unity.GetTouchPos();
			curScreenSpace = new Vector3 (touchPos.x, touchPos.y, scrSpace.z);
			curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
			transform.position = curPosition;
			//CDefine.DebugLog("cortnMouseDrag : [" + gameObject.name + "] offset : " + offset + " touchPos : " + touchPos + " curPosition : " + curPosition ); //chamto test
			yield return null;

		}

		TouchEnded(); //after process
		//CDefine.DebugLog("cortnMouseDrag end"); //chamto test


	}

	void TouchBegan() 
	{
		if (false == Single.InputMgr.permitEvent_DropTouch)
						return;

		//CDefine.DebugLog("TouchBegan "+ gameObject.name);
		//CDefine.DebugLog("TouchBegan : " + CInputManager.IsTouch());


		transform.Translate(0,0,-1); //drop sprite up
		//StopCoroutine("cortnMouseDrag");
		StartCoroutine("cortnMouseDrag");

	}
	void TouchMoved() 
	{
		if (false == Single.InputMgr.permitEvent_DropTouch)
			return;

		//------------------------------------------------------------------------
		//  Switch State of Rule - Step00_Waiting => Step01_DropMove
		//------------------------------------------------------------------------
		Rule_PuzzleAndDragon rule = Single.MonoMain.rules as Rule_PuzzleAndDragon;
		if (null != rule && rule.state == Rule_PuzzleAndDragon.eState.Step00_Waiting) 
		{
			rule.state = Rule_PuzzleAndDragon.eState.Step01_DropMove;
		}
		//------------------------------------------------------------------------

		//CDefine.DebugLog("___________________TouchMoved "+gameObject.name+"___________________");
		//CDefine.DebugLog("TouchMoved : " + CInputManager.IsTouch());
		//TouchDrag(); //chamto test



		OnCollision(); //chamto test

	}
	void TouchEnded() 
	{
		if (false == Single.InputMgr.permitEvent_DropTouch)
			return;

		//CDefine.DebugLog("___________________TouchEnded "+ gameObject.name+"___________________");
		transform.Translate(0,0,1); //drop sprite down
		m_prevCollisionDrop = null;
		Single.DropMgr.ClearMovedPath();

		//선택된 객체의 이동 종료처리
		StopAni();

		//20150403 chamto test
		//MonoDrop.Remove (this);
		Single.DropMgr.FindJoinConditions (3);
		//Single.DropMgr.WholeDropping (new Index2 (0, 0), new Index2 (5, 9));

	}


	//==============: collision method :========================================================================================

	//20140830 chamto - ccdTest function
	private MonoDrop _prevLastCollisionDrop = null;
	void OnCollision()
	{

		//const float NONCOLLISION_MIN_DISTANCE = 0.55f;
		const float NONCOLLISION_MIN_DISTANCE = 0.55f;
		List<MonoDrop> colList = Single.DropMgr.CalcCCDCollision (this, NONCOLLISION_MIN_DISTANCE);

		MonoDrop lastDrop = null;
		if (0 < colList.Count) 
		{
			lastDrop = colList[colList.Count-1];		
			//CDefine.DebugLog ("collisionList Count : "+ colList.Count + " " + lastDrop); //chamto test
		}

		foreach (MonoDrop dstDrop in colList) 
		{

			if(_prevLastCollisionDrop == dstDrop) return; //one treatment , avoidance continuous collision
			
			if(null != dstDrop && this != dstDrop)
			{
				//Debug.Log("--------- onCollision ---------"); //chamto test
				//moving complete after process
				//1.stop move animation
				if(null != m_prevCollisionDrop) m_prevCollisionDrop.StopAni();
				dstDrop.StopAni(); 
				
				//2.setting target position 
				//swap gotoPostion
				//Single.DropMgr.SwapgotoLocalPosition(this,dstDrop);
				Single.DropMgr.SwapMonoDropInBoard(this.id , dstDrop.id, false);
				//break;

				//3.animation target drop
				//rolling drop
				dstDrop.MovingAni(dstDrop.gotoLocalPosition);
				
				
			}
		}
		_prevLastCollisionDrop = lastDrop;
	}

	MonoDrop m_prevCollisionDrop = null;
	void OnCollision1111()
	{
		const float NONCOLLISION_MIN_DISTANCE = 0.55f;
		//CDrop collisionDrop = CMain.MGDrop.CalcCollision(this); //chamto test 
		MonoDrop collisionDrop = Single.DropMgr.GetShortestDistance(this,NONCOLLISION_MIN_DISTANCE); 
		//CDefine.DebugLog("___"+collisionDrop); //chamto test



		if(m_prevCollisionDrop == collisionDrop) return; //one treatment , avoidance continuous collision

		if(null != collisionDrop && this != collisionDrop)
		{

			Single.DropMgr.AddMovedPath(collisionDrop);
			//CDefine.DebugLog("_______________"); //chamto test
			//moving complete after process

			if(null != m_prevCollisionDrop) m_prevCollisionDrop.StopAni(); //test comment
			collisionDrop.StopAni(); 


//			CDefine.DebugLog("OnCollision " + collisionDrop.gameObject.name + " pos : " + collisionDrop.transform.position + 
//			                 "  local+Parent : " + (collisionDrop.transform.localPosition + collisionDrop.transform.parent.transform.position)); //chamto test

			//swap gotoPostion
			Single.DropMgr.SwapMonoDropInBoard(this.id , collisionDrop.id , false);
			
			//rolling drop
			//collisionDrop.MovingAni(collisionDrop.gotoLocalPosition); //test comment


			CDefine.DebugLog("OnCollision " + collisionDrop.gameObject.name); //chamto test


		}//else
		//{
		//	CDefine.DebugLog("unreachead OnCollision else : prevDrop : " + m_prevCollisionDrop); //chamto test
		//}
		m_prevCollisionDrop = collisionDrop;
	}


	//==============: member method :========================================================================================


	public void MovingAni(Vector3 dstLocalPos)
	{

		StartCoroutine("cortnMovingAni",dstLocalPos);
	}
	public void StopAni()
	{
		transform.localPosition = gotoLocalPosition;
		StopCoroutine("cortnMovingAni");
	}
	IEnumerator cortnMovingAni(Vector3 dstLocalPos)
	{
		Vector3 diff = new Vector3(1,1,1);
		while(Math.Abs(diff.sqrMagnitude) > float.Epsilon )
		{
			diff = dstLocalPos - transform.localPosition;
			transform.localPosition += diff * Time.deltaTime * 30;
			//Debug.Log("loop.. :"+ this+ " dstLocalPos :"+dstLocalPos + "  target:"+transform.localPosition + "  diff:"+diff.sqrMagnitude ); //chamto test
			yield return null;
		}

		//CDefine.DebugLog("!!!!!!! Ended cortnMovingAni " + this); //chamto test
		transform.localPosition = dstLocalPos;
	}

	//==============: factory method :========================================================================================

	static public bool Remove(MonoDrop drop)
	{
		if (null == drop)
						return false;

		Single.DropMgr.mapDrop.Remove (drop.id);

		//drop.SetColor(new Color(1,1,1,0.2f));
		//drop.GetBoxCollider2D().enabled = false;
		//drop.gameObject.SetActive(false);
		MonoBehaviour.Destroy(drop.gameObject);

		//20150403 chamto test
		Single.DropMgr.Update_DebugMap ();

		return true;
	}


	static public MonoDrop Create(Transform parent , eResKind eDrop , Vector3 localPos)
	{
		GameObject newObj = CResoureManager.CreatePrefab(SResDefine.pfDROPINFO);
		if(null == newObj) 
		{
			CDefine.DebugLogError(string.Format("Failed to create Prefab : " + SResDefine.pfDROPINFO));
			return null;
		}
		MonoDrop drop = newObj.GetComponent<MonoDrop>();
		if(null == drop) 
		{
			CDefine.DebugLogError(string.Format("MonoDrop is null"));
			return null;
		}
		

		//-------------------------------------------------

		//drop.dropInfo = DropInfo.Create ();
		drop.id = Single.ResMgr.GetSequenceId ();
		//drop.SetIndex(Single.DropMgr.boardInfo.GetPositionToIndex2D (localPos));
		drop.index2D = Single.DropMgr.boardInfo.GetPositionToIndex2D (localPos);
		drop.setDropKind = eDrop;

		//Specify the parent object
		drop.transform.parent = parent;
		drop.name = "drop" + drop.id;
		
		//newObj.transform.position = new Vector3(relativeCoord_x,relativeCoord_y,0); //[주의!!] 부모에 대한 상대좌표를 지정해야 하기 때문에 localposition 을 써야 한다.  
		drop.transform.localPosition = localPos;
		
		
		//todo modify that localposition
		drop.gotoLocalPosition = localPos;

		//20150331 chamto test
		//drop.testIndex2.ix = drop.index2D.ix;
		//drop.testIndex2.iy = drop.index2D.iy;
		drop.m_textMesh_Index2 = MonoDrop.Add3DText (drop.transform, drop.index2D.ToString (), Color.white, new Vector3(-0.5f,0,-2f));
		//Index2 localIdx = Single.DropMgr.Board.GetPositionToIndex2D (drop.gotoLocalPosition);
		//drop.m_textMesh_LocalIdx = MonoDrop.Add3DText (drop.transform, localIdx.ToString(), Color.red, new Vector3(-0.5f,-0.3f,-2f));

		
		return drop;
	}

	public void UpdateTextMesh()
	{
		if (null != m_textMesh_Index2)
			m_textMesh_Index2.text = this.index2D.ToString ();

		if (null != m_textMesh_LocalIdx) 
		{
			Index2 localIdx = Single.DropMgr.boardInfo.GetPositionToIndex2D (this.gotoLocalPosition);
			m_textMesh_LocalIdx.text = localIdx.ToString();
		}
	}

	static public TextMesh Add3DText(Transform parent, string text, Color color , Vector3 pos)
	{
		
		GameObject objGui = MonoBehaviour.Instantiate (Resources.Load("Prefab/3DText"),Vector3.zero,Quaternion.identity) as GameObject;
		TextMesh gui = objGui.GetComponent<TextMesh> ();
		objGui.transform.parent = parent;
		objGui.transform.localPosition = pos;
		gui.text = text;
		gui.color = color;
		gui.anchor = TextAnchor.UpperLeft;

		return gui;
	}
	
}


//=====================================================================

[System.Serializable]
public struct Index2
{

	//max_column = root(int_max + 1)
	//min_column = root(int_min - 1)
	static int MAX_COLUMN = 46340;
	static int MIN_COLUMN = -46341;

	public int ix;
	public int iy;


	static public Index2 Zero
	{
		get 
		{
			return new Index2(0,0);
		}
	}

	static public Index2 Max
	{
		get 
		{
			return new Index2(MAX_COLUMN,MAX_COLUMN);
		}
	}

	public int this [int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return this.ix;
			case 1:
				return this.iy;
			
			default:
				throw new IndexOutOfRangeException ("Invalid Index2 index!");
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				this.ix = value;
				break;
			case 1:
				this.iy = value;
				break;
			
			default:
				throw new IndexOutOfRangeException ("Invalid Index2 index!");
			}
		}
	}

	public Index2(int ix, int iy)
	{
		this.ix = ix;
		this.iy = iy;
	}

	static bool ValidMaxColumn(int maxColumn)
	{
		if (MIN_COLUMN <= maxColumn && maxColumn <= MAX_COLUMN) 
		{
			return true;
		}

		return false;
	}

	static public Index2 Index1ToIndex2(int index1 , int maxColumn)
	{
		if (false == ValidMaxColumn (maxColumn)) 
		{
			Debug.LogError("exception !! : invalid value that maxcolumn");
			return new Index2();
		}

		if (0 == maxColumn) 
		{
			Debug.LogError ("exception !! : divide by 0 ");
			return new Index2();
		}


		Index2 value;
		value.ix = index1 % maxColumn;
		value.iy = index1 / maxColumn;

		return value;
	}

	static public int Index2ToIndex1(Index2 ixyPair , int maxColumn)
	{
		return ixyPair.ix + ixyPair.iy * maxColumn;
	}


	static public bool operator != (Index2 ixy1 , Index2 ixy2)
	{
		if (ixy1.ix != ixy2.ix || ixy1.iy != ixy2.iy)
			return true;

		return false;
	}

	static public bool operator == (Index2 ixy1 , Index2 ixy2)
	{
		if (ixy1.ix == ixy2.ix && ixy1.iy == ixy2.iy)
			return true;

		return false;
	}


	public override bool Equals (object other)
	{
		if (!(other is Index2))
		{
			return false;
		}
		Index2 ixy = (Index2)other;
		return Equals (ixy);
	}

	public bool Equals(Index2 ixy)
	{
		if (this.ix == ixy.ix && this.iy == ixy.iy)
			return true;

		return false;
	}

	override public int GetHashCode()
	{
		return ix ^ iy;
	}

	override public string ToString()
	{
		return ix + "," + iy;
	}

}
public struct Index3
{
	public int ix;
	public int iy;
	public int iz;

	public Index3(int ix, int iy, int iz)
	{
		this.ix = ix;
		this.iy = iy;
		this.iz = iz;
	}
}

//20150403 chamto - no use , must be cleared
[System.Serializable]
public class DropInfo
{

	//==============: member Constant :========================================================================================
	//public const float WIDTH_DROP = 1.15f;
	//public const float HEIGHT_DROP = 1.15f;


	//==============: member variables :========================================================================================
	private int 	m_id;
	private	Index2	m_index2D;


	//==============: property definition :========================================================================================
	//[HideInInspector] [SerializeField]
	//[ExposeProperty]
	public int id
	{
		get
		{
			return m_id;
		}
		set 
		{
			m_id = value;
		}
	}


	public Index2 index2D
	{
		get
		{
			return m_index2D;
		}
		set
		{
			m_index2D = value;
		}
	}

	//==============: get,set method :========================================================================================



	//==============: factory method :========================================================================================

	static public DropInfo Create()
	{
		DropInfo newObj = new DropInfo ();
		newObj.Init ();

		return newObj;
	}

	//==============: member method :========================================================================================
	public void Init()
	{
		m_id = 0;
		m_index2D.ix = 0;
		m_index2D.iy = 0;
	}

}


