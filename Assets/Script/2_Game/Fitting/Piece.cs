
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
		public class GroupList : List<Piece>
		{
		}

		
		public class PieceList : Dictionary<Index2, List<Piece>>
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

	}

}
