
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;


namespace PuzzAndBidurgi
{


	using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
	//using T_JoinsInLine = System.Collections.Generic.Dictionary<BundleWithDrop,System.Collections.Generic.List<MonoDrop>> ;
	using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
	using T_Bundle = System.Collections.Generic.Dictionary<PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;

	//Dictionary<PairIndex2, List< List<MonoDrop>>>

	namespace Fitting
	{

		public struct PairIndex2
		{
			public Index2 origin;
			public Index2 direction;
			
			public PairIndex2(Index2 _origin, Index2 _direction)
			{
				origin = _origin;
				direction = _direction;
			}
		}




		public class GroupNumber
		{}




		//Piece in Line (Line is column or row in board)
		public class Piece
		{
			//드롭의 종류
			public DropInfo.eKind kind;

			//활성조건을 만족하는 드롭의 시작, 끝 위치
			public Index2 start;
			public Index2 end;

		}

		//Lines : Piece + Piece + .....
		public class Lines : List<Piece>
		{

		}


		/// <summary>
		/// Bundle : Lines1 + Lines2 + Lines3 ...
		/// 
		/// 활성조건을 만족하는 한덩어리의 드롭들
		/// 
		/// 번들에는 활성조건을 만족하는 같은 종류의 드롭이 한덩어리만 들어가야 한다.
		/// </summary>
		public class Bundle
		{
			public UInt16 id;

			// ---> row
			public Dictionary<Index2, Lines> row = new Dictionary<Index2, Lines> ();

			// ^ column
			public Dictionary<Index2, Lines> column = new Dictionary<Index2, Lines> ();

			//row + column
			public List<Index2> rowColumn = new List<Index2> ();
		}

		/*
		public class FittingList
		{
			public List<Bundle> bundles = new List<Bundle> ();


			private bool availableJoin(MonoDrop start , out int jump)
			{
				if(null == d_start)
				{
					d_jump = 1;
					return false;
				}
				
				Index2 d_next = d_start.index2D;
				MonoDrop d_nextDrop = null;
				for(int i=0;i<minJoin-1;i++)
				{
					d_next.ix += (int)direction.x;
					d_next.iy += (int)direction.y;
					d_nextDrop = this.mapDrop.GetMonoDropByIndex2(d_next);
					if(false == this.boardInfo.BelongToViewArea(d_next))
						d_nextDrop = null;

					if(null == d_nextDrop || d_start.dropKind != d_nextDrop.dropKind)
					{
						d_jump = i+1;
						return false;
					}
				}
				
				d_jump = 0;
				return true;
			}

//20160221 chamto - first job : DropMap의 드롭생성 처리를 만든후 , 연관해서 작성해야 한다. 
			public Lines LineInspect(Index2 start, Index2 direction, UInt16 length, UInt16 minJoin)
			{

				UInt16 jumpCount = 0;
				Index2 compareIndex = start;
				Index2 nextIndex = start;
				MonoDrop nextDrop = null;
				MonoDrop compareDrop = this.mapDrop.GetMonoDropByIndex2(compareIndex);
				
				List<MonoDrop> drops = null;
				List<List<MonoDrop>> joins = new List<List<MonoDrop>> ();
				
				//int findGroupCount = 0;
				BundleWithDrop refDrop = null;
				BundleWithDrop findGroup = null;
				
				
				PairIndex2 key_pairIdx;
				direction.Normalize ();
				key_pairIdx.direction.ix = (int)direction.x;
				key_pairIdx.direction.iy = (int)direction.y;
				key_pairIdx.origin = startIxy;
				
				//CDefine.DebugLog ("----------LineInspection : dir : " + direction + "----------"); //chamto test
				for (int i=0; i <= lengthOfLine; i++) 
				{
					
					nextIndex.ix = startIxy.ix + (int)(direction.x) * i;
					nextIndex.iy = startIxy.iy + (int)(direction.y) * i;
					nextDrop = this.mapDrop.GetMonoDropByIndex2(nextIndex);
					if(false == this.boardInfo.BelongToViewArea(nextIndex))
						nextDrop = null;
					
					
					//if(0 == drops.Count)
					if(null == drops)
					{
						//-------------------------------------------
						//라인 시작지점에서 검사
						if(false == d_availableJoin(compareDrop, out jumpCount))
						{
							//Debug.Log ("avJoin => false : compareIndex:"+compareIndex+" nextIxy:"+nextIndex+" compareDrop:"+compareDrop+" nextDrop:"+nextDrop+" jumpCount:"+jumpCount); //chamto test
							
							i+=jumpCount-1;
							compareIndex.ix = compareIndex.ix + (int)(direction.x) * jumpCount;
							compareIndex.iy = compareIndex.iy + (int)(direction.y) * jumpCount;
							compareDrop = this.mapDrop.GetMonoDropByIndex2(compareIndex);
							if(false == this.boardInfo.BelongToViewArea(compareIndex))
								compareDrop = null;
							
							//CDefine.DebugLog(i+" - compareIndex : "+compareIndex); //chamto test
							
							continue;
						}
						
						
						drops = new List<MonoDrop>();
						joins.Add(drops);
						refDrop = BundleWithDrop.Create(key_pairIdx,drops);
						refDrop.refBundle.AddRefInfo(refDrop);
						m_groupDrop.AddRefData(refDrop.refBundle);
						m_groupDrop.AddRefDrop(refDrop);
						//findGroupCount = 0;
						
					}
					
					
					if(null != compareDrop)
					{
						//-------------------------------------------
						//연속되어 배치된 드롭이 있다면 드롭종류가 같은지 검사한다.
						if(null != nextDrop && compareDrop.dropKind == nextDrop.dropKind)
						{
							//next add , end is not processed
							drops.Add(nextDrop);
							nextDrop.m_bundleInfo = refDrop;
							//Debug.Log("i " + i +"LineInspection  listJoin.Count : " + listJoin.Count + "  index:" + nextIndex.ToString()); //chamto test
							
							
							//한번 이상 찾았으면, 더이상 찾지 않는다
							//if(0 == findGroupCount)
							{
								findGroup = FindJoinGroup_InFourWay(nextDrop);
								if(null != findGroup)
								{
									//CDefine.DebugLog(nextDrop.index2D+ ": dir : " + direction +" : findJoinGroup : "+ findGroup + " refBundle : " + findGroup.refBundle);//chamto test
									if(null == findGroup.refBundle.lines)
									{
										CDefine.DebugLogWarning("!!!! null == findGroup.refBundle.lines");
									}
									//CDefine.DebugLog(findGroup.refBundle.ToStringLines());
									BundleWithDrop.EngraftBundleData(refDrop, findGroup);
									//findGroupCount++;
								}
							}
							
						}else
						{
							drops = null;
							i-=1;
						}
					}		
					
					compareIndex = nextIndex;
					compareDrop = nextDrop;
					
				}
				
				return joins;
			}

		}*/
	}





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
}
