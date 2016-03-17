
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


		public class Piece
		{
			public enum eDirection
			{
				NONE	= 0,
				ROW		= 1,
				COLUMN	= 2,
			};

			//드롭의 종류
			public DropInfo.eKind kind;

			//강화드롭수
			public int reinforce; 

			//활성조건을 만족하는 드롭의 시작, 끝 위치
			public Index2 start;
			public Index2 end;
			public Index2 dir;
			public int length;

			//조각의 늘어진 방향
			//public eDirection direction;


			static public ML.LineSegment3 ToLine(Piece p)
			{
				ML.LineSegment3 a;
				a.origin = new ML.Vector3 (p.start.ix, p.start.iy, 0);
				a.direction = new ML.Vector3 (p.end.ix - p.start.ix, p.end.iy - p.start.iy, 0);
				a.last = new ML.Vector3 (p.end.ix, p.end.iy, 0);

				return a;
			}


			public bool IsJoin(Piece p)
			{
				ML.Vector3 pt1, pt2;
				Index2 idx1, idx2, diff;

				ML.LineSegment3.ClosestPoints(out pt1, out pt2, Piece.ToLine (this), Piece.ToLine (p));

				idx1 = Index2.Vector3ToIndex2 (pt1, 1, 1);
				idx2 = Index2.Vector3ToIndex2 (pt2, 1, 1);

				diff = idx1 - idx2;
				diff.ix = Math.Abs (diff.ix);
				diff.iy = Math.Abs (diff.iy);

				//두 드롭조각이 겹쳐있거나 붙어있는 경우를 찾는다.
				//특정 축에 대하여 점의차가 0이면 두 선분은  특정 축에 겹쳐있다.
				//특정 축에 대하여 점의차가 1이면 두 선분은  특정 축에 붙어있다.
				if (diff.ix == 0 && diff.iy <= 1  ) 
				{
					return true;
				}
				if (diff.ix <= 1 && diff.iy == 0  ) 
				{
					return true;
				}

				return false;
			}


		}

	


		public class Bundle
		{
			// ---> row
			public List<Piece> row = new List<Piece>();
			
			// ^ column
			public List<Piece> column = new List<Piece>();

			public List<Index2> totalIndex2 = new List<Index2>();

			public DropInfo.eKind kind;
			public int totalCount; //전체 드롭수
			public int totalReinforce; //전체 강화드롭수
			public int shape;//드롭모양

		}



		public class BundleLookup
		{

			public List<Bundle> bundleList = new List<Bundle>();

			private DropMap 	_refDropMap = null;
			private List<Piece> _rowPiece = new List<Piece>();
			private List<Piece> _columnPiece = new List<Piece>();


			public void SetDropMap(DropMap refDropMap)
			{
				_refDropMap = refDropMap;
			}

			public void Inspection(Piece.eDirection dir)
			{
				const int PUD_PIECE_MIN_LENGTH = 3;
				int roopCount = 0;
				if(Piece.eDirection.ROW == dir)
					roopCount = _refDropMap.GetMapHeight ();
				if(Piece.eDirection.COLUMN == dir)
					roopCount = _refDropMap.GetMapWidth ();


				Index2 start = Index2.Zero , end = Index2.Zero;
				List<int> pieceCountList = null;
				for (int i=0; i<roopCount; i++) 
				{
					//row
					if(Piece.eDirection.ROW == dir)
					{
						start.ix = 0;
						start.iy = i;
						end.ix = _refDropMap.GetMapWidth() - 1;
						end.iy = i;
					}

					//column
					if(Piece.eDirection.COLUMN == dir)
					{
						start.ix = i;
						start.iy = 0;
						end.ix = i;
						end.iy = _refDropMap.GetMapHeight() - 1;
					}

					if(true == _refDropMap.FindPiece(start, end, PUD_PIECE_MIN_LENGTH, out pieceCountList))
					{
						//todo
					}


				}//end for

			}//end Inspection

			public bool FindPiece(Index2 start, Index2 end, int minLength, out List<Piece> pieceList)
			{
				List<int> countList = null;
				pieceList = new List<Piece> ();

				Index2 dir = end - start;
				//0이 아니라면 길이를 1로 만든다.
				if(dir.ix != 0) dir.ix = dir.ix / dir.ix; 
				if(dir.iy != 0) dir.iy = dir.iy / dir.iy;

				if(true == _refDropMap.FindPiece(start, end, minLength, out countList))
				{
					Piece p = null;
					Index2 current = start;
					for(int i=0;i<countList.Count;i++)
					{
						p = new Piece();
						p.start = current;
						p.dir = dir;
						p.length = countList[i];
						p.end =  current + dir * (countList[i]-1);
						pieceList.Add(p);

						current = current + dir * countList[i];
					}


					return true;
				}

				return false;
			}

			public void Join()
			{
			
			}



		}


		public class FittingList
		{
			public List<Bundle> bundles = new List<Bundle> ();

			/*
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
			 //*/

		}
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
