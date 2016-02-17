using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PuzzAndBidurgi
{

	public  class BoardInfo
	{
		public enum eStandard
		{
			eLeftBottom = 0,
			eLeftUp = 1,
			eCenter = 2,
		}

		private Vector2 	m_squareLength;
		private Index2 		m_boardSize;

		private Index2 		m_viewSize;
		private Index2 		m_viewPosition;
		//private eStandard 	m_eViewStandard;

		public float squareWidth
		{
			get
			{
				return m_squareLength.x;
			}
		}
		public float squareHeight
		{
			get
			{
				return m_squareLength.y;
			}
		}
		public Index2 boardSize
		{
			get
			{
				return m_boardSize;
			}
		}
		public Index2 viewSize
		{
			get
			{
				return m_viewSize;
			}
		}

		public BoardInfo()
		{
			m_squareLength = new Vector2 (1.15f, 1.15f);
			m_boardSize = new Index2 (6, 10 + 5);
			m_viewSize = new Index2 (6, 5);

			m_viewPosition = new Index2 (0, 0);
			//m_eViewStandard = eStandard.eLeftBottom;
		}

		//min, max nonviewArea
		public Index2 GetMinNonviewArea()
		{
			Index2 value = this.GetMaxViewArea ();
			value.iy += 1;
			value.ix = 0;
			return  value;
		}
		public Index2 GetMaxNonviewArea()
		{
			return this.GetMaxBoardArea ();
		}

		//min, max viewArea
		public Index2 GetMinViewArea()
		{
			return this.GetIndexAt_ViewLeftBottom ();
		}
		public Index2 GetMaxViewArea()
		{
			return this.GetIndexAt_ViewRightUp ();
		}

		//min, max boardArea
		public Index2 GetMinBoardArea()
		{
			return Index2.Zero;
		}
		public Index2 GetMaxBoardArea()
		{
			return new Index2 (m_boardSize.ix-1, m_boardSize.iy-1);
		}

		public  Vector3 GetPositionAt_ViewLeftUp()
		{
			return this.GetIndex2DToPosition (this.GetIndexAt_ViewLeftUp ());
		}
		public  Vector3 GetPositionAt_ViewRightUp()
		{
			return this.GetIndex2DToPosition (this.GetIndexAt_ViewRightUp ());
		}
		public  Vector3 GetPositionAt_ViewLeftBottom()
		{
			return this.GetIndex2DToPosition (this.GetIndexAt_ViewLeftBottom ());
		}
		public  Vector3 GetPositionAt_ViewRightBottom()
		{
			return this.GetIndex2DToPosition (this.GetIndexAt_ViewRightBottom ());
		}

		public  Index2 GetIndexAt_ViewLeftUp()
		{
			return new Index2 (m_viewPosition.ix, m_viewPosition.iy + m_viewSize.iy -1);
		}
		public  Index2 GetIndexAt_ViewRightUp()
		{
			return new Index2 (m_viewPosition.ix + m_viewSize.ix -1, m_viewPosition.iy + m_viewSize.iy -1);
		}
		public  Index2 GetIndexAt_ViewLeftBottom()
		{
			return new Index2 (m_viewPosition.ix, m_viewPosition.iy);
		}
		public  Index2 GetIndexAt_ViewRightBottom()
		{
			return new Index2 (m_viewPosition.ix + m_viewSize.ix -1, m_viewPosition.iy);
		}

		public Index2 GetPositionToIndex2D(Vector3 pos)
		{
			return BoardInfo.GetPositionToIndex2D(pos, this.squareWidth, this.squareHeight);
		}

		static public Index2 GetPositionToIndex2D(Vector3 pos , float in_squareWidth , float in_squareHeight)
		{
			Index2 ixy;
			ixy.ix = (int)((pos.x + (in_squareWidth * 0.5f)) / in_squareWidth);
			ixy.iy = (int)((pos.y + (in_squareHeight * 0.5f)) / in_squareHeight);
			
			return ixy;
		}

		//return localPosition  
		public Vector3 GetIndex2DToPosition(Index2 ixy)
		{
			return BoardInfo.GetIndex2DToPosition(ixy, this.squareWidth, this.squareHeight);
		}

		static public Vector3 GetIndex2DToPosition(Index2 ixy , float in_squareWidth , float in_squareHeight)
		{
			Vector3 posOfPut;
			posOfPut.x = in_squareWidth * ixy.ix;
			posOfPut.y = in_squareHeight * ixy.iy;
			posOfPut.z = 0;
			
			return posOfPut;
		}

		public Bounds GetBoundaryOfView(Vector3 UIRootPos)
		{
			Vector3 center, size;
			size.x = this.squareWidth * m_viewSize.ix;
			size.y = this.squareHeight * m_viewSize.iy;
			//size.x = ConstDrop.UI_Width * ConstBoard.Max_Row;
			//size.y = ConstDrop.UI_Height * ConstBoard.Max_Column;
			size.z = 0;
			
			center.x = UIRootPos.x + (size.x * 0.5f) - (this.squareWidth * 0.5f) + (this.squareWidth * m_viewPosition.ix);
			center.y = UIRootPos.y + (size.y * 0.5f) - (this.squareHeight * 0.5f) + (this.squareHeight * m_viewPosition.iy);
			//center.x = Single.UIRoot.transform.position.x + (size.x * 0.5f) - (ConstDrop.UI_Width * 0.5f);
			//center.y = Single.UIRoot.transform.position.y - (size.y * 0.5f) + (ConstDrop.UI_Height * 0.5f);// - size.y;
			center.z = 0;

#if UNITY_EDITOR
			//-------------------------------------------------------------------------
			//20140906 chamto test - debugCode
			//-------------------------------------------------------------------------
			Single.MonoDebug.boundary.transform.position = center;
			Single.MonoDebug.boundary.transform.localScale = size;
			//-------------------------------------------------------------------------
#endif
			
			return new Bounds (center, size);
		}

		public bool BelongToArea(Index2 min, Index2 max, Index2 ixy)
		{
			if ((min.ix <= ixy.ix && ixy.ix <= max.ix) && (min.iy <= ixy.iy && ixy.iy <= max.iy))
				return true;
			
			return false;
		}

		public bool BelongToViewArea(Index2 ixy)
		{
			Index2 min = this.GetIndexAt_ViewLeftBottom ();
			Index2 max = this.GetIndexAt_ViewRightUp ();
			return BelongToArea (min, max, ixy);
		}

	}


}//end namespace
