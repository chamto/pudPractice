
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;

using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
//using T_JoinsInLine = System.Collections.Generic.Dictionary<BundleWithDrop,System.Collections.Generic.List<MonoDrop>> ;
using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
using T_Bundle = System.Collections.Generic.Dictionary<PuzzAndBidurgi.PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;

public class MonoDrop : MonoBehaviour 
{
	//==============: mono member variables :========================================================================================

	public TextMesh			m_textMesh_Index2 = null;
	public TextMesh			m_textMesh_LocalIdx = null;
	//public Index2			testIndex2;

	//spr : sprite , rdr : renderer
	private SpriteRenderer 	m_sprRdr = null;
	private BoxCollider2D	m_boxCollider2D = null;
	public  float 			m_aniSpeed = 30f;

	//==============: member variables :========================================================================================

	private int 			m_id;
	private	Index2			m_index2D;
	public eResKind 		m_eKind = eResKind.None;

	/// <summary>
	/// goto position the rolling drop
	/// </summary>
	private Vector3			m_gotoLocalPosition;

	//join group info
	public BundleWithDrop		m_bundleInfo = null;


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
		aabb.mMinima.x = this.GetComponent<Collider>().bounds.min.x;
		aabb.mMinima.y = this.GetComponent<Collider>().bounds.min.y;
		aabb.mMinima.z = this.GetComponent<Collider>().bounds.min.z;
		aabb.mMaxima.x = this.GetComponent<Collider>().bounds.max.x;
		aabb.mMaxima.y = this.GetComponent<Collider>().bounds.max.y;
		aabb.mMaxima.z = this.GetComponent<Collider>().bounds.max.z;
		
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

	public void SwapGotoLocalPosition (MonoDrop dstDrop)
	{
		if (null != dstDrop) 
		{ 
			Vector3 dstLocalPos = dstDrop.gotoLocalPosition;
			dstDrop.gotoLocalPosition = this.gotoLocalPosition;
			this.gotoLocalPosition = dstLocalPos;
		}
	}

	public void UpdateGotoLocalPosition()
	{
		this.gotoLocalPosition = Single.DropMgr.boardInfo.GetIndex2DToPosition (this.index2D);
	}

	public void ApplyGotoLocalPosition()
	{
		this.transform.localPosition = this.gotoLocalPosition;
	}

	public void MoveToIndex(Index2 dstIxy)
	{
		this.SetIndex (dstIxy);
		this.UpdateGotoLocalPosition ();

		//this.ApplygotoLocalPosition ();
		this.m_aniSpeed = 3f;
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

	//drag test code .. afterwards completing
	void test_TouchDrag(Vector3 offset)
	{
		Vector2 touchPos = Input_Unity.GetTouchPos();

		Vector3 curScreenSpace = new Vector3 (touchPos.x, touchPos.y, 0);
		transform.position = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
		//transform.position = Camera.main.ScreenToWorldPoint (curScreenSpace);
	}

	//cortn : coroutine
	IEnumerator cortnMouseDrag()
	{
		Vector2 touchPos = Input_Unity.GetTouchPos();
		Vector3 scrSpace = Camera.main.WorldToScreenPoint (transform.position);
		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touchPos.x, touchPos.y, scrSpace.z));
		//CDefine.DebugLog(offset + "---"); //chamto test
//		if (Math.Abs (offset.x) > 1.15f / 2f) 
//						offset.x = 0;
//		if (Math.Abs (offset.y) > 1.15f / 2f) 
//			offset.y = 0;



		Vector3 curScreenSpace;
		Vector3 curPosition;
		Vector2 prevTouchPos = touchPos;
		while (Input_Unity.IsTouch())
		//while(Input.GetTouch(0).phase != TouchPhase.Ended)
		//while(Input.touchCount > 0)
		{
			//CDefine.DebugLog("IsTouchCount : " + Input.touchCount); //chamto test
			touchPos = Input_Unity.GetTouchPos();
			if((touchPos - prevTouchPos).sqrMagnitude > 0.5f)
			{
				curScreenSpace = new Vector3 (touchPos.x, touchPos.y, scrSpace.z);
				curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;

				transform.position = curPosition;
				//CDefine.DebugLog("cortnMouseDrag : [" + gameObject.name + "] offset : " + offset + " touchPos : " + touchPos + " curPosition : " + curPosition + "  sqMag:" + (touchPos - prevTouchPos).sqrMagnitude); //chamto test
			}


			prevTouchPos = touchPos;
			yield return null;

		}

		//TouchEnded(); //after process
		//CDefine.DebugLog("cortnMouseDrag end"); //chamto test


	}

	void TouchBegan() 
	{
		if (false == Single.InputMgr.permitEvent_DropTouch)
						return;

		//if(true == Single.DropMgr.groupDrop.ContainsDrop_FromMap (this)) return;

		//CDefine.DebugLog("___________________TouchBegan "+ gameObject.name+"___________________");
		//CDefine.DebugLog("TouchBegan : " + CInputManager.IsTouch());

		StopAni (); //20150417 chamto - 애니메이션 코루틴을 정지시킨 후 드래그 코루틴을 활성화한다. (버그수정 1-1-2 : 동시에 하나의 대상에 대하여 위치갱신하여, 드롭이 떨리는 문제 발생)
		this.DropSpriteUp ();
		//StartCoroutine("cortnMouseDrag");

		//------------------------------------------------
		Vector2 touchPos = Input_Unity.GetTouchPos();
		_offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touchPos.x, touchPos.y, 0));

	}

	Vector3 _offset = Vector3.zero; //chamto temp
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

		test_TouchDrag (_offset); //chamto temp

		OnCollision(); //chamto test

	}
	void TouchEnded() 
	{
		if (false == Single.InputMgr.permitEvent_DropTouch)
			return;

		//CDefine.DebugLog("___________________TouchEnded "+ gameObject.name+"___________________");
		//m_prevCollisionDrop = null;

		//선택된 객체의 이동 종료처리
		StopAni();

		//20150403 chamto test
		//MonoDrop.Remove (this);
		//const int MIN_JOIN_NUMBER = 3;
		//Single.DropMgr.FindJoinConditions (MIN_JOIN_NUMBER);
		//Single.DropMgr.MoveAllJoinDrops ();
		//Single.DropMgr.MoveNextJoinDrops ();
		//Single.DropMgr.WholeDropping (Single.DropMgr.boardInfo.GetMinBoardArea(), Single.DropMgr.boardInfo.GetMaxBoardArea());
		//Single.DropMgr.WholeDroppingOnView ();

		this.StopCoroutine("cortnMouseDrag"); 
		this.StopCoroutine ("cortnMovingComboAni");
		StartCoroutine ("cortnMovingComboAni");

	}

	void DropSpriteUp()
	{
		//transform.Translate(0,0,-1); //drop sprite up
		Vector3 v3Pos = transform.position;
		v3Pos.z = -1;
		transform.position = v3Pos;
	}


	IEnumerator cortnMovingComboAni()
	{

		Single.InputMgr.permitEvent_DropTouch = false;

		const int MIN_JOIN_NUMBER = 3;

		GroupDrop groupDrop = Single.DropMgr.FindJoinConditions (MIN_JOIN_NUMBER);


		while (0 != groupDrop.GetMaxCount()) 
		{
			if(groupDrop.GetNextCount() == groupDrop.GetMaxCount())
			{
				yield return new WaitForSeconds(0.7f);
				Single.DropMgr.WholeDropping (Single.DropMgr.boardInfo.GetMinBoardArea(), Single.DropMgr.boardInfo.GetMaxBoardArea());
				Single.DropMgr.FindJoinConditions (MIN_JOIN_NUMBER);
			}

			yield return new WaitForSeconds(0.7f);

			Single.DropMgr.MoveNextJoinDrops ();

		}


		yield return new WaitForSeconds(1.2f);
		Single.InputMgr.permitEvent_DropTouch = true;

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
				if(null != _prevLastCollisionDrop) _prevLastCollisionDrop.StopAni();
				dstDrop.StopAni(); 
				
				//2.setting target position 
				//swap gotoPostion
				//Single.DropMgr.SwapgotoLocalPosition(this,dstDrop);
				Single.DropMgr.SwapMonoDropInBoard(this.id , dstDrop.id, false);
				//break;

				//3.animation target drop
				//rolling drop
				dstDrop.m_aniSpeed = 30;
				dstDrop.MovingAni(dstDrop.gotoLocalPosition);
				
				
			}
		}
		_prevLastCollisionDrop = lastDrop;
	}

//	MonoDrop m_prevCollisionDrop = null;
//	void OnCollision1111()
//	{
//		const float NONCOLLISION_MIN_DISTANCE = 0.55f;
//		//CDrop collisionDrop = CMain.MGDrop.CalcCollision(this); //chamto test 
//		MonoDrop collisionDrop = Single.DropMgr.GetShortestDistance(this,NONCOLLISION_MIN_DISTANCE); 
//		//CDefine.DebugLog("___"+collisionDrop); //chamto test
//
//
//
//		if(m_prevCollisionDrop == collisionDrop) return; //one treatment , avoidance continuous collision
//
//		if(null != collisionDrop && this != collisionDrop)
//		{
//
//			Single.DropMgr.AddMovedPath(collisionDrop);
//			//CDefine.DebugLog("_______________"); //chamto test
//			//moving complete after process
//
//			if(null != m_prevCollisionDrop) m_prevCollisionDrop.StopAni(); //test comment
//			collisionDrop.StopAni(); 
//
//
////			CDefine.DebugLog("OnCollision " + collisionDrop.gameObject.name + " pos : " + collisionDrop.transform.position + 
////			                 "  local+Parent : " + (collisionDrop.transform.localPosition + collisionDrop.transform.parent.transform.position)); //chamto test
//
//			//swap gotoPostion
//			Single.DropMgr.SwapMonoDropInBoard(this.id , collisionDrop.id , false);
//			
//			//rolling drop
//			//collisionDrop.MovingAni(collisionDrop.gotoLocalPosition); //test comment
//
//
//			CDefine.DebugLog("OnCollision " + collisionDrop.gameObject.name); //chamto test
//
//
//		}//else
//		//{
//		//	CDefine.DebugLog("unreachead OnCollision else : prevDrop : " + m_prevCollisionDrop); //chamto test
//		//}
//		m_prevCollisionDrop = collisionDrop;
//	}


	//==============: member method :========================================================================================


	public void MovingAni(Vector3 dstLocalPos)
	{
		this.StopCoroutine("cortnMovingAni"); //기존 동작되던 coroutine을 정지후, 새로운 coroutine을 동작시킨다.

		this.StartCoroutine("cortnMovingAni",dstLocalPos );
	}
	public void StopAni()
	{
		this.ApplyGotoLocalPosition ();
		//transform.localPosition = gotoLocalPosition;
		this.StopCoroutine("cortnMovingAni");
	}
	IEnumerator cortnMovingAni(Vector3 dstLocalPos)
	{

		Vector3 diff = new Vector3(1,1,1);

		//while(diff.sqrMagnitude > 0) //bug code : 실수값 부정확도에 따른 문제 때문에 루프를 못빠져 나온다
		while(Math.Abs(diff.sqrMagnitude) > float.Epsilon )
		{
			diff = dstLocalPos - transform.localPosition;
			transform.localPosition += diff * Time.deltaTime * m_aniSpeed;
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

#if UNITY_EDITOR
		Single.DropMgr.Update_DebugMap ();  //chamto test
#endif


		return true;
	}


	static public void DismissGroup(MonoDrop drop)
	{
		if (null != drop.m_bundleInfo) 
		{
			drop.m_bundleInfo.DissmissBundle();
		}
		drop.m_bundleInfo = null;
	}
	static public void MoveToEmptySquare(MonoDrop drop)
	{

		if (null == drop)
						return;

		//drop.bundleInfo = null; //import!!! - dismiss group
		MonoDrop.DismissGroup (drop);

		//---------------------------------- move to emptySquare in nonviewArea
		Index2 empty = Single.DropMgr.mapDrop.FindEmptySquare (Single.DropMgr.boardInfo.GetMinNonviewArea (),
		                                                                  Single.DropMgr.boardInfo.GetMaxNonviewArea ());
		//Debug.Log ("MoveToEmptySquare : " + empty);
		if (Index2.None != empty) 
		{
			drop.SetIndex (empty);
			drop.UpdateGotoLocalPosition ();
			drop.SetColor(Color.red);

			//drop.ApplyGotoLocalPosition();
			drop.m_aniSpeed = 5;
			drop.MovingAni(drop.gotoLocalPosition);
			drop.GetBoxCollider2D().enabled = false;
			//drop.setDropKind = Single.DropMgr.GetRandDrop(9);
			drop.setDropKind = Single.DropMgr.GetRandDrop(5);
		}

#if UNITY_EDITOR
		Single.DropMgr.Update_DebugMap ();  //chamto test
#endif
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

