
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;


namespace PuzzAndBidurgi
{

	[System.Serializable]
	public class DropInfo
	{
		public enum eKind
		{
			None = 0,
			Red,
			Green,
			Blue,
			Light,
			Dark,
			Heart,
			Obstruction,
			Posion,
			Posion2,
			Max,
		}

		//==============: member variables :========================================================================================
		public const UInt16 NULL_ID_UINT16 = UInt16.MaxValue;

		public UInt16 id = NULL_ID_UINT16;

		//드롭의 종류
		public eKind kind = eKind.None;

		//강화드롭 상태인지를 나타냄
		public byte  reinforcement = 0;

		public void Init()
		{
			id = NULL_ID_UINT16;
			kind = eKind.None;
			reinforcement = 0;
		}

	}
}



