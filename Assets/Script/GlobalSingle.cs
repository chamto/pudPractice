using UnityEngine;
using System;
using System.Collections;
using PuzzAndBidurgi;

/// <summary>
/// Global Single
/// </summary>
public class Single
{

	// ------------------------mono------------------------

	public static MonoMain MonoMain
	{
		get
		{
			return CSingletonMono<MonoMain>.Instance;
		}
	}

	public static GameObject OBJRoot
	{
		get
		{
			return CSingletonMono<MonoMain>.Instance.m_objRoot;
		}
	}

	public static MonoInputManager InputMgr 
	{
		get
		{
			return CSingletonMono<MonoInputManager>.Instance;
		}
	}

	public static MonoDebug MonoDebug
	{
		get
		{
			return CSingletonMono<MonoDebug>.Instance;
		}
	}



	// ------------------------class------------------------

	public static CResoureManager ResMgr
	{
		get
		{
			return CSingleton<CResoureManager>.Instance;
		}
	}
	
	public static CDropManager DropMgr
	{
		get
		{
			return CSingleton<CDropManager>.Instance;
		}
	}

}
