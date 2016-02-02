
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;

using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
//using T_JoinsInLine = System.Collections.Generic.Dictionary<BundleWithDrop,System.Collections.Generic.List<MonoDrop>> ;
using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
using T_Bundle = System.Collections.Generic.Dictionary<PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;

//=====================================================================

/// <summary>
/// 같은 조건의 드롭에 대한, 덩어리 정보
/// </summary>
public class BundleData
{
	// ***** dataStructor of T_Bundle  **************************************
	//
	//bundle ------ joinsInLine<0,0> dir<1,0> ---------- dropsInLine<0~n,0>
	//              joinsInLine<0,1> dir<1,0>            dropsInLine<0~n,1>
	//              joinsInLine<0,2> dir<1,0>                .
	//                   .                                   .
	//                   .
	//
	// **********************************************************************
	//public Dictionary<PairIndex2, List<List<MonoDrop>>> lineBundle = null; 

	//지정된 조건의 선형으로 연속된 드롭정보를 저장
	public T_Bundle 					lines 	= null;

	//지정된 조건의 전체 드롭정보를 저장
	public Dictionary<Index2, MonoDrop> mapFull	= null;

	public List<BundleWithDrop>			listRefDrop = null;

	public bool ContainsDrop_FromMap(Index2 dropIdx)
	{
		if (null == mapFull)
						return false;

		MonoDrop drop = null;
		mapFull.TryGetValue (dropIdx, out drop);
		if (null == drop)
						return false;
		return true;
	}

	public void AddRefInfo(BundleWithDrop refInfo)
	{
		if (null == refInfo || null == listRefDrop)
						return;

		if (true == listRefDrop.Contains (refInfo))
						return;

		listRefDrop.Add (refInfo);
	}

	public void CopyRefInfoList(List<BundleWithDrop> src_refInfoList)
	{
		if (null == listRefDrop || null == src_refInfoList)
						return;

		foreach (BundleWithDrop getRef in src_refInfoList) 
		{
			AddRefInfo(getRef);
		}

	}

	public void UpdateRefInfoList_ToThis()
	{
		foreach (BundleWithDrop getRef in listRefDrop) 
		{
			if(null == getRef || null == getRef.refBundle) continue;
			
			getRef.refBundle = this;
		}
	}

	public void UpdateMap()
	{
		if (null == lines || null == mapFull)
						return;

		mapFull.Clear ();

		//Debug.Log ("mapFull keyList ----------------------------------------------------"); //chamto test

		MonoDrop addDrop = null;
		foreach (T_JoinsInLine joins in lines.Values) 
		{
			if(null == joins) {  CDefine.DebugLogError("Error !! : null == joins"); return; }

			foreach (T_DropsInLine drops in joins) 
			{
				if(null == drops) {  CDefine.DebugLogError("Error !! : null == drops"); return; }

				//foreach (MonoDrop monoDrop in drops) 
				for(int i=0;i<drops.Count;i++)
				{
					addDrop = drops.ElementAt(i);
					if(null == addDrop) {  continue; }

					if(true == mapFull.ContainsKey(addDrop.index2D))
					{
						mapFull[addDrop.index2D] = addDrop;
					}else 
					{
						mapFull.Add(addDrop.index2D, addDrop);
					}

					//Debug.Log ("mapFull keyList : " +addDrop.index2D.ToString ()); //chamto test
				}
			}
		}

	}//end

	public static void DismissLines(BundleData data)
	{
		if (null == data)
			return;
		
		if(null != data.lines)
		{
			//data.lines.Clear(); //다른 그룹에서 참조로 사용하므로, 사용안한다고 해제하면 안된다
			data.lines = null;
		}
	}

	public static void ClearData(BundleData data)
	{
		if (null == data)
						return;

		if(null != data.lines)
		{
			data.lines.Clear(); 
			data.lines = null;
		}

		if(null != data.mapFull)
		{
			data.mapFull.Clear();
			data.mapFull = null;
		}

		if(null != data.listRefDrop)
		{
			data.listRefDrop.Clear();
			data.listRefDrop = null;
		}
	}

	public static BundleData Create()
	{
		BundleData bundle = new BundleData();
		bundle.lines = new T_Bundle ();
		bundle.mapFull = new Dictionary<Index2, MonoDrop> ();
		bundle.listRefDrop = new List<BundleWithDrop> ();

		return bundle;
	}

	public String ToStringLines()
	{
		String buff = "lineCount :" + lines.Count;

		foreach (List<List<MonoDrop>> listlist in lines.Values) 
		{
			if(null == listlist) continue;

			buff = String.Concat (buff+"\nLines :");
			foreach (List<MonoDrop> list in listlist)
			{
				if(null == list) continue;
				
				foreach (MonoDrop drop in list)
				{
					if(null == drop) continue;
					
					buff = String.Concat (buff + " : " + drop.index2D);
				}
			}
		}
		return buff;
	}	
}

