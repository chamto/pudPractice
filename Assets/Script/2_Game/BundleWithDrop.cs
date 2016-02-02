
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
/// Bundle with Drop
/// </summary>
public class BundleWithDrop
{


	public BundleData refBundle = null; 

	public void UpdateBundleMap()
	{
		if (null != refBundle) 
		{
			refBundle.UpdateMap();
		}
	}

	public bool EqualRefBundle(BundleWithDrop dstGroup)
	{
		if (null == dstGroup)
						return false;
		if (null == dstGroup.refBundle)
						return false;
		if (this.refBundle != dstGroup.refBundle)
						return false;
		return true;
	}

	public void DissmissBundle()
	{
		//CDefine.DebugLog ("--------------------------------ClearBundle!!!!!!!!!!!!"); //chamto test
		if(null != this.refBundle)
		{
			BundleData.DismissLines(this.refBundle);
		}

		//this.refBundle = null; //chamto temp
	}

	//public void Add(Index2 firstIdxOfColumAndRow, List<MonoDrop> listJoinDrop)
	public void Add(PairIndex2 key_OriginAndDir, T_DropsInLine refAddDrops)
	{
		if (null == refBundle) 
		{
			CDefine.DebugLogError("error !!! : null == refBundle");
			return;				
		}

		//List<List<MonoDrop>> rows = null;
		T_JoinsInLine refJoins = null;
		if (refBundle.lines.TryGetValue (key_OriginAndDir, out refJoins)) 
		{

			if(null == refJoins)
			{
				refJoins = new T_JoinsInLine ();
				refBundle.lines.Add (key_OriginAndDir, refJoins);
			}

			//Duplicate check
			foreach(T_DropsInLine getDrops in refJoins)
			{
				if(null != getDrops)
				{
					if(getDrops == refAddDrops)
						return;
				}
			}

			refJoins.Add (refAddDrops);
		} else 
		{
			refJoins = new T_JoinsInLine ();
			refBundle.lines.Add (key_OriginAndDir, refJoins);
			refJoins.Add (refAddDrops);
		}
	}
	
	//Engraft srcGroup to dstGroup
	static public void EngraftBundleData(BundleWithDrop srcGroup , BundleWithDrop toDstGroup)
	{
		if (null == srcGroup || null == toDstGroup)
			return;

		//CDefine.DebugLog ("----Engraft : \n-->src : " + srcGroup.refBundle.ToStringLines() + " \n-->dst : " + toDstGroup.refBundle.ToStringLines()); //chamto test

		if (true == srcGroup.EqualRefBundle (toDstGroup))
			return;

		PairIndex2 key;
		//List<List<MonoDrop>> rows = null;
		T_JoinsInLine refJoins = null;
		for (int i=0; i< srcGroup.refBundle.lines.Values.Count; i++) 
		{
			key = srcGroup.refBundle.lines.Keys.ElementAt(i);
			refJoins = srcGroup.refBundle.lines.Values.ElementAt(i);

			if(null == refJoins) continue;

			foreach(T_DropsInLine getDrops in refJoins)
			{
				if(null == getDrops) continue;

				toDstGroup.Add(key, getDrops);
			}
		}

		toDstGroup.refBundle.CopyRefInfoList (srcGroup.refBundle.listRefDrop);

		//BundleData.Clear (srcGroup.refBundle);
		srcGroup.DissmissBundle ();
		//BundleData.DismissLines(this.refBundle);

		//Importance !! : Engraft the all refPointer
		//srcGroup.refBundle = toDstGroup.refBundle;
		toDstGroup.refBundle.UpdateRefInfoList_ToThis ();
		
	}
	
	static public BundleWithDrop Create(PairIndex2 key_OriginAndDir, T_DropsInLine refAddDrops)
	{
		BundleWithDrop group = new BundleWithDrop ();
		group.refBundle = BundleData.Create ();
		
		group.Add (key_OriginAndDir, refAddDrops);
		
		return group;
	}
}
