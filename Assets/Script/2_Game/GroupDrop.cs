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

	public class GroupDrop
	{
		//-------------------------------------
		//  BundleWithDrop ---- BundleData
		//  BundleWithDrop ----| 
		//  BundleWithDrop ----|
		private List<BundleData> 		m_listRefData = new List<BundleData>();
		private List<BundleWithDrop>	m_listRefDrop = new List<BundleWithDrop> ();
		private int 					m_nextCount = 0;
		//Queue<BundleData> m_q = new Queue<BundleData>();

		public bool ContainsDrop_FromMap(MonoDrop drop)
		{
			if (null == drop)
								return false;

			foreach (BundleData data in m_listRefData) 
			{
				if(null == data) continue;
				if(true == data.ContainsDrop_FromMap(drop.index2D)) return true;
			}

			return false;
		}

		public Dictionary<Index2,MonoDrop> Next()
		{
			//Debug.Log (m_listBundle.Count + " - group count"); //chamto test
			if (0 == m_listRefData.Count || m_nextCount >= m_listRefData.Count)
								return null;

			m_nextCount++;

			return m_listRefData[m_nextCount-1].mapFull;
		}

		public void AddRefData(BundleData data)
		{
			m_listRefData.Add (data);
		}

		public void AddRefDrop(BundleWithDrop refDrop)
		{
			m_listRefDrop.Add (refDrop);
		}

		public void InitNextCount ()
		{
			m_nextCount = 0;
		}

		public int GetNextCount()
		{
			return m_nextCount;
		}

		public int GetMaxCount()
		{
			return m_listRefData.Count;
		}

		public void UpdateMap()
		{
			List<BundleData> deathList = null;
			int idx = 0;
			foreach (BundleData data in m_listRefData) 
			{

				if(null != data && null != data.lines && 0 != data.lines.Count)
				{
					data.UpdateMap();
				}

				if(null == data || null == data.lines || 0 == data.lines.Count)
				{
					if(null == deathList)
						deathList = new List<BundleData>();

					deathList.Add(data);

				}

				idx++;
			}

			//Debug.Log("GroupDrop : UpdateMap"); //chamto test
			if (null != deathList) 
			{
				foreach (BundleData deathData in deathList) 
				{
					//Debug.Log(deathList.Count+"   " + m_listBundle.Count); //chamto test
					m_listRefData.Remove(deathData);
				}			
			}

			this.PrintListBundle_mapFull (); //chamto test

		}

		public void DismissRefDrop()
		{
			foreach (BundleWithDrop refDrop in m_listRefDrop) 
			{
				if(null == refDrop) continue;

				refDrop.refBundle = null;
			}
		}

		public void Clear()
		{
			InitNextCount ();

			foreach (BundleData data in m_listRefData) 
			{
				if(null == data) continue;

				BundleData.ClearData(data);
			}
			m_listRefData.Clear ();

			DismissRefDrop ();
			m_listRefDrop.Clear ();
		}

		public void PrintListBundle_lines()
		{

			foreach (BundleData data in m_listRefData) 
			{
				if (null == data || null == data.lines)
					continue;

				Debug.Log ("----------PrintListBundle------------"+m_listRefData.Count);
				Debug.Log (data.ToStringLines());

			}
			
		}

		public void PrintListBundle_mapFull()
		{
			String buff = "";
			foreach (BundleData data in m_listRefData) 
			{
				if (null == data || null == data.lines || null == data.mapFull)
					continue;
				
				buff = "";
				Debug.Log ("----------PrintListBundle------------"+m_listRefData.Count);
				foreach (Index2 idx in data.mapFull.Keys) 
				{
					buff = String.Concat (buff + " : " + idx);
				}
				Debug.Log (buff);
			}
			
		}

	}

	/// <summary>
	/// C drop manager.
	/// 드롭을 생성/제거/배치 하는 관리 객체
	/// </summary>
	//end class
}//end namespace
