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



	public enum eMapKind
	{
		eGRID_MAP = 0,
		eHEX_MAP = 1,
	}
	
	public struct Hex3
	{}

	public struct PercentItem
	{
		public int dropKind;

		public override string ToString()
		{
			return dropKind.ToString();
		}
	}

	public class RandomTable<T> where T : struct
	{

		//변동백분율 항목 표시값
		//private const float	    FLAG_TRANS_PERCENT_VALUE = 100.0f; 

		//최대 정수퍼센트지값
		private const Int32  	MAX_IP_VALUE = 1000000;

		//fpToip : 실수 퍼센트지를 정수퍼센트지로 변환
		private const float  	FPERCENT_TO_IPERCENT = (float)MAX_IP_VALUE / 100.0f;

		private System.Random 	_random = new System.Random();


		//원본 실수값  <항목 , 전체에서 항목이 나올 확률>
		//외부에서 보는 값 , 가공되지 않은 값
		private Dictionary<T, float> 	_outTableFp = new Dictionary<T, float>();

		//실수값을 정수값으로 변환
		//내부에서 확률구간검사용으로 사용되는 값 , 가공된 값
		private Dictionary<T, UInt32> 	_inTableIp = new Dictionary<T, UInt32>();

		//변동백분율 목록 : translating percentage value
		//변동백분율값이 어떤것인지 확인하는 용도임 
		private List<T> 				_tpvList = new List<T> ();


		private bool 					_needCalcTable = true;


		public Int32 GetTableCount()
		{
			return _outTableFp.Count;
		}

		public void ClearTable()
		{
			_outTableFp.Clear ();
			_tpvList.Clear ();

			this.clearInnerTable ();
		}

		private void clearInnerTable()
		{
			_inTableIp.Clear ();

			_needCalcTable = true;
		}

		public bool Update(T key, float value)
		{
			float getValue = 0;
			if (true == _outTableFp.TryGetValue (key, out getValue)) 
			{
				_outTableFp[key] = value;
				_needCalcTable = true;

				return true;
			}

			return false;
		}


		public void ActiveTpv(T key, bool isTpv)
		{
			float outValue = 0;
			if (true == _outTableFp.TryGetValue(key,out outValue))
			{
				if(true == isTpv)
				{
					_tpvList.Add(key);
					_needCalcTable = true;
				}
			}

			if(false == isTpv)
			{
				_tpvList.Remove(key);
				_needCalcTable = true;
			}
		}

		public void Remove(T key)
		{
			_outTableFp.Remove (key);
			_needCalcTable = true;

			this.ActiveTpv (key, false);

		}

		public void Add(T key, float value)
		{
			_outTableFp.Add (key, value);
			_needCalcTable = true;
		}

		//isTpv : 처음 값을 넣을때 변동백분율값인지 표시해서 넣어준다.
		public void Add(T key, float value, bool isTpv)
		{
			this.Add (key, value);

			if (true == isTpv) 
			{
				this.ActiveTpv(key, isTpv);
			}
		}


		private void calcFp1ToFp2()
		{
			//1 -- 		calcFp1ToFp2        --
			//fp 1차값 : 0.2 100 0.2 100 0.6 : count 5
			//확률이 100인 항목은 계산에서 제외한다. 항목이 100이면 변동확률을 사용
			//100 - (0.2 + 0.2 + 0.6) => 99
			//변동확률 구하기 : 99 / 2(변동확률 항목수) = 49.5
			//fp 2차값 : 0.2 49.5 0.2 49.5 0.6

			float sum = 0;
			float avgTpv = 0;
			foreach (KeyValuePair<T,float> keyValue in _outTableFp) 
			{
				sum += keyValue.Value;
			}
			foreach (T key in _tpvList) 
			{
				sum -= _outTableFp[key];
			}


			if (0 == _tpvList.Count)
				avgTpv = (100f - sum);
			else
				avgTpv =(100f - sum) / _tpvList.Count;


			foreach (T key in _tpvList) 
			{
				_outTableFp[key] = avgTpv;
			}

			float test = sum + (avgTpv * _tpvList.Count);
			//CDefine.DebugLog (test); //chamto test
			if (Mathf.Abs (100f - test) >= float.Epsilon) 
			{
				CDefine.DebugLogError("invalid totalSumValue of _outTableFp. must be clost to 100Percent: " + test);			
			}

		}

		private void calcFp2ToIp()
		{
			//2 -- 		calcFp2ToIp        --
			//최대 정수퍼센트지값 : max 1000 
			//fp100 * fpToip10 = ip1000
			//ip1000 / fp100 = fpToip10
			//ip     2   495 2 495  6

			//확률구간이 적용된 ip로 바꾸기
			//현재ip + 다음ip = 다음ipRange 
			//ipRange  2   497 499 994 1000

			UInt32 rangeValue = 0;
			UInt32 prevValue = 0;
			foreach (KeyValuePair<T,float> keyValue in _outTableFp) 
			{
				rangeValue = (UInt32)(keyValue.Value * FPERCENT_TO_IPERCENT);
				rangeValue += prevValue;
				_inTableIp.Add(keyValue.Key, rangeValue);
				prevValue = rangeValue;
			}

			//백분율을 넘어서는 확률 구간이 있으면 안된다. 
			if (MAX_IP_VALUE < rangeValue) 
			{
				CDefine.DebugLogError("invalid range value. must be close to MAX_IP_VALUE: " + rangeValue);
			}

		}

		public void CalcTable()
		{
			//fp : 실수퍼센트지 , ip : 정수퍼센트지
			//fpToip : 실수 퍼센트지를 정수퍼센트지로 변환
			
			//1 -- 		calcFp1ToFp2        --
			//fp 1차값 : 0.2 100 0.2 100 0.6 : count 5
			//확률이 100인 항목은 계산에서 제외한다. 항목이 100이면 변동확률을 사용
			//100 - (0.2 + 0.2 + 0.6) => 99
			//변동확률 구하기 : 99 / 2(변동확률 항목수) = 49.5
			//fp 2차값 : 0.2 49.5 0.2 49.5 0.6
			
			//2 -- 		calcFp2ToIp        --
			//최대 정수퍼센트지값 : max 1000 
			//fp100 * fpToip10 = ip1000
			//ip1000 / fp100 = fpToip10
			//ip     2   495 2 495  6

			//확률구간이 적용된 ip로 바꾸기
			//현재ip + 다음ip = 다음ipRange 
			//ipRange  2   497 499 994 1000

			this.clearInnerTable ();
			this.calcFp1ToFp2 ();
			this.calcFp2ToIp ();

			_needCalcTable = false;
		}


		public T GetRandValue()
		{
			if (true == _needCalcTable) 
			{
				CDefine.DebugLogWarning("CalcTable function must be called");
			}

			T rValue = default(T);

			Int32 rand = _random.Next (0, MAX_IP_VALUE+1);
			//CDefine.DebugLog ("rand :" + rand); //chamto test

			UInt32 prevValue = 0;
			foreach (KeyValuePair<T,UInt32> kv in _inTableIp) 
			{
				if( prevValue <= rand && rand < kv.Value)
				{
					rValue = kv.Key;
				}

				prevValue = kv.Value;
			}

			return rValue;
		}

		public void PrintValue()
		{
			UInt32 sumIp = 0;
			float sumFp = 0; 
			string strFpTable = "";
			string strIpTable = "";
			foreach (KeyValuePair<T,float> kv in _outTableFp) 
			{
				sumFp += kv.Value;
				strFpTable += "[" + kv.Key + "] " + kv.Value + " | ";
			}
			foreach (KeyValuePair<T,UInt32> kv in _inTableIp) 
			{
				sumIp += kv.Value;
				strIpTable += "[" + kv.Key + "] " + kv.Value + " | ";
			}
			CDefine.DebugLog(strFpTable);	
			strIpTable += "count:" + _inTableIp.Count;
			CDefine.DebugLog(strIpTable);	
			CDefine.DebugLog ("IPSum : " + sumIp + "  FPSum : " + sumFp + "  tpvCount : " + _tpvList.Count);


		}

		public static void PrintProbabilityDistribution(RandomTable<PercentItem> randTable, UInt16 tryCount)
		{
			if (null == randTable)
								return;

			PercentItem item;
			ArrayList disb = new ArrayList();
			for (int i=0; i< randTable.GetTableCount(); i++) 
			{
				disb.Add(0);
			}

			for (UInt16 i=0; i<tryCount; i++) 
			{
				item  = randTable.GetRandValue();
				disb[item.dropKind] = (int)disb[item.dropKind] + 1;
				//temp += this.GetRandValue() + "  |";
			}
			
			string temp = "distribution : ";
			for (int i=0; i< disb.Count; i++) 
			{
				temp += "[" + i + "] " + disb[i] + " | ";
			}
			CDefine.DebugLog (temp);

		}

		public static void Test()
		{
			PercentItem item;
			RandomTable<PercentItem> randTable = new RandomTable<PercentItem> ();
			item.dropKind = 0;
			randTable.Add (item, 0.2f);
			
			item.dropKind = 1;
			randTable.Add (item, 20f);
			
			item.dropKind = 2;
			randTable.Add (item, 100f, true);
			
			item.dropKind = 3;
			randTable.Add (item, 100f, true);
			
			item.dropKind = 4;
			randTable.Add (item, 15f);
			
			randTable.CalcTable ();
			randTable.PrintValue();
			RandomTable<PercentItem>.PrintProbabilityDistribution (randTable, 1000);
			RandomTable<PercentItem>.PrintProbabilityDistribution (randTable, 1000);

			CDefine.DebugLog ("");
			/*
			item.dropKind = 1;
			randTable.Update (item, 50f);
			item.dropKind = 2;
			randTable.Update (item, 0.8f);
			randTable.ActiveTpv (item, false);
			item.dropKind = 5;
			randTable.Add (item, 0, true);
			item.dropKind = 6;
			randTable.Add (item, 0, true);
			item.dropKind = 7;
			randTable.Add (item, 0, true);
			item.dropKind = 8;
			randTable.Add (item, 0, true);
			randTable.CalcTable ();
			randTable.PrintValue();
			RandomTable<PercentItem>.PrintProbabilityDistribution (randTable, 1000);
			*/


		}
	}

	//"model" - controller - view
	public class DropMap
	{

		private UInt16 _sequenceId = 0;

		private System.Random 		_random = new System.Random();

		//격자형 지도 인덱스
		private Dictionary<Index2,DropInfo> _map = new Dictionary<Index2,DropInfo>();

		//육각형 지도 인덱스
		//private Dictionary<Hex3,Int32> 	_hexMap = new Dictionary<Hex3,Int32>();

		private UInt16 _width = 0;
		private UInt16 _height = 0;

		public UInt16 GetMapWidth() 
		{
			return _width;
		}
		public UInt16 GetMapHeight() 
		{
			return _height;
		}


		//20160221 chamto - fix me : 지정한 드롭목록에서 드롭별 확률에 맞게 반환되게 수정되어야 한다.
		public DropInfo.eKind GetRandDrop(byte max_kind)
		{
			
			return (DropInfo.eKind)_random.Next (1, max_kind);
		}


		public void CreateDropMap(UInt16 width, UInt16 height, Index2 startPos)
		{
			UInt16 MAP_SIZE = (UInt16)(width * height);

			_width = width;
			_height = height;

			DropInfo.eKind defaultKind = DropInfo.eKind.Heart;
			Index2 ixy = startPos;
			DropInfo dropInfo = null;
			for(UInt16 i=0 ; i < MAP_SIZE ; i++)
			{
				ixy.ix = (int)(i% width); 
				ixy.iy = (int)(i/ height); 

				dropInfo = this.createDropInfo(defaultKind);
				this.AddValue(ixy, dropInfo);

			}
		
		}

		private DropInfo createDropInfo(DropInfo.eKind ekind)
		{
			DropInfo drop = new DropInfo ();
			drop.id = _sequenceId++;
			drop.kind = ekind;

			return drop;
		}


		//minLength : 조각의 최소길이 (퍼드에서는 3이다.)
		public bool FindPiece (Index2 start, Index2 end, int minLength, out List<int> countList)
		{
			bool findPiece = false;
			Index2 current = start;

			Index2 dir = end - start;
			//0이 아니라면 길이를 1로 만든다.
			if(dir.ix != 0) dir.ix = dir.ix / dir.ix; 
			if(dir.iy != 0) dir.iy = dir.iy / dir.iy;

			int count = 1;
			DropInfo.eKind prevKind = _map[current].kind;
			countList = new List<int> ();
			while ((end-start).LengthSquared() > (current-start).LengthSquared()) 
			{

				current = current + dir; //next position

				if(prevKind == _map[current].kind)
				{
					count++;

					//조각의 최소길이를 만족하는 것이 있는 경우 : 반환값을 ‘참’으로 설정한다. 
					if(count >= minLength)
						findPiece = true;

					//전에 것과 같고, 마지막 위치일 경우 : 조각의 수를 찾은목록에 추가하고 반복문을 빠져나온다.
					if(current == end)
					{
						countList.Add(count);

						break;
					}
				}else
				{
					countList.Add(count);

					count = 1;  //init count
				}//end if


				prevKind = _map[current].kind;

			}//end while

			return findPiece;
		}


		public bool AddValue(Index3 key_index , DropInfo value)
		{
			DropInfo getValue = null;
			if (true == _map.TryGetValue (key_index, out getValue)) 
			{
				return false;
			}
			
			_map.Add(key_index, value);
			return true;
		}

		public bool SetValue(Index3 key_index , DropInfo value)
		{
			
			DropInfo getValue = null;
			if (false == _map.TryGetValue (key_index, out getValue)) 
			{
				return false;
			}
			
			_map[key_index] = value;
			return true;
		}

		public bool TryGetValue(Index2 key_index , out DropInfo getValue)
		{
			if (true == _map.TryGetValue (key_index, out getValue)) 
			{
				if(null != _map[key_index])
				{
					getValue =  _map[key_index];
					return true;
				}
				
			}

			getValue = null;
			return false;
		}

		public void Init()
		{
			//_map.Select (
			//	drop => {if(null != drop.Value) drop.Value.Init();}
			//);

			foreach (DropInfo value in _map.Values) 
			{
				if(null != value) value.Init();
			}
		}

		public void Clear(Index2 key_index)
		{
			DropInfo getValue = null;
			if(true == this.TryGetValue(key_index , out getValue))
			{
				this.SetValue (key_index, null);
			}

		}

		public void ClearAll()
		{
			foreach (Index2 key_index in _map.Keys) 
			{
				this.SetValue (key_index, null);
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
