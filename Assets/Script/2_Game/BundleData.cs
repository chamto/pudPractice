
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

		public class GroupList : List<Piece>
		{

		}


		public class Piece
		{

			//드롭의 종류
			public DropInfo.eKind kind;

			//강화드롭수
			public int reinforceCount; 

			//활성조건을 만족하는 드롭의 시작, 끝 위치
			public Index2 start; 
			public Index2 end;
			public Index2 dir;
			public int length;

			//public GroupList groupList;
			public Bundle groupList;
			

			static public List<Index2> ToList(Piece p)
			{
				List<Index2> list = new List<Index2> ();
				Index2 current;
				for (int i=0; i<p.length; i++) 
				{
					current = p.start + p.dir * i;
					list.Add(current);
				}
				
				return list;
			}

			static public ML.LineSegment3 ToLine(Piece p)
			{
				ML.LineSegment3 a;
				a.origin = new ML.Vector3 (p.start.ix, p.start.iy, 0);
				a.direction = new ML.Vector3 (p.end.ix - p.start.ix, p.end.iy - p.start.iy, 0);
				a.last = new ML.Vector3 (p.end.ix, p.end.iy, 0);

				return a;
			}


			public bool IsJoin(Piece p, int minLength)
			{
				if (p.length < minLength || this.length < minLength)
					return false;
				if (this.kind != p.kind) 
					return false;


				ML.Vector3 pt1, pt2;
				Index2 idx1, idx2, diff;

				ML.LineSegment3.ClosestPoints(out pt1, out pt2, Piece.ToLine (this), Piece.ToLine (p));

				idx1 = Index2.Vector3ToIndex2 (pt1, 1, 1);
				idx2 = Index2.Vector3ToIndex2 (pt2, 1, 1);

				diff = idx1 - idx2;
				diff.ix = Math.Abs (diff.ix);
				diff.iy = Math.Abs (diff.iy);

				//두 드롭조각이 겹쳐있거나 붙어있는 경우를 찾는다.
				//특정 축에 대하여 점의 차가 0이면 두 선분은  특정 축에 겹쳐있다.
				//특정 축에 대하여 점의 차가 1이면 두 선분은  특정 축에 붙어있다.
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


			public void Join(Piece dest, int minJoinLength)
			{
				if(true == this.IsJoin(dest , minJoinLength))
				{

					if(null == dest.groupList && null == this.groupList)
					{	//this 에 새그룹 생성
						
						this.groupList = new Bundle();
						dest.groupList = this.groupList;
						this.groupList.Add(this);
						this.groupList.Add(dest);
					}
					else if(dest.groupList == this.groupList)
					{	//이미 같은 그룹이다.
						return;
					}
					else if(null != this.groupList && null != dest.groupList)
					{	//this 그룹으로 합치기
						
						//this.groupList.AddRange(dest.groupList);
						this.groupList.UnionWith(dest.groupList);
						dest.groupList = this.groupList;
					}
					else if(null != this.groupList)
					{	//this 그룹에 추가하기
						
						this.groupList.Add(dest);
						dest.groupList = this.groupList;
						
					}else if(null != dest.groupList)
					{	//dest 그룹에 추가하기
						
						dest.groupList.Add(this);
						this.groupList = dest.groupList;
					}
					
				}//end if
			}


		}

		public class PieceList : Dictionary<Index2, List<Piece>>
		{
		}
	

		//같은 종류의 터질 수 있는 드롭 뭉치
		public class Bundle : HashSet<Piece>
		{
			public enum eShapeKind
			{
				NONE = 0,
				FIVE_CROSS = 1,
			}


			public HashSet<Index2> totalIndex2 = null;

			public DropInfo.eKind kind;
			public int totalCount; //전체 드롭수
			public int totalReinforce; //전체 강화드롭수
			public eShapeKind eShape;//드롭모양


			public void UpdateTotalIndex()
			{
				totalIndex2 = new HashSet<Index2>();
				foreach (Piece p in this) 
				{
					foreach(Index2 id2 in Piece.ToList(p))
					{
						totalIndex2.Add(id2);
					}
				}

				totalCount = totalIndex2.Count;
				eShape = this.findShape ();
				kind = this.ElementAt (0).kind;

				totalReinforce = -1; //강화드롭의 전체개수는 DropMap 을 직접조회해서 구해야 한다.
			}

			public int GetMaxLength(Index2 dir , out Piece getPiece)
			{
				//todo
				getPiece = null;
				return 0;
			}

			public int GetMaxLength_Including_Reinforce(Index2 dir , out Piece getPiece)
			{
				//todo
				getPiece = null;
				return 0;
			}

			private eShapeKind findShape()
			{
				eShapeKind kind = eShapeKind.NONE;

				if (true == findShape_FiveCross ()) 
				{
					kind = eShapeKind.FIVE_CROSS;
				}

				return kind;
			}
			private bool findShape_FiveCross()
			{

				//five_cross
				const int FIVE_CROSS_COUNT = 5;
				const int CROSS_WIDTH_LENGTH = 3;
				if (FIVE_CROSS_COUNT == this.totalCount) 
				{
					Index2 cross_center_row = Index2.None;
					Index2 cross_center_column = Index2.None;
					foreach(Piece p in this)
					{
						if(CROSS_WIDTH_LENGTH == p.length)
						{
							if(Index2.Right == p.dir)
							{
								cross_center_row = p.start + p.dir * 2;
							}
							if(Index2.Up == p.dir)
							{
								cross_center_column = p.start + p.dir * 2;
							}
						}
					}
					if(Index2.None != cross_center_row && 
					   cross_center_row == cross_center_column)
					{
						return true;
					}
				}

				return false;
			}

		}



		public class BundleLookup
		{

			public HashSet<Bundle> bundleSet = new HashSet<Bundle>();

			private PieceList _rowPieceList = null;
			private PieceList _columnPieceList = null;


			public void SetRowPieceList(PieceList rowList)
			{
				_rowPieceList = rowList;
			}
			public void SetColumnPieceList(PieceList columnList)
			{
				_columnPieceList = columnList;
			}


			private void joinPiece(PieceList pieceList_1, PieceList pieceList_2, int minJoinLength)
			{
				foreach (List<Piece> srcList in pieceList_1.Values) 
				{
					foreach (List<Piece> destList in pieceList_2.Values) 
					{
						foreach(Piece srcP in srcList)
						{
							foreach(Piece destP in destList)
							{
								srcP.Join(destP, minJoinLength);
								
							}//end foreach1
							
						}//end foreach2
						
					}//end foreach3
				}//end foreach4
			}

			private void joinPiece(PieceList pieceList, int minJoinLength)
			{
				List<Piece> prevList = null;
				foreach (List<Piece> curList in pieceList.Values) 
				{
					if(null != prevList || 0 != prevList.Count)
					{
						foreach(Piece curP in curList)
						{
							foreach(Piece prevP in prevList)
							{
								curP.Join(prevP, minJoinLength);
								
							}//end foreach1

						}//end foreach2

					}//end if3

					prevList = curList;
				}//end foreach4

			}//end func


			public void CreatePieceGroup()
			{
				const int MIN_PIECE_LENGTH = 3;
				this.joinPiece (this._rowPieceList, MIN_PIECE_LENGTH);
				this.joinPiece (this._columnPieceList, MIN_PIECE_LENGTH);
				this.joinPiece (this._rowPieceList, this._columnPieceList, MIN_PIECE_LENGTH);

			}

			public void CreateBundle()
			{
				foreach (List<Piece> pList in _rowPieceList.Values) 
				{
					foreach(Piece p in pList)
					{
						bundleSet.Add(p.groupList);
					}
				}

				foreach (List<Piece> pList in _columnPieceList.Values) 
				{
					foreach(Piece p in pList)
					{
						bundleSet.Add(p.groupList);
					}
				}
			
			}



		}


		public class FittingList
		{
			public List<Bundle> bundles = new List<Bundle> ();



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
