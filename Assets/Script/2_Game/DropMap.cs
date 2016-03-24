using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PuzzAndBidurgi
{


//	public enum eMapKind
//	{
//		eGRID_MAP = 0,
//		eHEX_MAP = 1,
//	}
//	
//	public struct Hex3
//	{}

	//"model" - controller - view
	public class DropMap
	{

		private UInt16 _sequenceId = 0;

		private System.Random 		_random = new System.Random();

		//격자형 지도 인덱스
		private Dictionary<Index2,DropInfo> _map = new Dictionary<Index2,DropInfo>();

		//육각형 지도 인덱스
		//private Dictionary<Hex3,Int32> 	_hexMap = new Dictionary<Hex3,Int32>();

		private UInt16 _width = 0;
		private UInt16 _height = 0;

		public UInt16 GetMapWidth() 
		{
			return _width;
		}
		public UInt16 GetMapHeight() 
		{
			return _height;
		}

		//20160221 chamto - fix me : 지정한 드롭목록에서 드롭별 확률에 맞게 반환되게 수정되어야 한다.
		public DropInfo.eKind GetRandDrop(byte max_kind)
		{
			
			return (DropInfo.eKind)_random.Next (1, max_kind);
		}

		public void CreateDropMap(UInt16 width, UInt16 height, Index2 startPos)
		{
			UInt16 MAP_SIZE = (UInt16)(width * height);

			_width = width;
			_height = height;

			DropInfo.eKind defaultKind = DropInfo.eKind.Heart;
			Index2 ixy = startPos;
			DropInfo dropInfo = null;
			for(UInt16 i=0 ; i < MAP_SIZE ; i++)
			{
				ixy.ix = (int)(i% width); 
				ixy.iy = (int)(i/ height); 

				dropInfo = this.createDropInfo(defaultKind);
				this.AddValue(ixy, dropInfo);

			}
		
		}

		private DropInfo createDropInfo(DropInfo.eKind ekind)
		{
			DropInfo drop = new DropInfo ();
			drop.id = _sequenceId++;
			drop.kind = ekind;

			return drop;
		}

		private Fitting.Piece createPiece(DropInfo.eKind kind, Index2 start, Index2 dir, int reinforceCount)
		{
			Fitting.Piece p = new Fitting.Piece();
			p.kind = kind;
			p.dir = dir;
			p.groupList = null;
			p.start = start;
			p.end = start + dir;
			p.length = 1;
			p.reinforceCount = reinforceCount; 

			return p;
		}

		public struct Area
		{
			public Index2 origin;
			public int width;
			public int height;
			private Index2 temp;

			public Index2 LeftBottom
			{
				get
				{
					return origin;
				}

			}
			public Index2 LeftUp
			{
				get
				{
					temp = origin;
					temp.iy += (height - 1);
					return temp;	
				}

			}
			public Index2 RightBottom
			{
				get
				{
					temp = origin;
					temp.ix += (width - 1);
					return temp;	
				}

			}
			public Index2 RightUp
			{
				get
				{
					temp.ix = (width - 1);
					temp.iy = (height - 1);
					return origin + temp;	
				}

			}

			public bool IsInclude(Index2 pos)
			{
				if (this.LeftBottom.ix <= pos.ix && pos.ix <= this.RightBottom.ix) 
				{
					if(this.LeftBottom.iy <= pos.iy && pos.iy <= this.LeftUp.iy)
						return true;
				}

				return false;
			}
		}

		public void FindPiece (Index2 start, Index2 dir, Area area, out List<Fitting.Piece> pieceList)
		{
			Index2 current = start;

//			Index2 dir = end - start;
//			//0이 아니라면 길이를 1로 만든다.
//			if(dir.ix != 0) dir.ix = dir.ix / dir.ix; 
//			if(dir.iy != 0) dir.iy = dir.iy / dir.iy;

			//int count = 1;
			//DropInfo.eKind prevKind = _map[current].kind;
			DropInfo.eKind prevKind = DropInfo.eKind.None;
			pieceList = new List<Fitting.Piece> ();
			Fitting.Piece p = null;
			//while ((end-start).LengthSquared() >= (current-start).LengthSquared()) 
			while(area.IsInclude(current))
			{

				if( DropInfo.eKind.None != _map[current].kind )
				if(prevKind != _map[current].kind )
				{
					p = this.createPiece (_map[current].kind, current, dir, _map[current].reinforcement);
					pieceList.Add (p);

				}else
				{
					p.length++;
					p.end = p.start + dir * p.length;
					p.reinforceCount += _map[current].reinforcement;

				}

				current = current + dir; //next position
				prevKind = _map[current].kind;

			}//end while

		}

		public Fitting.PieceList FindPieceList(Index2 findingDir, Area area)
		{ 
			//todo

			//const int PUD_PIECE_MIN_LENGTH = 3;

			//조각을 찾는 방향에 따른 루프방향 구하기 (찾는 방향과 루프방향은 서로 직각이다)
			Index2 nextDir;
			Index2 loopDir;
			Index2 loopStart;
			int loopLength;
			if (findingDir == Index2.Right) 
			{
				loopDir = Index2.Up;
				nextDir = loopDir;
				loopStart = area.LeftBottom;
				loopLength = area.height;
			} else 
			if (findingDir == Index2.Up) 
			{
				loopDir = Index2.Right;
				nextDir = loopDir;
				loopStart = area.LeftBottom;
				loopLength = area.width;
			} else
			if (findingDir == Index2.RightUp) 
			{
				loopDir = Index2.LeftUp;
				nextDir = loopDir.X_AxisSeparation;
				loopStart = area.RightBottom;
				loopLength = area.width + area.height;
			} else
			if (findingDir == Index2.LeftUp) 
			{
				loopDir = Index2.RightUp;
				nextDir = loopDir.X_AxisSeparation;
				loopStart = area.LeftBottom;
				loopLength = area.width + area.height;
			} else 
			{
				return null;
			}

			Fitting.PieceList fullList = new Fitting.PieceList ();
			List<Fitting.Piece> someList = null; 

			Index2 nextPos = loopStart;
			for (int i=0; i<loopLength; i++) 
			{

				this.FindPiece(nextPos, findingDir, area, out someList);
				if(0 != someList.Count)
				{
					fullList.Add(nextPos,someList);
				}

				//다음위치가 조각 구하는 영역을 벗어났을 경우 : 증가 방향을 바꾸어 준다. 
				//이는 조각찾는 방향이 우상,좌상 일때만 해당한다.  findingDir : rightup , leftup
				if (findingDir == Index2.RightUp || findingDir == Index2.LeftUp)
				if(false == area.IsInclude(nextPos + nextDir))
				{
					nextDir = loopDir.Y_AxisSeparation;
				}

				nextPos += nextDir;

			}//end for

			return null;
		}

		public bool AddValue(Index3 key_index , DropInfo value)
		{
			DropInfo getValue = null;
			if (true == _map.TryGetValue (key_index, out getValue)) 
			{
				return false;
			}
			
			_map.Add(key_index, value);
			return true;
		}

		public bool SetValue(Index3 key_index , DropInfo value)
		{
			
			DropInfo getValue = null;
			if (false == _map.TryGetValue (key_index, out getValue)) 
			{
				return false;
			}
			
			_map[key_index] = value;
			return true;
		}

		public bool TryGetValue(Index2 key_index , out DropInfo getValue)
		{
			if (true == _map.TryGetValue (key_index, out getValue)) 
			{
				if(null != _map[key_index])
				{
					getValue =  _map[key_index];
					return true;
				}
				
			}

			getValue = null;
			return false;
		}

		public void Init()
		{
			//_map.Select (
			//	drop => {if(null != drop.Value) drop.Value.Init();}
			//);

			foreach (DropInfo value in _map.Values) 
			{
				if(null != value) value.Init();
			}
		}

		public void Clear(Index2 key_index)
		{
			DropInfo getValue = null;
			if(true == this.TryGetValue(key_index , out getValue))
			{
				this.SetValue (key_index, null);
			}

		}

		public void ClearAll()
		{
			foreach (Index2 key_index in _map.Keys) 
			{
				this.SetValue (key_index, null);
			}
		}
		
		//==============: debug method:========================================================================================
		
		public void Debug_PrintMap()
		{
			CDefine.DebugLog ("_map--------------------------------------------- : " + _map.Count);
			for (int i=0; i<_map.Count; i++) 
			{
				CDefine.DebugLog (_map.Keys.ToList()[i].ToString() + "  UID: " + _map.Values.ToList()[i]);
			}
			
		}

	}
	
}//end namespace
