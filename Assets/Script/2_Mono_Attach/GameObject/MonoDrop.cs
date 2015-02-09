
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PuzzAndBidurgi;

public class MonoDrop : MonoBehaviour 
{
	//==============: member variables :==============
	
	//spr : sprite , rdr : renderer
	private SpriteRenderer 	m_sprRdr = null;
	public int 				keyOfPosition;
	public eResKind 		m_eKind = eResKind.None;
	public eResKind 		kind
	{
		get
		{
			return m_eKind;
		}
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

	/// <summary>
	/// first position the rolling drop
	/// </summary>
	private Vector3			m_firstLocalPosition;
	public Vector3			firstLocalPosition
	{
		get { return m_firstLocalPosition; }
		set { m_firstLocalPosition = value; }	
	}
	public Vector3			firstWorldPosition
	{
		get { return m_firstLocalPosition + transform.parent.position; }
	}


	private bool m_isSelected = false;

	//==============: member method :==============

	void Awake()
	{
		m_sprRdr = gameObject.GetComponentInChildren<SpriteRenderer>();

	}



	void TouchDrag()
	{
		Vector2 touchPos = Input_Unity.GetTouchPos();
		Vector3 scrSpace = Camera.main.WorldToScreenPoint (transform.position);
		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touchPos.x, touchPos.y, scrSpace.z));

			
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

		m_isSelected = true;
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
		m_isSelected = false;
		transform.Translate(0,0,1); //drop sprite down
		m_prevCollisionDrop = null;
		Single.DropMgr.ClearMovedPath();

		//선택된 객체의 이동 종료처리
		StopAni();

	}

	//no use
	//void OnCollisionEnter2D (Collision2D col)
	void _noUse_OnTriggerEnter2D(Collider2D col)
	//void OnTriggerStay2D(Collider2D col)
	{
		//if(gameObject.Equals(col.gameObject.transform.parent.gameObject)) return;

		//CDefine.DebugLog("OnTriggerEnter2D: detected!! " + col.gameObject.name); //chamto test

		//선택된 객체와 자리를 바꾼다
		if(false == m_isSelected)
		{
			MonoDrop movingDrop = col.gameObject.transform.parent.GetComponent<MonoDrop>();
			//CDrop movingDrop = col.gameObject.transform.GetComponent<CDrop>();
			if(null == movingDrop)
			{
				CDefine.DebugLogWarning("OnCollisionEnter2D : CDrop is Null!!! : " + this); return;
			}

			//switch firstLocalPosition : movingDrop <-> regionDrop
			Vector3 temp = movingDrop.firstLocalPosition;
			movingDrop.firstLocalPosition = firstLocalPosition;
			firstLocalPosition = temp;

			//rolling drop
			gameObject.transform.localPosition = firstLocalPosition;
		}
	}

//	private void _SwapFirstLocalPosition(MonoDrop drop1 , MonoDrop drop2) 
//	{
//		Vector3 temp = drop1.firstLocalPosition;
//		drop1.firstLocalPosition = drop2.firstLocalPosition;
//		drop2.firstLocalPosition = temp;
//	}


	//20140830 chamto - ccdTest function
	MonoDrop _prevLastCollisionDrop = null;
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

				//moving complete after process
				//1.stop move animation
				if(null != m_prevCollisionDrop) m_prevCollisionDrop.StopAni();
				dstDrop.StopAni(); 
				
				//2.setting target position 
				//swap firstPostion
				//Single.DropMgr.SwapFirstLocalPosition(this,dstDrop);
				Single.DropMgr.SwapMonoDropInBoard(this.keyOfPosition , dstDrop.keyOfPosition, false);
				//break;

				//3.animation target drop
				//rolling drop
				dstDrop.MovingAni(dstDrop.firstLocalPosition);
				
				
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

			//swap firstPostion
			//Vector3 temp = firstLocalPosition;
			//firstLocalPosition = collisionDrop.firstLocalPosition;
			//collisionDrop.firstLocalPosition = temp;

			//_SwapFirstLocalPosition(this , collisionDrop);
			Single.DropMgr.SwapMonoDropInBoard(this.keyOfPosition , collisionDrop.keyOfPosition , false);
			
			//rolling drop
			collisionDrop.MovingAni(collisionDrop.firstLocalPosition); //test comment


			//CDefine.DebugLog("OnCollision " + collisionDrop.gameObject.name); //chamto test


		}//else
		//{
		//	CDefine.DebugLog("unreachead OnCollision else : prevDrop : " + m_prevCollisionDrop); //chamto test
		//}
		m_prevCollisionDrop = collisionDrop;
	}

	public void MovingAni(Vector3 dstLocalPos)
	{

		StartCoroutine("cortnMovingAni",dstLocalPos);
	}
	public void StopAni()
	{
		transform.localPosition = firstLocalPosition;
		StopCoroutine("cortnMovingAni");
	}
	IEnumerator cortnMovingAni(Vector3 dstLocalPos)
	{
		Vector3 diff = new Vector3(1,1,1);
		while(diff.sqrMagnitude > 0)
		{
			diff = dstLocalPos - transform.localPosition;
			transform.localPosition += diff * Time.deltaTime * 30;
			yield return null;
		}

		//CDefine.DebugLog("cortnMoving " + gameObject.name); //chamto test
		transform.localPosition = dstLocalPos;
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
	

}

//namespace PuzzAndBidurgi
//{
//	public class CDrop
//	{
//		public CDrop ()
//		{
//		}
//
//	}
//}