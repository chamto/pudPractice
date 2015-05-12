using UnityEngine;
using System.Collections;

public class BaseRules 
{

//	protected eRule m_eRule = eRule.eNone;
//
//	public enum eRule
//	{
//		None = -1,
//
//		PuzzleAndDragon = 0,
//		Custom_1 		= 1,
//		Custom_2 		= 2,
//
//		Max,
//	}



	virtual public void Init()
	{
		//m_eRule = eRule.PuzzleAndDragon; 
	}

	virtual public void Update () 
	{
	
	}

	virtual public void NextState()
	{

	}
	
}
