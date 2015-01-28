using UnityEngine;
using System.Collections;
using PuzzAndBidurgi;

public class Rule_PuzzleAndDragon  :  BaseRules
{

	//==============: Enum :==============
	public enum eState
	{
		Step00_Waiting = 0,
		Step01_DropMove = 1,
		Step02_DropRemoval = 2,
		Step03_DropRestoration = 3,
		Step04_TurnFeed = 4,
		
		Max,
	}

	//==============: Member variable :==============
	private eState m_eState = eState.Step00_Waiting;
	public eState state
	{
		get 
		{
			return m_eState;
		}
		set
		{
			if(m_eState != value)
				m_stateElapsedTime = 0.0f;

			m_eState = value;
		}
	}


	private float m_stateElapsedTime = 0.0f;
	public float stateElapsedTime
	{
		get 
		{
			return m_stateElapsedTime;
		}
	}




	override public void Init() 
	{
	}

	override public void Update () 
	{

		switch (m_eState) 
		{
			case eState.Step00_Waiting:
			{
				Single.InputMgr.permitEvent_DropTouch = true;

			}
			break;
			case eState.Step01_DropMove:
			{
				Single.InputMgr.permitEvent_DropTouch = true;
				
				m_stateElapsedTime += Time.deltaTime;

				//------------------------------------------------------------------------
				//  Switch State of Rule  - Step01_DropMove => Step02_DropRemoval
				// 지정된 시간이 지나면 다음상태로 전환한다
				//------------------------------------------------------------------------
				if(ConstRolePuzzleAndDragon.DropMoveTime <= m_stateElapsedTime)
				{
					this.NextState();
				}
				//------------------------------------------------------------------------

			}
			break;
			case eState.Step02_DropRemoval:
			{
				Single.InputMgr.permitEvent_DropTouch = false;
			}
			break;
			case eState.Step03_DropRestoration:
			{
				Single.InputMgr.permitEvent_DropTouch = false;
			}
			break;
			case eState.Step04_TurnFeed:
			{
				Single.InputMgr.permitEvent_DropTouch = false;
			}
			break;

		}//end switch
	}



	override public void  NextState()
	{
		int nextNumber = ((int)this.m_eState + 1) % (int)eState.Max;
		this.state = (eState)nextNumber;

	}




	//==============: Factory method :==============

	public static Rule_PuzzleAndDragon Factory()
	{
		Rule_PuzzleAndDragon rule = new Rule_PuzzleAndDragon ();
		rule.Init ();
		return rule;
	}
}
