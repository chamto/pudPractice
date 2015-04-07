using UnityEngine;
using System.Collections;

/// <summary>
/// 20140724 chamto 
/// 유티니 플랫폼별 입력처리를 공통의 인터페이스로 묶은 중계라이브러리
/// </summary>
public class Input_Unity
{

	public static bool IsTouch()
	{
		//CDefine.DebugLog("IsTouchCount : " + Input.touchCount);

		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//CDefine.DebugLog("1  IsTouchCount : " + Input.touchCount);
			return (Input.touchCount > 0);
			//return (Input.touchCount > 0 || Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved);
			//return Input.GetMouseButton(0);
		}else if(Application.platform == RuntimePlatform.OSXEditor)
		{
			//CDefine.DebugLog("2  IsTouchCount : " + Input.touchCount);
			return Input.GetMouseButton(0);
		}
		
		return false;
	}

	public static Vector2 GetTouchPos()
	{
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Input.GetTouch(0).position;
		}else if(Application.platform == RuntimePlatform.OSXEditor)
		{
			return Input.mousePosition;
		}
		
		return Vector2.zero;
	}

	public static Vector3 GetTouchWorldPos()
	{
		Vector3 pos = Input_Unity.GetTouchPos ();

		return Camera.main.ScreenToWorldPoint (pos);
	}

	public static bool GetMouseButtonMove(int button)
	{
		if (Input.GetMouseButton (button) && Input.GetMouseButtonDown (button) == false) 
		{
			return true;
		}

		return false;
	}
}
