﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PuzzAndBidurgi
{

	using T_DropsInLine = System.Collections.Generic.List<MonoDrop> ;
	using T_JoinsInLine = System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>> ;
	using T_Bundle = System.Collections.Generic.Dictionary<PairIndex2, System.Collections.Generic.List<System.Collections.Generic.List<MonoDrop>>> ;


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
		private MapDrop 						m_mapDrop = new MapDrop();
		private BoardInfo						m_boardInfo = new BoardInfo();
		private GroupDrop						m_groupDrop = new GroupDrop ();

		System.Random 		m_random = new System.Random();


		//==============: property definition :========================================================================================
		public MapDrop 						mapDrop
		{
			get
			{
				return m_mapDrop;
			}
		}
		public BoardInfo					boardInfo 
		{
			get 
			{
				return m_boardInfo;
			}
		}
		public GroupDrop					groupDrop 
		{
			get 
			{
				return m_groupDrop;
			}
		}
		public Index2						VIEW_SIZE
		{
			get
			{
				return m_boardInfo.viewSize;
			}
		}




	
		//------------------------------------------------------------------------
		// drop method
		//------------------------------------------------------------------------


		public bool ValidSwapMonoDrop (MonoDrop drop1, MonoDrop drop2)
		{
			if (null == drop1 || null == drop2)
				return false;
			
			//피타고라스 정리  [가로*가로 + 세로*세로 = 빗변*빗변] 를 이용하여 직각삼각형의 빗변을 구함
			//구할려는 삼각형의 가로,세로길이가 같기 때문에 식을 다음과 같이 정리
			//빗변*빗변 = 가로*가로*2
			//실수값을 비교, 오차가 발생할것이기 떄문에 가중치값을 더함
			//빗변*빗변 = 가로*가로*2 + 가중치
			//float dist = m_board.squareWidth * m_board.squareWidth * 2 + 0.15f;
			float dist = (m_boardInfo.squareWidth * m_boardInfo.squareWidth + m_boardInfo.squareHeight * m_boardInfo.squareHeight) + 0.15f;
			
			return (dist > GetSqrDistance (drop1.gotoWorldPosition, drop2.gotoWorldPosition));
		}
		

		//public bool SwapMonoDropInBoard(int keyOfPos1 , int keyOfPos2 , bool applyPosition)
		public bool SwapMonoDropInBoard(int id1 , int id2 , bool applyPosition)
		{

			if (id1 == id2) 
				return false; 

			MonoDrop temp1 = m_mapDrop.GetMonoDropByUID (id1);
			MonoDrop temp2 = m_mapDrop.GetMonoDropByUID (id2);

			if (null == temp1 || null == temp2)
								return false;

			CDefine.DebugLog ("----------------SwapMonoDropInBoard 1: " + temp1.index2D.ToString() + "  " + temp2.index2D.ToString()); //20150331 chamto test

			if (false == ValidSwapMonoDrop (temp1, temp2))
								return false;


			//CDefine.DebugLog ("----------------SwapMonoDropInBoard 2: " + ixy1.ToString() + "  " + ixy2.ToString()); //20150331 chamto test

			//swap the index2 coord
			//Index2 tempIndex = temp1.index2D;
			//temp1.SetIndex (temp2.index2D);
			//temp2.SetIndex (tempIndex);
			temp1.SwapIndex (temp2.index2D);



			//3. swap localPosition of monoDrop
			temp1.SwapGotoLocalPosition (temp2);

			if (true == applyPosition) 
			{
				temp1.ApplyGotoLocalPosition();
				temp2.ApplyGotoLocalPosition();
			}


			//chamto test
			temp1.UpdateTextMesh ();
			temp2.UpdateTextMesh ();

			return true;
		}


		//------------------------------------------------------------------------
		// basic method
		//------------------------------------------------------------------------

		public MonoDrop AddDropInBoard(Index2 placePos , eResKind eDropKind)
		{
			return null;
		}

		public void RemoveDrop(Index2 placedPos)
		{
			//m_mapDrop.Remove(
		}


		public eResKind GetRandDrop(byte max_dropKind)
		{

			return (eResKind)m_random.Next (1, max_dropKind);
		}
		public void Init()
		{
			const byte MAX_DROPKIND = 5;
			int COUNT_BOARDAREA = boardInfo.boardSize.ix * boardInfo.boardSize.iy ;
			int COUNT_VIEWAREA = (boardInfo.GetMaxViewArea ()+1).ix * (boardInfo.GetMaxViewArea ()+1).iy;


			Vector3 pos = Vector3.zero;
			MonoDrop pDrop = null;
			Index2 ixy = Index2.Zero;
			for(int i=0 ; i < COUNT_BOARDAREA -30 ; i++)
			{
				ixy.ix = (int)(i% boardInfo.boardSize.ix); 
				ixy.iy = (int)(i/boardInfo.boardSize.ix); 
				pos.x = ixy.ix * m_boardInfo.squareWidth;
				pos.y = ixy.iy * m_boardInfo.squareHeight;


				pDrop = MonoDrop.Create(Single.OBJRoot.transform, 
				                        GetRandDrop(MAX_DROPKIND),
				                        pos);
				pDrop.SetIndex(ixy);
				m_mapDrop.Add(pDrop.id, pDrop);


				//------ setting drop of color that invisialbe 30~  ------------
				pDrop.SetColor(Color.gray); //chamto test
				if(null != pDrop && i >= COUNT_VIEWAREA)
				{
					pDrop.SetColor(Color.blue);
					pDrop.GetBoxCollider2D().enabled = false; //20150212 chamto - 터치입력을 못받게 충돌체를 비활성 시켜 놓는다.
				}

			}


			//임시로 map의 빈공간을 만들어준다.
			for (int i = COUNT_BOARDAREA -30; i < COUNT_BOARDAREA ; i++) 
			{
				ixy.ix = (int)(i% boardInfo.boardSize.ix); 
				ixy.iy = (int)(i/boardInfo.boardSize.ix); 

				m_mapDrop.SetValue(ixy,null);
			}

			//20150406 chamto - bug1-1 reproduction : The removal of non-union drop : at func LineInspection 
			//m_mapDrop.GetMonoDropByIndex2 (new Index2 (4, 5)).setDropKind = eResKind.Dark;
			//m_mapDrop.GetMonoDropByIndex2 (new Index2 (4, 4)).setDropKind = eResKind.Dark;
			//m_mapDrop.GetMonoDropByIndex2 (new Index2 (4, 3)).setDropKind = eResKind.Dark;
			//m_mapDrop.GetMonoDropByIndex2 (new Index2 (4, 2)).setDropKind = eResKind.Dark;

#if UNITY_EDITOR
			//20150403 chamto test
			DebugMap_Init (); 
			Update_DebugMap ();
#endif
		
			/*
			RandomTable<PercentItem>.Test ();
			//*/


		}


		Dictionary<Index2,MonoDrop> _debugMap = new Dictionary<Index2,MonoDrop>();
		public void DebugMap_Init()
		{

			int COUNT_BOARDAREA = boardInfo.boardSize.ix * boardInfo.boardSize.iy ;

			Vector3 pos = Vector3.zero;
			MonoDrop pDrop = null;
			Index2 ixy = Index2.Zero;
			for(int i=0 ; i<COUNT_BOARDAREA ; i++)
			{
				ixy.ix = (int)(i% boardInfo.boardSize.ix); 
				ixy.iy = (int)(i/boardInfo.boardSize.ix); 
				pos.x = ixy.ix * m_boardInfo.squareWidth;
				pos.y = ixy.iy * m_boardInfo.squareHeight;
				pDrop = MonoDrop.Create(Single.MonoDebug.UIMapRoot.transform, 
				                        eResKind.None,
				                        pos);

				pDrop.SetColor(Color.gray); 
				pDrop.GetBoxCollider2D().enabled = false; 

				_debugMap.Add(ixy,pDrop);
								
			}

		}

		public void Update_DebugMap()
		{
			int COUNT_BOARDAREA = boardInfo.boardSize.ix * boardInfo.boardSize.iy ;

			MonoDrop pDropInMap = null;
			MonoDrop pDropInDebug = null;
			Index2 ixy = Index2.Zero;
			for (int i=0; i<COUNT_BOARDAREA; i++) 
			{
				ixy.ix = (int)(i% boardInfo.boardSize.ix); 
				ixy.iy = (int)(i/boardInfo.boardSize.ix); 
				pDropInMap = this.mapDrop.GetMonoDropByIndex2(ixy);
				_debugMap.TryGetValue(ixy,out pDropInDebug);
				if(null != pDropInMap && null != pDropInDebug)
				{
					//Debug.Log("111----------------------------------------:"+pDropInMap);
					pDropInDebug.setDropKind = pDropInMap.dropKind;
					pDropInDebug.SetColor(Color.gray);

				}
				if(null == pDropInMap && null != pDropInDebug)
				{
					//Debug.Log("222----------------------------------------");
					pDropInDebug.setDropKind = eResKind.None;
				}
			}
		}

		public void Update()
		{
		}




		public void MoveDrop(MonoDrop movingDrop , Vector3 direction , ushort amountOfMovement)
		{
			if (null == movingDrop) 
			{
				return;
			}

			//길이를 1로 만든다 (정규화)
			direction.Normalize ();

			Index2 placedIndex = movingDrop.index2D;
			Index2 nextIndex = movingDrop.index2D;;
			MonoDrop nextDrop = null;
			for (ushort aOm = 1; aOm <= amountOfMovement; aOm++) 
			{

				nextIndex.ix += ((int)direction.x);
				nextIndex.iy += ((int)direction.y);
				nextDrop = this.mapDrop.GetMonoDropByIndex2 (nextIndex);

				//CDefine.DebugLog ("----------MoveDrop : "+ aOm +" :"+ nextIndex.ToString() + "  drop:"+nextDrop  ); //chamto test
				//if (null == nextDrop && this.boardInfo.BelongToViewArea (nextIndex)) 
				if (null == nextDrop && this.boardInfo.BelongToArea (boardInfo.GetMinBoardArea(), boardInfo.GetMaxBoardArea(), nextIndex)) 
				{
					placedIndex = nextIndex;
					//CDefine.DebugLog ("----------MoveDrop : " + movingDrop.index2D + "  placedIndex :" + placedIndex.ToString() + "  v3:"+direction  ); //chamto test
					continue;
				}
				break;
			}

			if (movingDrop.index2D != placedIndex) 
			{
				//CDefine.DebugLog ("----------MoveToIndex : " + placedIndex.ToString() ); //chamto test
				//chamto test , temp code
				if(this.boardInfo.BelongToArea(boardInfo.GetMinNonviewArea(),boardInfo.GetMaxNonviewArea() ,movingDrop.index2D)
				   && this.boardInfo.BelongToViewArea(placedIndex))
				{
					movingDrop.SetColor(Color.white);
					movingDrop.GetBoxCollider2D().enabled = true; //20150212 chamto - 터치입력을 못받게 충돌체를 비활성 시켜 놓는다.
				}

				movingDrop.MoveToIndex (placedIndex);
			}

		}

		public void WholeDropping(Index2 min , Index2 max)
		{

			const ushort AMOUNT_MOVEMENT = 5;
			int maxColumn = max.ix - min.ix;
			int maxRow = max.iy - min.iy;
			
			//CDefine.DebugLog("maxColumn : " + maxColumn + "  max:" + maxView + " min:" + minView);
			//maxRow = 1; //chamto test
			Index2 key = new Index2(0,0);
			MonoDrop value = null;
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy;
				
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					key.ix = ix;
					
					value = this.mapDrop.GetMonoDropByIndex2(key);
					//CDefine.DebugLog("WholeDroppingOnView : " + key.ToString() + "  " + value);
					if(null != value)
					{
						this.MoveDrop(value,Vector3.down,AMOUNT_MOVEMENT);

					}
				}
			}
		}
		public void WholeDroppingOnView()
		{
			//view area
			Index2 minView = this.boardInfo.GetIndexAt_ViewLeftBottom ();
			Index2 maxView = this.boardInfo.GetIndexAt_ViewRightUp ();

			WholeDropping (minView, maxView);
		}



		//temp code , 20150414 chamto 
//		private List<GroupDrop> m_ListJoinGroups = new List<GroupDrop>();
//		private Dictionary<Index2, List<ushort>> m_mapForGroupsInfo = new Dictionary<Index2, List<ushort>>();
//		public void SetGroupInfo(ushort groupNumber , Index2 dstIndex)
//		{
//			List<ushort> listGroupNumber = null;
//			if(false == m_mapForGroupsInfo.TryGetValue(dstIndex, out listGroupNumber))
//			{
//				listGroupNumber = new List<ushort>();
//				listGroupNumber.Add(groupNumber);
//				m_mapForGroupsInfo.Add(dstIndex, listGroupNumber);
//			}else
//			{
//				foreach(ushort getNum in listGroupNumber)
//				{
//					if(getNum == groupNumber)
//					{
//						return;
//					}
//				}
//				listGroupNumber.Add(groupNumber);
//			}
//		}
//
//		public void SetGroupsInfo_FourDir(ushort groupNumber, Index2 dstIndex)
//		{
//			this.SetGroupInfo (groupNumber, dstIndex);
//			this.SetGroupInfo (groupNumber, dstIndex + Index2.Up);
//			this.SetGroupInfo (groupNumber, dstIndex + Index2.Down);
//			this.SetGroupInfo (groupNumber, dstIndex + Index2.Left);
//			this.SetGroupInfo (groupNumber, dstIndex + Index2.Right);
//		}

		public BundleWithDrop FindJoinGroup_InFourWay(MonoDrop standardDrop)
		{
			if (null == standardDrop)
								return null;

			BundleWithDrop group = null;
			group = FindJoinGroup (standardDrop, standardDrop.index2D + Index2.Up);
			if (null != group)
				return group;

			group = FindJoinGroup (standardDrop, standardDrop.index2D + Index2.Right);
			if (null != group)
				return group;

			group = FindJoinGroup (standardDrop, standardDrop.index2D + Index2.Down);
			if (null != group)
				return group;

			group = FindJoinGroup (standardDrop, standardDrop.index2D + Index2.Left);
			if (null != group)
				return group;

			return group;
		}

		public BundleWithDrop FindJoinGroup(MonoDrop srcDrop, Index2 dstIdx)
		{
			//대상위치에 드롭이 있는지 검사
			MonoDrop dstDrop = this.mapDrop.GetMonoDropByIndex2(dstIdx);
			if(false == this.boardInfo.BelongToViewArea(dstIdx))
				dstDrop = null;

			if (null == srcDrop || null == dstDrop)
						return null;
			//드롭이 있다면 같은 드롭인지 검사
			if (srcDrop.dropKind != dstDrop.dropKind)
						return null;


			if(null != dstDrop.m_bundleInfo && null != dstDrop.m_bundleInfo.refBundle 
			   && null == dstDrop.m_bundleInfo.refBundle.lines)
				CDefine.DebugLogWarning("--------------------problem !! ------FindJoinGroup : srcIdx:"+srcDrop.index2D+" dstIdx:" + dstIdx); //chamto test

			//대상위치에 그룹이 있는지 검사
			//if (null == dstDrop.bundleInfo)
			if (null == dstDrop.m_bundleInfo || null == dstDrop.m_bundleInfo.refBundle //)
			    || null == dstDrop.m_bundleInfo.refBundle.lines)  //chamto temp
			{ 
			 		
				return null;
			}

			if (null != srcDrop.m_bundleInfo) 
			{
				//이미 같은 그룹주소이면 합칠 필요없음
				if(true ==  srcDrop.m_bundleInfo.EqualRefBundle(dstDrop.m_bundleInfo))
				{
					return null;
				}
			}

			return dstDrop.m_bundleInfo;
		}

		delegate bool AvailableJoin(MonoDrop start , out int jump);
		public List<List<MonoDrop>> LineInspection(Index2 startIxy, Vector3 direction, ushort lengthOfLine, ushort minJoin)
		{

			AvailableJoin d_availableJoin = delegate(MonoDrop d_start , out int d_jump) 
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
			};




			int jumpCount = 0;
			Index2 compareIndex = startIxy;
			Index2 nextIndex = startIxy;
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

		public GroupDrop FindJoinConditions(ushort minJoin)
		{

			m_groupDrop.Clear ();
			//this.DismissGroupInfoWithDrop (); //chamto test

			//List<List<MonoDrop>> listLineTotal = null;
			Index2 minView = this.boardInfo.GetIndexAt_ViewLeftBottom ();
			Index2 maxView = this.boardInfo.GetIndexAt_ViewRightUp ();
			int maxColumn = maxView.ix - minView.ix;
			int maxRow = maxView.iy - minView.iy;


			Index2 key = Index2.Zero;
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy;
				key.ix = 0;
				//listLineTotal = LineInspection(key, Vector3.right, (ushort)maxColumn, minJoin);
				LineInspection(key, Vector3.right, (ushort)maxColumn, minJoin);
				
			}//endfor

			for (int ix=0; ix <= maxColumn; ix++) 
			{
				key.iy = 0;
				key.ix = ix;
				//listLineTotal = LineInspection(key, Vector3.up, (ushort)maxRow, minJoin);
				LineInspection(key, Vector3.up, (ushort)maxRow, minJoin);
				
			}//endfor


			m_groupDrop.UpdateMap();
			//this.MoveAllJoinDrops ();

			//m_groupDrop.DismissRefDrop (); //chamto test

#if UNITY_EDITOR
			Udpate_DebugGroupInfo (); //chamto test
#endif


			return m_groupDrop;
		}
	
		public Dictionary<Index2,MonoDrop> MoveNextJoinDrops()
		{
			Dictionary<Index2,MonoDrop> mapBundle = m_groupDrop.Next ();
			//Debug.Log (mapBundle + " - MoveNextJoinDrops" );  //chamto test
			if (null == mapBundle)
								return null;
			foreach (MonoDrop drop in mapBundle.Values) 
			{
				MonoDrop.MoveToEmptySquare(drop);
			}

			return mapBundle;
		}


		//temp code
		public void DismissGroupInfoWithDrop()
		{
			Index2 minView = this.boardInfo.GetIndexAt_ViewLeftBottom ();
			Index2 maxView = this.boardInfo.GetIndexAt_ViewRightUp ();
			int maxColumn = maxView.ix - minView.ix;
			int maxRow = maxView.iy - minView.iy;
			
			
			Index2 key = Index2.Zero;
			MonoDrop drop = null;
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy;
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					key.ix = ix;
					drop = mapDrop.GetMonoDropByIndex2(key);
					if(null != drop && null != drop.m_bundleInfo)
					{
						drop.m_bundleInfo = null;
					}
					
				}//endfor
				
			}//endfor
		}

		//temp code
		public void MoveAllJoinDrops()
		{
			Index2 minView = this.boardInfo.GetIndexAt_ViewLeftBottom ();
			Index2 maxView = this.boardInfo.GetIndexAt_ViewRightUp ();
			int maxColumn = maxView.ix - minView.ix;
			int maxRow = maxView.iy - minView.iy;
			
			
			Index2 key = Index2.Zero;
			MonoDrop drop = null;
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy;
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					key.ix = ix;
					drop = mapDrop.GetMonoDropByIndex2(key);
					if(null != drop && null != drop.m_bundleInfo)
					{
						MonoDrop.MoveToEmptySquare(drop);
					}

				}//endfor

			}//endfor
		}

		public void Udpate_DebugGroupInfo()
		{
			Index2 minView = this.boardInfo.GetIndexAt_ViewLeftBottom ();
			Index2 maxView = this.boardInfo.GetIndexAt_ViewRightUp ();
			int maxColumn = maxView.ix - minView.ix;
			int maxRow = maxView.iy - minView.iy;
			
			
			Index2 key = Index2.Zero;
			MonoDrop drop = null;
			MonoDrop drop2 = null;
			for (int iy=0; iy <= maxRow; iy++) 
			{
				key.iy = iy;
				for (int ix=0; ix <= maxColumn; ix++) 
				{
					key.ix = ix;

					_debugMap.TryGetValue(key,out drop);
					drop2 = mapDrop.GetMonoDropByIndex2(key);
					drop.SetColor(Color.gray);

					if(null != drop2 && null != drop2.m_bundleInfo && null != drop2.m_bundleInfo.refBundle)
					{

						switch(drop2.dropKind)
						{
							case eResKind.Red:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Blue:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Dark:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Green:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Heart:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Light:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Obstruction:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							case eResKind.Posion:
							{
							drop.SetColor(new Color(2f,2f,2f));
								break;
							}
							default:
							{
								drop.SetColor(Color.gray);
								break;
							}
						}

						
					}//endif

					if(null != drop2 && null != drop2.m_bundleInfo && null != drop2.m_bundleInfo.refBundle && null == drop2.m_bundleInfo.refBundle.lines)
					{
						drop.SetColor(new Color(1,0,0,0.7f));
					}	
					
				}//endfor
				
			}//endfor
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
			Vector3 putPos_left_up = m_boardInfo.GetPositionAt_ViewLeftUp () + Single.OBJRoot.transform.position;
			Vector3 putPos_right_up = m_boardInfo.GetPositionAt_ViewRightUp () + Single.OBJRoot.transform.position;
			Vector3 putPos_left_bottom = m_boardInfo.GetPositionAt_ViewLeftBottom () + Single.OBJRoot.transform.position;
			Vector3 putPos_right_bottom = m_boardInfo.GetPositionAt_ViewRightBottom () + Single.OBJRoot.transform.position;
			
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
			
			Bounds bob = m_boardInfo.GetBoundaryOfView (Single.OBJRoot.transform.position);
			ML.LineSegment3 result = new ML.LineSegment3();
			result.origin = srcDrop.gotoWorldPosition;
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
			//note!! before moveing , dstDrop is gotoPosition 
			return (toDstPos - standardPos).sqrMagnitude;
		}

		/// <summary>
		/// Gets the shortest distance.
		/// </summary>
		/// <returns>The shortest distance.</returns>
		/// <param name="standardDrop">Standard drop.</param>
		/// <param name="minDistance">최소거리의 최소값을 지정. 최소값이 5라면, 최소거리는 적어도 5보다 같거나 커야한다.</param>
		//public MonoDrop GetShortestDistance(MonoDrop standardDrop, float minDistance)
		public MonoDrop FindDropShortestDistance(MonoDrop standardDrop, float minDistance)
		{
			if(null == standardDrop || 0 >= minDistance) return null;
			if(0 == m_mapDrop.DtnrId.Count) return null;

			List<MonoDrop> list = m_mapDrop.DtnrId.Values.ToList();
			//list.Remove(standardDrop); //deduplicate

			//list.Sort(SortDistanceCompareTo);

			//기준점으로 부터 제곱길이가 가장 작은순으로 정렬한 드롭목록을 얻는다.
			list = (from dstDrop in list
			        orderby GetSqrDistance(standardDrop.gotoWorldPosition,dstDrop.gotoWorldPosition) ascending
					select dstDrop).ToList();

			//foreach(MonoDrop drop in list)			
			//	CDefine.DebugLog(drop + "  " + Math.Sqrt(GetSqrDistance(standardDrop,drop)));
			//CDefine.DebugLog(GetSqrDistance(standardDrop,list[0]) + "  " + (minDistance*minDistance)); //chamto test
			if(GetSqrDistance(standardDrop.gotoWorldPosition,list[0].gotoWorldPosition) <= (minDistance * minDistance)) return null;
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
			ls3.origin = srcDrop.gotoWorldPosition;
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
			Single.MonoDebug.lineRender.useWorldSpace = false;
			//-------------------------------------------------------------------------
#endif

			//2. 3. 모든드롭에 대해 전수 조사한다. (선분 근처 드롭만 조사하게 최적화 필요)
			//SortedDictionary<float , MonoDrop> collisionDtnr = new Dictionary<float , MonoDrop> ();
			List<MonoDrop> collisionList = new List<MonoDrop> ();
			float t_c = 0.0f;
			foreach (MonoDrop dstDrop in m_mapDrop.DtnrId.Values) 
			{
				//self exclusion
				if(srcDrop == dstDrop) continue;

				if((nonCollision_minDistance * nonCollision_minDistance) > ls3.MinimumDistanceSquared(dstDrop.gotoWorldPosition,out t_c))
				{
					//20140907 chamto test
//					float dist = ls3.MinimumDistanceSquared(dstDrop.gotoWorldPosition,out t_c);
//					{
//						CDefine.DebugLog("object : "+dstDrop  +"  mSqrtDist : "+dist + " noncSqrtDist : " + nonCollision_minDistance * nonCollision_minDistance + "  gotowp : "+dstDrop.gotoWorldPosition + " t_c : " + t_c);
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
			                         orderby GetSqrDistance(srcDrop.gotoWorldPosition,dstDrop.gotoWorldPosition) ascending
			                         select dstDrop).ToList();


//			CDefine.DebugLog ("----- line -----  ori :" + ls3.origin + "  last : " + ls3.last + " dict : " + ls3.direction); //chamto test

			//20150331 chamto test
			if (0 != result.Count) 
			{
				string sss = "";
				foreach(MonoDrop mm  in result)
				{
					sss += mm.index2D.ToString() + " | ";
				}
				
				CDefine.DebugLog (sss);
			}


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
			foreach(MonoDrop dstDrop in m_mapDrop.DtnrId.Values)
			{
				//self exclusion
				if(srcDrop == dstDrop) continue;

				//dstBox.center = new Vector2(dstDrop.transform.position.x,dstDrop.transform.position.y);
				dstBox.center = new Vector2(dstDrop.gotoWorldPosition.x , dstDrop.gotoWorldPosition.y);

				if(true == srcBox.Overlaps(dstBox,true)) //include allowInverse
				{
					return dstDrop;
				}

			}

			return null;
		}

		




	}//end class
}//end namespace




