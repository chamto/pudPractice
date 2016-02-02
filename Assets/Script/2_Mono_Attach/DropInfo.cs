
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;



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

