using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PuzzAndBidurgi
{

	//deprecate
//	public enum eDropKind : ushort
//	{
//		
//		//기본 드랍 5종
//		Red = 0,
//		Green,
//		Blue,
//		Light,
//		Dart,
//		
//		//회복드랍
//		Heart,
//		
//		//방해드롭
//		Obstruction,
//		Posion,
//		
//		
//		Max,
//	}


	namespace NDrop
	{
		public struct SDropInfo
		{
			public Vector2 pos;
			public eResKind eDropKind;
			public bool isVisible;
		}


		/// <summary>
		/// 각각의 드롭에 대한 이동순서 정보
		/// </summary>
		public class CMoveSequence
		{
			private float m_timeDelta;
		}

		/// <summary>
		/// 전체 드롭의 구역별 고유 위치값 (안보이는 드롭도 포함한다)
		/// </summary>
		public class CLocations
		{
			//--- Quadrangle shape
			private ushort m_columns;
			private ushort m_rows;

			//--- Circle shape			!!!!!!!Design forecast

			//--- AnyType shape			!!!!!!!Design forecast

			private ArrayList list = new ArrayList();
			
			public void Init(ushort columns , ushort rows)
			{
			}
		}
		
		public class CPath
		{
			private Dictionary<float,MonoDrop> 	m_dtnrMovedPath = new Dictionary<float,MonoDrop>();
			
			
			public void ClearMovedPath()
			{
				//chamto test
				CDefine.DebugLog("=-=-=-=-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-=");
				foreach(KeyValuePair<float,MonoDrop>  kv in m_dtnrMovedPath)
				{
					CDefine.DebugLog(kv.Key + "  " + kv.Value.name);
				}
				
				m_dtnrMovedPath.Clear();
			}
			public void AddMovedPath(MonoDrop drop)
			{
				if(null == drop) return;
				
				int prevAddedDropIdx = m_dtnrMovedPath.Count-1;
				if(0 <= prevAddedDropIdx)
				{
					//same object required
					if(drop == m_dtnrMovedPath.ElementAt(prevAddedDropIdx).Value) return;
				}
				
				
				CDefine.DebugLog("AddMovedPath : " + Time.fixedTime + " " + Time.time);
				m_dtnrMovedPath.Add(Time.time , drop);
			}
			public MonoDrop NextMovedPath()
			{
				if(0 == m_dtnrMovedPath.Count) return null;
				
				float timeKey = m_dtnrMovedPath.Keys.ElementAt(0);
				MonoDrop getDrop = m_dtnrMovedPath[timeKey];
				m_dtnrMovedPath.Remove(timeKey);
				
				return getDrop;
			}
		}//class CPath
	}//namespace NDrop


	public class DropMap : Dictionary<int,MonoDrop>
	{
		//단순히 인덱스를 조회하기 위한 인덱스맵을 조직한다
		private Dictionary<Index2,int> mapIndex2 = new Dictionary<Index2,int>();


		public MonoDrop GetMonoDropByUID(int keyUID) 
		{
			MonoDrop getValue = null;
			if(TryGetValue(keyUID,out getValue))
				return getValue;

			return null;
		}

		public MonoDrop GetMonoDropByIndex2(Index2 ixy)
		{
			int getUID = 0;
			if(mapIndex2.TryGetValue(ixy,out getUID))
				return this.GetMonoDropByUID(getUID);

			return null;
		}

		public MonoDrop GetMonoDropByIndex1(int ixy , int maxColumn)
		{
			return null;
		}

	
		//==============: override method:========================================================================================
		virtual public void Add(int key, MonoDrop value)
		{
			if (null == value) 
			{
				Debug.LogError("Add : null == value");
				return;
			}

			base.Add (key, value);
			mapIndex2.Add (value.dropInfo.index2D, key);

		}

		virtual public bool Remove(int key)
		{
			MonoDrop getValue = null;
			if (TryGetValue (key, out getValue)) 
			{
				mapIndex2.Remove(getValue.dropInfo.index2D);
			}

			return base.Remove (key);
		}

	}


	public  class DropBoard
	{
		public enum eStandard
		{
			eLeftBottom = 0,
			eLeftUp = 1,
			eCenter = 2,
		}


		private Vector2 m_squareLength;
		private Index2 m_boardSize;

		private Index2 		m_viewPosition;
		private eStandard 	m_eViewStandard;

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

	

		public DropBoard()
		{
			m_squareLength = new Vector2 (1.15f, 1.15f);
			m_boardSize = new Index2 (6, 5);

			m_viewPosition = new Index2 (0, 0);
			m_eViewStandard = eStandard.eLeftBottom;
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
			return new Index2 (m_viewPosition.ix, m_viewPosition.iy + boardSize.iy -1);
		}
		public  Index2 GetIndexAt_ViewRightUp()
		{
			return new Index2 (m_viewPosition.ix + boardSize.ix -1, m_viewPosition.iy + boardSize.iy -1);
		}
		public  Index2 GetIndexAt_ViewLeftBottom()
		{
			return new Index2 (m_viewPosition.ix, m_viewPosition.iy);
		}
		public  Index2 GetIndexAt_ViewRightBottom()
		{
			return new Index2 (m_viewPosition.ix + boardSize.ix -1, m_viewPosition.iy);
		}



		public Index2 GetPositionToIndex2D(Vector3 pos)
		{
			return DropBoard.GetPositionToIndex2D(pos, this.squareWidth, this.squareHeight);
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
			return DropBoard.GetIndex2DToPosition(ixy, this.squareWidth, this.squareHeight);
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
			size.x = this.squareWidth * m_boardSize.ix;
			size.y = this.squareHeight * m_boardSize.iy;
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
	}


	/// <summary>
	/// C drop manager.
	/// 드롭을 생성/제거/배치 하는 관리 객체
	/// </summary>
	public class CDropManager
	{
		
		//==============: member variables :==============
		/// <summary>
		/// The m_map find it the unique identity number interface.
		/// </summary>
		private DropMap 						m_mapDrop = new DropMap();
		private DropBoard						m_board = new DropBoard();

		//private Dictionary<int,MonoDrop> 		m_dtnrDrop = new Dictionary<int,MonoDrop>();
		//private Dictionary<float,MonoDrop> 	m_dtnrMovedPath = new Dictionary<float,MonoDrop>();
		private NDrop.CPath m_dropPath = new NDrop.CPath();


		//==============: property definition :========================================================================================
		public DropMap 						MapDrop
		{
			get
			{
				return m_mapDrop;
			}
		}
		public DropBoard					Board 
		{
			get 
			{
				return m_board;
			}
		}
		public Index2						BoardSize
		{
			get
			{
				return m_board.boardSize;
			}
		}




		//------------------------------------------------------------------------
		// currection method
		//------------------------------------------------------------------------




		public ML.LineSegment3 CorrectionLineSegment(MonoDrop srcDrop , ML.LineSegment3 lineSeg3)
		{
			if (null == srcDrop)
				return lineSeg3;

			//Correction value
			//PairInt parameter is array index(0 start , 1 is not ).
			//Index2 startPos = PairInt.Start_C5_R0;
			Vector3 putPos_left_up = m_board.GetPositionAt_ViewLeftUp () + Single.UIRoot.transform.position;
			Vector3 putPos_right_up = m_board.GetPositionAt_ViewRightUp () + Single.UIRoot.transform.position;
			Vector3 putPos_left_bottom = m_board.GetPositionAt_ViewLeftBottom () + Single.UIRoot.transform.position;
			Vector3 putPos_right_bottom = m_board.GetPositionAt_ViewRightBottom () + Single.UIRoot.transform.position;

			//Vector3 putPos_left_up = GetPositionOfPutDrop (new PairInt (0, 0));
			//Vector3 putPos_right_up = GetPositionOfPutDrop (new PairInt (0, (int)ConstBoard.Max_Row-1));
			//Vector3 putPos_left_bottom = GetPositionOfPutDrop (new PairInt ((int)ConstBoard.Max_Column-1, 0));
			//Vector3 putPos_right_bottom = GetPositionOfPutDrop (new PairInt ((int)ConstBoard.Max_Column-1, (int)ConstBoard.Max_Row-1));

#if UNITY_EDITOR
			//-------------------------------------------------------------------------
			//20140906 chamto test
			//-------------------------------------------------------------------------
			Single.MonoDebug.cube_LeftUp.transform.position = putPos_left_up;
			Single.MonoDebug.cube_RightUp.transform.position = putPos_right_up;
			Single.MonoDebug.cube_LeftBottom.transform.position = putPos_left_bottom;
			Single.MonoDebug.cube_RightBottom.transform.position = putPos_right_bottom;
			//-------------------------------------------------------------------------
#endif

			Bounds bob = m_board.GetBoundaryOfView (Single.UIRoot.transform.position);
			ML.LineSegment3 result = new ML.LineSegment3();
			result.origin = srcDrop.firstWorldPosition;
			result.last = lineSeg3.last;



			//CDefine.DebugLog ("Bounds " + bob + bob.min + bob.max);
			if (lineSeg3.last.y >= bob.max.y) 
			{	//------------- correction up -------------
				//CDefine.DebugLog("----correction up");
				result.last_y = putPos_left_up.y;
				if(lineSeg3.last.x <= bob.min.x)
				{	//correction left-up
					result.last = putPos_left_up;
					//CDefine.DebugLog("----correction left up" + putPos_left_up);
				}
				if(lineSeg3.last.x >= bob.max.x)
				{	//correction right-up
					result.last = putPos_right_up;
					//CDefine.DebugLog("----correction right up");
				}

			}else
				if (lineSeg3.last.y <= bob.min.y) 
			{	////------------- correction bottom -------------
				//CDefine.DebugLog("----correction bottom");
				result.last_y = putPos_left_bottom.y;
				if(lineSeg3.last.x <= bob.min.x)
				{	//correction left-bottom
					result.last = putPos_left_bottom;
					//CDefine.DebugLog("----correction left bottom"+putPos_left_bottom);
				}
				if(lineSeg3.last.x >= bob.max.x)
				{	//correction right-bottom
					result.last = putPos_right_bottom;
					//CDefine.DebugLog("----correction right bottom");
				}

			}else
				if(lineSeg3.last.x <= bob.min.x)
			{	////------------- correction left -------------
				result.last_x = putPos_left_up.x;
				//CDefine.DebugLog("----correction left");
			}else
				if(lineSeg3.last.x >= bob.max.x)
			{	////------------- correction right -------------
				result.last_x = putPos_right_up.x;
				//CDefine.DebugLog("----correction right");
			}else 
			{
				////------------- correction is not required
				//CDefine.DebugLog("----correction is not required");
				return lineSeg3;
			}

			//CDefine.DebugLog ("Correction LineSegement : " + result);
			return result;
		}


		public bool ValidSwapMonoDrop (MonoDrop drop1, MonoDrop drop2)
		{
			if (null == drop1 || null == drop2)
								return false;

			//피타고라스 정리  [가로*가로 + 세로*세로 = 빗변*빗변] 를 이용하여 직각삼각형의 빗변을 구함
			//구할려는 삼각형의 가로,세로길이가 같기 때문에 식을 다음과 같이 정리
			//빗변*빗변 = 가로*가로*2
			//실수값을 비교, 오차가 발생할것이기 떄문에 가중치값을 더함
			//빗변*빗변 = 가로*가로*2 + 가중치
			//float dist = ConstDrop.UI_Width * ConstDrop.UI_Width * 2 + 0.15f;
			float dist = (m_board.squareWidth * m_board.squareWidth + m_board.squareHeight * m_board.squareHeight) + 0.15f;

			return (dist > GetSqrDistance (drop1.firstWorldPosition, drop2.firstWorldPosition));
		}


		//------------------------------------------------------------------------
		// drop method
		//------------------------------------------------------------------------

		//public bool SwapMonoDropInBoard(int keyOfPos1 , int keyOfPos2 , bool applyPosition)
		public bool SwapMonoDropInBoard(Index2 ixy1 , Index2 ixy2 , bool applyPosition)
		{

			if (ixy1 == ixy2) 
				return false; 

			MonoDrop temp1 = m_mapDrop.GetMonoDropByIndex2 (ixy1);
			MonoDrop temp2 = m_mapDrop.GetMonoDropByIndex2 (ixy2);

			if (null == temp1 || null == temp2)
								return false;

			if (false == ValidSwapMonoDrop (temp1, temp2))
								return false;

			//swap index2 coord
			Index2 tempIndex = temp1.dropInfo.index2D;
			temp1.dropInfo.index2D = temp2.dropInfo.index2D;
			temp2.dropInfo.index2D = tempIndex;

			//2. swap key of monoDrop
			//temp1.keyOfPosition = keyOfPos2;
			//temp2.keyOfPosition = keyOfPos1;

			//3. swap localPosition of monoDrop
			SwapFirstLocalPosition (temp1, temp2);
			if (true == applyPosition) 
			{
				temp1.transform.localPosition = temp1.firstLocalPosition;
				temp2.transform.localPosition = temp2.firstLocalPosition;
			}



			return true;

		}

		public void SwapFirstLocalPosition(MonoDrop drop1 , MonoDrop drop2) 
		{
			Vector3 temp = drop1.firstLocalPosition;
			drop1.firstLocalPosition = drop2.firstLocalPosition;
			drop2.firstLocalPosition = temp;
		}


		//------------------------------------------------------------------------
		// path method
		//------------------------------------------------------------------------

		public void ClearMovedPath()
		{
			m_dropPath.ClearMovedPath();
			//chamto test
//			CDefine.DebugLog("=-=-=-=-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-==-=-=-=");
//			foreach(KeyValuePair<float,MonoDrop>  kv in m_dtnrMovedPath)
//			{
//				CDefine.DebugLog(kv.Key + "  " + kv.Value.name);
//			}
//
//			m_dtnrMovedPath.Clear();
		}
		public void AddMovedPath(MonoDrop drop)
		{
			m_dropPath.AddMovedPath(drop);
//			if(null == drop) return;
//
//			int prevAddedDropIdx = m_dtnrMovedPath.Count-1;
//			if(0 <= prevAddedDropIdx)
//			{
//				//same object required
//				if(drop == m_dtnrMovedPath.ElementAt(prevAddedDropIdx).Value) return;
//			}
//
//
//			CDefine.DebugLog("AddMovedPath : " + Time.fixedTime + " " + Time.time);
//			m_dtnrMovedPath.Add(Time.time , drop);
		}
		public MonoDrop NextMovedPath()
		{
			return m_dropPath.NextMovedPath();
//			if(0 == m_dtnrMovedPath.Count) return null;
//
//			float timeKey = m_dtnrMovedPath.Keys.ElementAt(0);
//			MonoDrop getDrop = m_dtnrMovedPath[timeKey];
//			m_dtnrMovedPath.Remove(timeKey);
//
//			return getDrop;
		}


		//------------------------------------------------------------------------
		// basic method
		//------------------------------------------------------------------------

		public void Init()
		{


			//print dropKind list
			//const float WIDTH_DROP = 1.15f;
			//const float HEIGHT_DROP = 1.15f;
			const uint MAX_DROP_COLUMN = 6;
			const uint MAX_DROP5X6 = 5 * 6;
			const uint MAX_DROP5X6X2 = 5 * 6 * 2;
			const byte MAX_DROPKIND = 6;
			eResKind[] dropKind = new  eResKind[MAX_DROPKIND];
			dropKind[0] = eResKind.Red;
			dropKind[1] = eResKind.Green;
			dropKind[2] = eResKind.Blue;
			dropKind[3] = eResKind.Light;
			dropKind[4] = eResKind.Dart;
			dropKind[5] = eResKind.Heart;

			System.Random rndDrop = new System.Random();

			Vector3 pos = Vector3.zero;
			MonoDrop pDrop = null;
			Index2 ixy;
			for(int i=0 ; i<MAX_DROP5X6X2 ; i++)
			{
				ixy.ix = (int)(i% MAX_DROP_COLUMN); 
				ixy.iy = (int)(i/MAX_DROP_COLUMN); 
				pos.x = ixy.ix * m_board.squareWidth;
				pos.y = ixy.iy * m_board.squareHeight;
				pDrop = MonoDrop.Create(Single.UIRoot.transform, 
				                        dropKind[rndDrop.Next(0,MAX_DROPKIND)],
				                        pos);
				pDrop.dropInfo.index2D = ixy;
				m_mapDrop.Add(pDrop.dropInfo.id, pDrop);

				//6x5 : width6 height5
				//pDrop = CreateDrop(i, Single.UIRoot.transform, dropKind[rndDrop.Next(0,MAX_DROPKIND)] , 
				//                   (i% MAX_DROP_COLUMN) * WIDTH_DROP ,(i/MAX_DROP_COLUMN) * -HEIGHT_DROP);

				//m_dtnrDrop.Add(i,pDrop);

				//------ setting drop of color that invisialbe 30~  ------------
				if(null != pDrop && i >= MAX_DROP5X6)
				{
					pDrop.SetColor(Color.blue);
					pDrop.GetBoxCollider2D().enabled = false; //20150212 chamto - 터치입력을 못받게 충돌체를 비활성 시켜 놓는다.
				}

			}

			//chamto test
			//Vector3 v3Pos = CMain.UIRoot.transform.position;
			//v3Pos.x = -2.9f;
			//CMain.UIRoot.transform.position = v3Pos;
		
		}

		public void Update()
		{
		}


		
		/// <summary>
		/// Get Position of put Drop of board
		/// </summary>
		/// <returns> center position of put drop.</returns>
		/// <param name="dropPair">Drop pair.</param>
//		public Vector3 GetPositionOfPutDrop(Index2 dropPair)
//		{
//			Vector3 posOfPut;
//			posOfPut.x = Single.UIRoot.transform.position.x + m_board.squareWidth * dropPair.ix;
//			posOfPut.y = Single.UIRoot.transform.position.y + m_board.squareHeight * dropPair.iy;
//			posOfPut.z = Single.UIRoot.transform.position.z;
//			
//			return posOfPut;
//		}







		//------------------------------------------------------------------------
		// collision method
		//------------------------------------------------------------------------

		/// <summary>
		/// 기준점에서 목표점까지의 벡터제곱길이를 반환한다.
		///  - 계산속도 때문에 제곱근을 구하지 않고, 제곱한값을 반환한다.
		/// </summary>
		/// <returns>The sqr distance.</returns>
		/// <param name="standardDrop">Standard drop.</param>
		/// <param name="toDstDrop">To dst drop.</param>
		//private float GetSqrDistance(MonoDrop standardDrop , MonoDrop toDstDrop)
		private float GetSqrDistance(Vector3 standardPos , Vector3 toDstPos)
		{
			//note!! before moveing , dstDrop is firstPosition 
			return (toDstPos - standardPos).sqrMagnitude;
		}

		/// <summary>
		/// Gets the shortest distance.
		/// </summary>
		/// <returns>The shortest distance.</returns>
		/// <param name="standardDrop">Standard drop.</param>
		/// <param name="minDistance">최소거리의 최소값을 지정. 최소값이 5라면, 최소거리는 적어도 5보다 같거나 커야한다.</param>
		public MonoDrop GetShortestDistance(MonoDrop standardDrop, float minDistance)
		{
			if(null == standardDrop || 0 >= minDistance) return null;
			if(0 == m_mapDrop.Count) return null;

			List<MonoDrop> list = m_mapDrop.Values.ToList();
			//list.Remove(standardDrop); //deduplicate

			//list.Sort(SortDistanceCompareTo);

			//기준점으로 부터 제곱길이가 가장 작은순으로 정렬한 드롭목록을 얻는다.
			list = (from dstDrop in list
			        orderby GetSqrDistance(standardDrop.firstWorldPosition,dstDrop.firstWorldPosition) ascending
					select dstDrop).ToList();

			//foreach(MonoDrop drop in list)			
			//	CDefine.DebugLog(drop + "  " + Math.Sqrt(GetSqrDistance(standardDrop,drop)));
			//CDefine.DebugLog(GetSqrDistance(standardDrop,list[0]) + "  " + (minDistance*minDistance)); //chamto test
			if(GetSqrDistance(standardDrop.firstWorldPosition,list[0].firstWorldPosition) <= (minDistance * minDistance)) return null;
			//if(standardDrop == list[0]) return null;

			return list[0];
		}



		//new multi CCD collision functuon - 20140619 chamto
		public List<MonoDrop> CalcCCDCollision(MonoDrop srcDrop, float nonCollision_minDistance)
		{
			if (null == srcDrop) return null;

			//1.터치드래그한 선분을 구한다.
			ML.LineSegment3 ls3 = new ML.LineSegment3 ();
			//ls3.origin = srcDrop.transform.position;
			ls3.origin = srcDrop.firstWorldPosition;
			ls3.last = Input_Unity.GetTouchWorldPos ();


			//1.1.드롭판을 벗어난 터치선을 보정한다.
			ls3 = this.CorrectionLineSegment (srcDrop, ls3);

			//1.2.터치선이 2차원 상에 있게 한다.
			ls3.origin.z = 0;
			ls3.direction.z = 0;


#if UNITY_EDITOR
			//-------------------------------------------------------------------------
			//20140906 chamto - test
			//-------------------------------------------------------------------------
			Single.MonoDebug.lineRender.SetWidth (0.1f, 0.4f);
			Single.MonoDebug.lineRender.SetPosition (0, ls3.origin);
			Single.MonoDebug.lineRender.SetPosition (1, ls3.direction + ls3.origin);
			//-------------------------------------------------------------------------
#endif

			//2. 3. 모든드롭에 대해 전수 조사한다. (선분 근처 드롭만 조사하게 최적화 필요)
			//SortedDictionary<float , MonoDrop> collisionDtnr = new Dictionary<float , MonoDrop> ();
			List<MonoDrop> collisionList = new List<MonoDrop> ();
			float t_c = 0.0f;
			foreach (MonoDrop dstDrop in m_mapDrop.Values) 
			{
				//self exclusion
				if(srcDrop == dstDrop) continue;

				if((nonCollision_minDistance * nonCollision_minDistance) > ls3.MinimumDistanceSquared(dstDrop.firstWorldPosition,out t_c))
				{
					//20140907 chamto test
//					float dist = ls3.MinimumDistanceSquared(dstDrop.firstWorldPosition,out t_c);
//					{
//						CDefine.DebugLog("object : "+dstDrop  +"  mSqrtDist : "+dist + " noncSqrtDist : " + nonCollision_minDistance * nonCollision_minDistance + "  firstwp : "+dstDrop.firstWorldPosition + " t_c : " + t_c);
//					}

					//Input collision list
					collisionList.Add(dstDrop);
				}
			}

			//4.선분과 충돌한 드롭목록을 “선분의 시작점에서 충돌드롭의 중점 까지의 거리”를 기준으로 오름차순 정렬한다. 
//			collisionDtnr = (from pairObj in collisionDtnr
//							orderby pairObj.Key ascending
//			                 select pairObj).ToDictionary (v1 => v1.Key,v1 => v1.Value);



			List<MonoDrop> result = (from dstDrop in collisionList
			                         orderby GetSqrDistance(srcDrop.firstWorldPosition,dstDrop.firstWorldPosition) ascending
			                         select dstDrop).ToList();


//			CDefine.DebugLog ("----- line -----  ori :" + ls3.origin + "  last : " + ls3.last + " dict : " + ls3.direction); //chamto test


			return result;
		}


		//chamto noUsed function !!
		//chamto need Optimization !!
		public MonoDrop CalcCollision(MonoDrop srcDrop)
		{


			const float BOX_WIDTH = 0.57f;
			const float BOX_HEIGHT = 0.57f;
			if(null == srcDrop) return null;

			Rect srcBox = new Rect();
			Rect dstBox = new Rect();
			srcBox.width = BOX_WIDTH;
			srcBox.height = BOX_HEIGHT;
			srcBox.center = new Vector2(srcDrop.transform.position.x,srcDrop.transform.position.y);
			dstBox = srcBox;
			foreach(MonoDrop dstDrop in m_mapDrop.Values)
			{
				//self exclusion
				if(srcDrop == dstDrop) continue;

				//dstBox.center = new Vector2(dstDrop.transform.position.x,dstDrop.transform.position.y);
				dstBox.center = new Vector2(dstDrop.firstWorldPosition.x , dstDrop.firstWorldPosition.y);

				if(true == srcBox.Overlaps(dstBox,true)) //include allowInverse
				{
					return dstDrop;
				}

			}

			return null;
		}

		




	}//end class
}//end namespace




