
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;

namespace PuzzAndBidurgi
{
	namespace Fitting
	{

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

	}

}
