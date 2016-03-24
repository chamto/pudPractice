
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

	}
}
