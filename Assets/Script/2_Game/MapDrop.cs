using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
using T_Bundle = System.Collections.Generic.Dictionary<PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;

namespace PuzzAndBidurgi
{

	public class MapDrop
	{
		//인덱스를 조회하기 위한 인덱스맵을 조직한다
		private Dictionary<Index2,MonoDrop> _mapIndex2 = new Dictionary<Index2,MonoDrop>();

		private Dictionary<int,MonoDrop> 	_mapId = new Dictionary<int, MonoDrop> ();

		public Dictionary<int,MonoDrop> DtnrId
		{
			get
			{
				return _mapId;
			}
		}

		//--------------------------------------------------
		//idMap 자료구조와 indexMap 자료구조는 1대1 대응하지 않는다.
		//예) 특정 드롭이 등록되지 않은 위치로 갈때 indexMap에 추가되게 된다. 
		//   이때 idMap은 드롭이 추가된것이 아니기 때문에 변동이 없다.
		private void notuse_UpdateValue(MonoDrop valueDrop , Index2 prevIndex)
		{
			if (null == valueDrop) 
			{
				CDefine.DebugLog("Error !!: null == valueDrop");
				return;
			}

			MonoDrop getValue = null;
			if (_mapId.TryGetValue (valueDrop.id, out getValue) && getValue == valueDrop) 
			{

				//Add index2Coord in mapIndex2 if index2 is not register
				if (false == _mapIndex2.TryGetValue (valueDrop.index2D, out getValue)) 
				{
					_mapIndex2.Add(valueDrop.index2D, valueDrop);
				}

				//MonoDrop prevPlacedDrop = _mapIndex2[valueDrop.index2D];
				_mapIndex2[valueDrop.index2D] = valueDrop;
			}
		}

		public bool SetValue(Index2 keyIndex , MonoDrop valueDrop)
		{
			if (null != valueDrop && valueDrop.index2D != keyIndex) 
			{
				CDefine.DebugLog("Warring !!! : valueDrop.index2D != keyIndex : " + valueDrop.index2D + " " + keyIndex);
				return false;
			}

			MonoDrop getValue = null;
			if (false == _mapIndex2.TryGetValue (keyIndex, out getValue)) 
			{
				_mapIndex2.Add(keyIndex, valueDrop);
			}

			_mapIndex2[keyIndex] = valueDrop;
			return true;
		}

		public MonoDrop GetMonoDropByUID(int keyUID) 
		{
			MonoDrop getValue = null;
			if(_mapId.TryGetValue(keyUID,out getValue))
				return getValue;

			return null;
		}

		public MonoDrop GetMonoDropByIndex2(Index2 ixy)
		{
			MonoDrop getValue = null;
			if (_mapIndex2.TryGetValue (ixy, out getValue))
				return getValue;

			return null;
		}

		//Inefficiency code !!
		public Index2 FindEmptySquare(Index2 min, Index2 max)
		{
			//Debug.Log ("FindEmptySquare :" + min + " max:" + max);
			int maxColumn = max.ix - min.ix;
			int maxRow = max.iy - min.iy;

			Index2 key = new Index2(0,0);
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy + min.iy;
				
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					key.ix = ix + min.ix;
					if(null == this.GetMonoDropByIndex2(key))
					{
						return key;
					}
				}
			}

			return Index2.None;
		}

		public List<Index2> FindEmptySquares(Index2 min, Index2 max)
		{
			List<Index2> listEmptySquares = new List<Index2> ();
			
			int maxColumn = max.ix - min.ix;
			int maxRow = max.iy - min.iy;
			
			Index2 key = new Index2(0,0);
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy + min.iy;
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					key.ix = ix + min.ix;
					if(null == this.GetMonoDropByIndex2(key))
					{
						listEmptySquares.Add(key);	
					}
				}
			}

			return listEmptySquares;
		}
	
		//==============: method:========================================================================================

		public bool Add(int key, MonoDrop value)
		{
			if (null == value) 
			{
				Debug.LogError("Add : null == value");
				return false;
			}

			//-----------------------
			//add this
			if (_mapId.ContainsKey (key)) 
			{
				_mapId [key] = value;

			} else 
			{
				_mapId.Add (key, value);
			}

			//-----------------------
			//add mapIndex2 
			if (_mapIndex2.ContainsKey (value.index2D)) 
			{
				_mapIndex2[value.index2D] = value;
			}else
			{
				//CDefine.DebugLog("value.index2D : " + value.index2D); //chamto test
				_mapIndex2.Add (value.index2D, value);
			}

			return true;
		}

		public bool Remove(int key)
		{
			MonoDrop getValue = null;
			if (_mapId.TryGetValue (key, out getValue)) 
			{
				_mapIndex2.Remove(getValue.index2D);
			}

			return _mapId.Remove (key);
		}


		//==============: debug method:========================================================================================

		public void Debug_PrintMap()
		{
			
			CDefine.DebugLog ("_mapId------------------------------------------------- : " + _mapId.Count);
			for (int i=0; i<_mapId.Count; i++) 
			{
				CDefine.DebugLog (_mapId.Keys.ToList()[i].ToString() + "  drop: " + _mapId.Values.ToList()[i]);

			}

			CDefine.DebugLog ("_mapIndex2--------------------------------------------- : " + _mapIndex2.Count);
			for (int i=0; i<_mapIndex2.Count; i++) 
			{
				CDefine.DebugLog (_mapIndex2.Keys.ToList()[i].ToString() + "  drop: " + _mapIndex2.Values.ToList()[i]);
			}

		}

	}
	
}//end namespace
