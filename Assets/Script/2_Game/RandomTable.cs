using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PuzzAndBidurgi
{
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
	
}//end namespace
