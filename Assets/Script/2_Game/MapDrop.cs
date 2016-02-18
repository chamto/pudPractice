using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PuzzAndBidurgi
{
	using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
	using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
	using T_Bundle = System.Collections.Generic.Dictionary<PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;



	static public class CONST_VALUE
	{
		public const Int32 UID_NULL = -1;
	}


	public class DropList<T>
	{
		private Dictionary<Int32, T> 	_mapId = new Dictionary<Int32, T> ();

		public bool Add(Int32 key, T value)
		{
			if (null == value)  
			{
				Debug.LogError("Add : null == value");
				return false;
			}


			if (true == _mapId.ContainsKey (key)) 
			{
				Debug.LogError("Add : true == _mapId.ContainsKey  " + key); 
				return false;
				
			} 

			_mapId.Add (key, value);

			return true;
		}
		
		public bool Remove(int key)
		{
			
			return _mapId.Remove (key);
		}

		public T GetValue(int keyUID) 
		{
			T getValue;
			//if(_mapId.TryGetValue(keyUID,out getValue))
			//	return getValue;
			_mapId.TryGetValue(keyUID,out getValue);

			return getValue;
		}

		//==============: debug method:========================================================================================
		
		public void Debug_PrintMap()
		{
			
			CDefine.DebugLog ("_mapId------------------------------------------------- : " + _mapId.Count);
			for (int i=0; i<_mapId.Count; i++) 
			{
				CDefine.DebugLog (_mapId.Keys.ToList()[i].ToString() + "  drop: " + _mapId.Values.ToList()[i]);
				
			}
		}
	}

	//model - "controller" - view
	public class DropController
	{

		private int m_sequenceId = 0;

		private DropList<MonoDrop> _dropList = new DropList<MonoDrop> ();
		private MapSet			   _mapSet = new MapSet();


		private MonoDrop createMonoDrop(Transform parent , eResKind eDrop , Vector3 localPos)
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
			

			drop.id = m_sequenceId++;

			drop.index2D = Single.DropMgr.boardInfo.GetPositionToIndex2D (localPos);
			drop.setDropKind = eDrop;
			
			//Specify the parent object
			drop.transform.parent = parent;
			drop.name = "drop" + drop.id;
			
			//[주의!!] 부모에 대한 상대좌표를 지정해야 하기 때문에 localposition 을 써야 한다.  
			drop.transform.localPosition = localPos;
			
			
			//todo modify that localposition
			drop.gotoLocalPosition = localPos;

			drop.SetColor(Color.gray);
			//drop.SetColor(Color.blue);
			//drop.GetBoxCollider2D().enabled = false; //20150212 chamto - 터치입력을 못받게 충돌체를 비활성 시켜 놓는다.
			
			//20150331 chamto test
			//drop.testIndex2.ix = drop.index2D.ix;
			//drop.testIndex2.iy = drop.index2D.iy;
			drop.m_textMesh_Index2 = MonoDrop.Add3DText (drop.transform, drop.index2D.ToString (), Color.white, new Vector3(-0.5f,0,-2f));
			//Index2 localIdx = Single.DropMgr.Board.GetPositionToIndex2D (drop.gotoLocalPosition);
			//drop.m_textMesh_LocalIdx = MonoDrop.Add3DText (drop.transform, localIdx.ToString(), Color.red, new Vector3(-0.5f,-0.3f,-2f));
			
			
			return drop;
		}


		public void AddDrop(eResKind eDrop , Vector3 localPos)
		{
			MonoDrop pDrop = null;
			pDrop = this.createMonoDrop(Single.OBJRoot.transform, 
			                        eDrop,
			                        localPos);

			_dropList.Add (pDrop.id, pDrop);

		}

		public bool RemoveDrop(MonoDrop drop)
		{
			if (null == drop)
				return false;

			_dropList.Remove (drop.id);

			MonoBehaviour.Destroy(drop.gameObject);

			return true;
		}

		//Inefficiency code !!
		public Index2 FindEmptySquare(Index2 min, Index2 max)
		{
			//Debug.Log ("FindEmptySquare :" + min + " max:" + max);
			int maxColumn = max.ix - min.ix;
			int maxRow = max.iy - min.iy;

			Int32 getValue;
			Index2 idx = new Index2(0,0);
			for (int iy=0; iy <= maxRow; iy++) 
			{
				idx.iy = iy + min.iy;
				
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					idx.ix = ix + min.ix;
					if(false == this._mapSet.TryGetValue(idx,out getValue))
					{
						return idx;
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

			Int32 getValue;
			Index2 idx = new Index2(0,0);
			for (int iy=0; iy <= maxRow; iy++) 
			{
				idx.iy = iy + min.iy;
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					idx.ix = ix + min.ix;
					if(false == this._mapSet.TryGetValue(idx,out getValue))
					{
						listEmptySquares.Add(idx);	
					}
				}
			}
			
			return listEmptySquares;
		}

	}



	public enum eMapKind
	{
		eGRID_MAP = 0,
		eHEX_MAP = 1,
	}
	
	public struct Hex3
	{}

	//"model" - controller - view
	public class MapSet
	{

		//격자형 지도 인덱스
		private Dictionary<Index3,Int32> _map = new Dictionary<Index3,Int32>();

		//육각형 지도 인덱스
		//private Dictionary<Hex3,Int32> 	_hexMap = new Dictionary<Hex3,Int32>();

		public bool SetValue(Index3 key_index , Int32 value_UId)
		{

			Int32 getValue = CONST_VALUE.UID_NULL;
			if (false == _map.TryGetValue (key_index, out getValue)) 
			{
				_map.Add(key_index, value_UId);
			}
			
			_map[key_index] = value_UId;
			return true;
		}

		public bool TryGetValue(Index3 key_index , out Int32 getValue)
		{
			if (true == _map.TryGetValue (key_index, out getValue)) 
			{
				if(CONST_VALUE.UID_NULL != _map[key_index])
				{
					getValue =  _map[key_index];
					return true;
				}
				
			}

			getValue = CONST_VALUE.UID_NULL;
			return false;
		}

		public void Clear(Index3 key_index)
		{
			Int32 getValue = CONST_VALUE.UID_NULL;
			if(true == this.TryGetValue(key_index , out getValue))
			{
				this.SetValue (key_index, CONST_VALUE.UID_NULL);
			}

		}

		public void ClearAll()
		{
			foreach (Index3 key_index in _map.Keys) 
			{
				this.SetValue (key_index, CONST_VALUE.UID_NULL);
			}
		}

		
		//==============: debug method:========================================================================================
		
		public void Debug_PrintMap()
		{
			CDefine.DebugLog ("_map--------------------------------------------- : " + _map.Count);
			for (int i=0; i<_map.Count; i++) 
			{
				CDefine.DebugLog (_map.Keys.ToList()[i].ToString() + "  UID: " + _map.Values.ToList()[i]);
			}
			
		}

	}


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
