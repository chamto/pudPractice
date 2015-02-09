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






	/// <summary>
	/// C drop manager.
	/// 드롭을 생성/제거/배치 하는 관리 객체
	/// </summary>
	public class CDropManager
	{
		
		//==============: member variables :==============
		private Dictionary<int,MonoDrop> 		m_dtnrDrop = new Dictionary<int,MonoDrop>();
		//private Dictionary<float,MonoDrop> 	m_dtnrMovedPath = new Dictionary<float,MonoDrop>();
		private NDrop.CPath m_dropPath = new NDrop.CPath();


		//------------------------------------------------------------------------
		// currection method
		//------------------------------------------------------------------------

		public Bounds GetBoundaryOfBoard()
		{
			Vector3 center, size;
			size.x = ConstDrop.UI_Width * ConstBoard.Max_Row;
			size.y = ConstDrop.UI_Height * ConstBoard.Max_Column;
			size.z = 0;

			center.x = Single.UIRoot.transform.position.x + (size.x * 0.5f) - (ConstDrop.UI_Width * 0.5f);
			center.y = Single.UIRoot.transform.position.y - (size.y * 0.5f) + (ConstDrop.UI_Height * 0.5f);
			center.z = 0;

			//-------------------------------------------------------------------------
			//20140906 chamto test
			//-------------------------------------------------------------------------
//			Single.MonoDebug.boundary.transform.position = center;
//			Single.MonoDebug.boundary.transform.localScale = size;
			//-------------------------------------------------------------------------

			return new Bounds (center, size);
		}


		public ML.LineSegment3 CorrectionLineSegment(MonoDrop srcDrop , ML.LineSegment3 lineSeg3)
		{
			if (null == srcDrop)
								return lineSeg3;

			//Correction value
			//PairInt parameter is array index(0 start , 1 is not ). 
			Vector3 putPos_left_up = GetPositionOfPutDrop (new PairInt (0, 0));
			Vector3 putPos_right_up = GetPositionOfPutDrop (new PairInt (0, (int)ConstBoard.Max_Row-1));
			Vector3 putPos_left_bottom = GetPositionOfPutDrop (new PairInt ((int)ConstBoard.Max_Column-1, 0));
			Vector3 putPos_right_bottom = GetPositionOfPutDrop (new PairInt ((int)ConstBoard.Max_Column-1, (int)ConstBoard.Max_Row-1));


			//-------------------------------------------------------------------------
			//20140906 chamto test
			//-------------------------------------------------------------------------
//			Single.MonoDebug.cube01.transform.position = putPos_left_up;
//			Single.MonoDebug.cube02.transform.position = putPos_right_up;
//			Single.MonoDebug.cube03.transform.position = putPos_left_bottom;
//			Single.MonoDebug.cube04.transform.position = putPos_right_bottom;
			//-------------------------------------------------------------------------

			Bounds bob = GetBoundaryOfBoard ();
			ML.LineSegment3 result = new ML.LineSegment3();
			result.origin = srcDrop.firstWorldPosition;
			result.last = lineSeg3.last;



			//CDefine.DebugLog ("Bounds " + bob + bob.min + bob.max);
			if (lineSeg3.last.y >= bob.max.y) 
			{	//correction up
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
			{	//correction bottom
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
			{	//correction left
				result.last_x = putPos_left_up.x;
				//CDefine.DebugLog("----correction left");
			}else
				if(lineSeg3.last.x >= bob.max.x)
			{	//correction right
				result.last_x = putPos_right_up.x;
				//CDefine.DebugLog("----correction right");
			}else 
			{
				//correction is not required
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
			float dist = ConstDrop.UI_Width * ConstDrop.UI_Width * 2 + 0.15f;

			return (dist > GetSqrDistance (drop1.firstWorldPosition, drop2.firstWorldPosition));
		}


		//------------------------------------------------------------------------
		// drop method
		//------------------------------------------------------------------------

		public bool SwapMonoDropInBoard(int keyOfPos1 , int keyOfPos2 , bool applyPosition)
		{

			if (keyOfPos1 == keyOfPos2)
								return false; 

			MonoDrop temp1 = null;
			MonoDrop temp2 = null;
			if (false == m_dtnrDrop.TryGetValue (keyOfPos1, out temp1))
								return false;

			if (false == m_dtnrDrop.TryGetValue (keyOfPos2, out temp2))
								return false;

			if (false == ValidSwapMonoDrop (temp1, temp2))
								return false;

			//1. swap position of dtnr 
			m_dtnrDrop.Remove (keyOfPos1);
			m_dtnrDrop.Remove (keyOfPos2);
			m_dtnrDrop.Add (keyOfPos1, temp2);
			m_dtnrDrop.Add (keyOfPos2, temp1);


			//2. swap key of monoDrop
			temp1.keyOfPosition = keyOfPos2;
			temp2.keyOfPosition = keyOfPos1;

			//3. swap localPosition of monoDrop
			SwapFirstLocalPosition (temp1, temp2);
			if (true == applyPosition) 
			{
				temp1.transform.localPosition = temp1.firstLocalPosition;
				temp2.transform.localPosition = temp2.firstLocalPosition;
			}

			//4. swap name of monoDrop
			temp1.name = "drop" + temp1.keyOfPosition.ToString("000");
			temp2.name = "drop" + temp2.keyOfPosition.ToString("000");


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
			const float WIDTH_DROP = 1.15f;
			const float HEIGHT_DROP = 1.15f;
			const uint MAX_DROP_COLUMN = 6;
			const uint MAX_DROP6X5 = 6 * 5;
			const uint MAX_DROP6X5X2 = 6 * 5 * 2;
			const byte MAX_DROPKIND = 6;
			eResKind[] dropKind = new  eResKind[MAX_DROPKIND];
			dropKind[0] = eResKind.Red;
			dropKind[1] = eResKind.Green;
			dropKind[2] = eResKind.Blue;
			dropKind[3] = eResKind.Light;
			dropKind[4] = eResKind.Dart;
			dropKind[5] = eResKind.Heart;

			System.Random rndDrop = new System.Random();

			MonoDrop pDrop = null;
			for(int i=0 ; i<MAX_DROP6X5 ; i++)
			{
				//6x5 : width6 height5
				pDrop = CreateDrop(i, Single.UIRoot.transform, dropKind[rndDrop.Next(0,MAX_DROPKIND)] , 
				                   (i% MAX_DROP_COLUMN) * WIDTH_DROP ,(i/MAX_DROP_COLUMN) * -HEIGHT_DROP);
				m_dtnrDrop.Add(i,pDrop);

				//------ setting drop of color that invisialbe 0~29  ------------
				//if(null != pDrop && i < MAX_DROP6X5)
				//{
				//	//pDrop
				//}

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
		/// Changes the key of position.
		/// </summary>
		/// <returns>The key of position.</returns>
		/// <param name="column">Column.</param>
		/// <param name="row">Row.</param>
		public int ChangeKeyOfPosition(int column, int row)
		{
			return (int)(column * ConstBoard.Max_Row + row);
		}
		
		/// <summary>
		/// Changes the pair in matrix on board
		/// </summary>
		/// <returns>The pair in matrix.</returns>
		/// <param name="keyOfPosition">Key of position.</param>
		public PairInt ChangePairInMatrix(int keyOfPosition)
		{
			return new PairInt (
				(int)(keyOfPosition / ConstBoard.Max_Row) , 
				(int)(keyOfPosition % ConstBoard.Max_Row)
				);
		}


		public MonoDrop GetMonoDrop(PairInt dropPair)
		{
			int keyOfPos1 = this.ChangeKeyOfPosition (dropPair.column, dropPair.row);
			
			MonoDrop temp1 = null;
			if (false == m_dtnrDrop.TryGetValue (keyOfPos1, out temp1))
				return null;
			
			return temp1;
		}
		
		
		/// <summary>
		/// Get Position of put Drop of board
		/// </summary>
		/// <returns> center position of put drop.</returns>
		/// <param name="dropPair">Drop pair.</param>
		public Vector3 GetPositionOfPutDrop(PairInt dropPair)
		{
			Vector3 posOfPut;
			posOfPut.x = Single.UIRoot.transform.position.x + ConstDrop.UI_Width * dropPair.row;
			posOfPut.y = Single.UIRoot.transform.position.y - ConstDrop.UI_Height * dropPair.column;
			posOfPut.z = Single.UIRoot.transform.position.z;
			
			return posOfPut;
		}

		private GameObject CreatePrefab(string path)
		{
			const string root = "Prefab/";
			return MonoBehaviour.Instantiate(Resources.Load(root + path)) as GameObject;
		}

		public void SetTheDropSprite(SpriteRenderer sprRd , eResKind eDrop)
		{
			if(null == sprRd) 
			{
				CDefine.DebugLogError(string.Format("SpriteRenderer is null"));
				return;
			}


			//"Texture/drop/dropHeart"
			//Array.IndexOf(

			sprRd.sprite = Single.ResMgr.LoadSprites(eDrop,SResDefine.spINDEX_ZERO);

		}

		/// <summary>
		/// Creates the drop.
		/// </summary>
		/// <returns>The drop.</returns>
		/// <param name="in_eKind">Dropkind.</param>
		/// <param name="relativeCoord_x">Relative coordinate X.</param>
		/// <param name="relativeCoord_y">Relative coordinate Y.</param>
		public MonoDrop CreateDrop(int keyOfPosition, Transform parent , eResKind eDrop , float relativeCoord_x , float relativeCoord_y)
		{

			GameObject newObj = CreatePrefab(SResDefine.pfDROPINFO);
			if(null == newObj) 
			{
				CDefine.DebugLogError(string.Format("Failed to create Prefab : " + SResDefine.pfDROPINFO));
				return null;
			}
			MonoDrop drop = newObj.GetComponent<MonoDrop>();
			if(null == drop) 
			{
				CDefine.DebugLogError(string.Format("MonoDrop is null"));
				return null;
			}
//			SpriteRenderer sprRd = newObj.GetComponentInChildren<SpriteRenderer>();
//			if(null == sprRd) 
//			{
//				CDefine.DebugLogError(string.Format("SpriteRenderer is null"));
//				return null;
//			}

			//-------------------------------------------------

			drop.keyOfPosition = keyOfPosition;
			newObj.name = "drop" + keyOfPosition.ToString("000");

			//CDefine.DebugLog(newObj.transform.localPosition+ "before--------"); //chamto test

			if(null != parent)
			{	//Specify the parent object
				newObj.transform.parent = parent.transform;
			}

			//CDefine.DebugLog(newObj.transform.localPosition+ "after--------"); //chamto test

			//SetTheDropSprite(sprRd , eDrop);

			//newObj.transform.position = new Vector3(relativeCoord_x,relativeCoord_y,0); //[주의!!] 부모에 대한 상대좌표를 지정해야 하기 때문에 localposition 을 써야 한다.  
			newObj.transform.localPosition = new Vector3(relativeCoord_x,relativeCoord_y,0);


			drop.kind = eDrop;
			drop.firstLocalPosition = newObj.transform.localPosition;

			return drop;

		}



		public void RemoveDrop(int keyOfPosition)
		{
			MonoDrop drop = GetMonoDrop(ChangePairInMatrix(keyOfPosition));
			if (null != drop) 
			{
				//drop.transform.
			}
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
			if(0 == m_dtnrDrop.Count) return null;

			List<MonoDrop> list = m_dtnrDrop.Values.ToList();
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


			//LineRenderer lr = new LineRenderer ();

			//-------------------------------------------------------------------------
			//20140906 chamto - test
			//-------------------------------------------------------------------------
			//Single.MonoDebug.lineRender.SetWidth (0.1f, 0.4f);
			//Single.MonoDebug.lineRender.SetPosition (0, ls3.origin);
			//Single.MonoDebug.lineRender.SetPosition (1, ls3.direction + ls3.origin);
			//-------------------------------------------------------------------------

			//2. 3. 모든드롭에 대해 전수 조사한다. (선분 근처 드롭만 조사하게 최적화 필요)
			//SortedDictionary<float , MonoDrop> collisionDtnr = new Dictionary<float , MonoDrop> ();
			List<MonoDrop> collisionList = new List<MonoDrop> ();
			float t_c = 0.0f;
			foreach (MonoDrop dstDrop in m_dtnrDrop.Values) 
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
			foreach(MonoDrop dstDrop in m_dtnrDrop.Values)
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




