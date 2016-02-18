using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
using T_Bundle = System.Collections.Generic.Dictionary<PuzzAndBidurgi.PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;

namespace PuzzAndBidurgi
{

	//deprecate
//	public enum eDropKind : ushort
//	{
//		
//		//기본 드랍 5종
//		Red = 0,
//		Green,
//		Blue,
//		Light,
//		Dart,
//		
//		//회복드랍
//		Heart,
//		
//		//방해드롭
//		Obstruction,
//		Posion,
//		
//		
//		Max,
//	}

	namespace NDrop
	{
//		public struct SDropInfo
//		{
//			public Vector2 pos;
//			public eResKind eDropKind;
//			public bool isVisible;
//		}

		/// <summary>
		/// 각각의 드롭에 대한 이동순서 정보
		/// </summary>
//		public class CMoveSequence
//		{
//			private float m_timeDelta;
//		}

		/// <summary>
		/// 전체 드롭의 구역별 고유 위치값 (안보이는 드롭도 포함한다)
		/// </summary>
//		public class CLocations
//		{
//			//--- Quadrangle shape
//			private ushort m_columns;
//			private ushort m_rows;
//
//			//--- Circle shape			!!!!!!!Design forecast
//
//			//--- AnyType shape			!!!!!!!Design forecast
//
//			private ArrayList list = new ArrayList();
//			
//			public void Init(ushort columns , ushort rows)
//			{
//			}
//		}
		
		public class CPath
		{
			private Dictionary<float,MonoDrop> 	m_dtnrMovedPath = new Dictionary<float,MonoDrop>();
			
			public void ClearMovedPath()
			{
				//chamto test
				//CDefine.DebugLog("=-=-=-=-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-=");
				//foreach(KeyValuePair<float,MonoDrop>  kv in m_dtnrMovedPath)
				//{
					//CDefine.DebugLog(kv.Key + "  " + kv.Value.name);
				//}
				
				m_dtnrMovedPath.Clear();
			}
			public void AddMovedPath(MonoDrop drop)
			{
				if(null == drop) return;
				
				int prevAddedDropIdx = m_dtnrMovedPath.Count-1;
				if(0 <= prevAddedDropIdx)
				{
					//same object required
					if(drop == m_dtnrMovedPath.ElementAt(prevAddedDropIdx).Value) return;
				}
				
				CDefine.DebugLog("AddMovedPath : " + Time.fixedTime + " " + Time.time);
				m_dtnrMovedPath.Add(Time.time , drop);
			}
			public MonoDrop NextMovedPath()
			{
				if(0 == m_dtnrMovedPath.Count) return null;
				
				float timeKey = m_dtnrMovedPath.Keys.ElementAt(0);
				MonoDrop getDrop = m_dtnrMovedPath[timeKey];
				m_dtnrMovedPath.Remove(timeKey);
				
				return getDrop;
			}
		}//class CPath
	}//namespace NDrop

	/// <summary>
	/// C drop manager.
	/// 드롭을 생성/제거/배치 하는 관리 객체
	/// </summary>
	//end class
}//end namespace
