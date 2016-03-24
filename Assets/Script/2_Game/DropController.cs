using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PuzzAndBidurgi
{

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

		//private int m_sequenceId = 0;

		//model - controller - “view” : 출력UI드롭
		private DropList<MonoDrop> _monoDropList = new DropList<MonoDrop> ();

		//"model" - controller - view : 드롭정보를 가지고 있는 데이타
		private DropMap			   _dropMap = new DropMap();

		public MonoDrop CreateMonoDrop(Transform parent , DropInfo dropInfo , Vector3 localPos)
		{
			GameObject newObj = CResoureManager.CreatePrefab(SResDefine.pfDROPINFO);
			if(null == newObj) 
			{
				CDefine.DebugLogError(string.Format("Failed to create Prefab : " + SResDefine.pfDROPINFO));
				return null;
			}
			MonoDrop monoDrop = newObj.GetComponent<MonoDrop>();
			if(null == monoDrop) 
			{
				CDefine.DebugLogError(string.Format("MonoDrop is null"));
				return null;
			}
			
			//-------------------------------------------------

			monoDrop.id = dropInfo.id;
			//monoDrop.setDropKind = dropInfo.kind;  //20160221 chamto - fix me : eKind 를 eResKind 로 변환하여 넘겨야 한다. 변환함수 추가하기

			monoDrop.index2D = Single.DropMgr.boardInfo.GetPositionToIndex2D (localPos);
			
			//Specify the parent object
			monoDrop.transform.parent = parent;
			monoDrop.name = "drop" + monoDrop.id.ToString("x5");
			
			//[주의!!] 부모에 대한 상대좌표를 지정해야 하기 때문에 localposition 을 써야 한다.  
			monoDrop.transform.localPosition = localPos;
			
			//todo modify that localposition
			monoDrop.gotoLocalPosition = localPos;

			monoDrop.SetColor(Color.gray);
			//drop.SetColor(Color.blue);
			//drop.GetBoxCollider2D().enabled = false; //20150212 chamto - 터치입력을 못받게 충돌체를 비활성 시켜 놓는다.
			
			//20150331 chamto test
			//drop.testIndex2.ix = drop.index2D.ix;
			//drop.testIndex2.iy = drop.index2D.iy;
			monoDrop.m_textMesh_Index2 = MonoDrop.Add3DText (monoDrop.transform, monoDrop.index2D.ToString (), Color.white, new Vector3(-0.5f,0,-2f));
			//Index2 localIdx = Single.DropMgr.Board.GetPositionToIndex2D (drop.gotoLocalPosition);
			//drop.m_textMesh_LocalIdx = MonoDrop.Add3DText (drop.transform, localIdx.ToString(), Color.red, new Vector3(-0.5f,-0.3f,-2f));
			
			return monoDrop;
		}

		public void AddDrop(DropInfo dropInfo , Vector3 localPos)
		{
			MonoDrop pDrop = null;
			pDrop = this.CreateMonoDrop(Single.OBJRoot.transform, 
			                        dropInfo,
			                        localPos);

			_monoDropList.Add (dropInfo.id, pDrop);

		}

		public bool RemoveDrop(MonoDrop drop)
		{
			if (null == drop)
				return false;

			_monoDropList.Remove (drop.id);

			MonoBehaviour.Destroy(drop.gameObject);

			return true;
		}

		//Inefficiency code !!
		public Index2 FindEmptySquare(Index2 min, Index2 max)
		{
			//Debug.Log ("FindEmptySquare :" + min + " max:" + max);
			int maxColumn = max.ix - min.ix;
			int maxRow = max.iy - min.iy;

			DropInfo getValue;
			Index2 idx = new Index2(0,0);
			for (int iy=0; iy <= maxRow; iy++) 
			{
				idx.iy = iy + min.iy;
				
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					idx.ix = ix + min.ix;
					if(false == this._dropMap.TryGetValue(idx,out getValue))
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

			DropInfo getValue;
			Index2 idx = new Index2(0,0);
			for (int iy=0; iy <= maxRow; iy++) 
			{
				idx.iy = iy + min.iy;
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					idx.ix = ix + min.ix;
					if(false == this._dropMap.TryGetValue(idx,out getValue))
					{
						listEmptySquares.Add(idx);	
					}
				}
			}
			
			return listEmptySquares;
		}

	}

//	public enum eMapKind
//	{
//		eGRID_MAP = 0,
//		eHEX_MAP = 1,
//	}
//	
//	public struct Hex3
//	{}

	//"model" - controller - view
	
}//end namespace
