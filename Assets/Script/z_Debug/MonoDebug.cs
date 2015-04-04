using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MonoDebug : MonoBehaviour 
{

	public GameObject	UIMapRoot = null;
	public LineRenderer lineRender = null;
	public GameObject cube_LeftUp = null;
	public GameObject cube_RightUp = null;
	public GameObject cube_LeftBottom = null;
	public GameObject cube_RightBottom = null;
	public GameObject boundary = null;
	public GameObject firstCube = null;


	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}


#if UNITY_EDITOR
	void OnGUI()
	{
		if (GUI.Button (new Rect (10, 10, 100, 70), "Whole Dropping")) 
		{
			//Time.timeScale -= 0.1f;
			//CDefine.DebugLog ("Time.timeScale : " + Time.timeScale);

//			if(Single.DropMgr.mapDrop.Remove(1))
//			{
//				CDefine.DebugLog("Drop Remove 1 ");
//			}


			CDefine.DebugLog("WholeDroppingOnView start -----------------------------");
			Single.DropMgr.WholeDroppingOnView();

			//Single.DropMgr.mapDrop.Debug_PrintMap();

		}

		if (GUI.Button (new Rect (10, 10+70, 100, 70), "Update DebugMap")) 
		{
			//Time.timeScale += 0.1f;
			//CDefine.DebugLog ("Time.timeScale : " + Time.timeScale);
			Single.DropMgr.Update_DebugMap ();
		}

		Rule_PuzzleAndDragon rule = Single.MonoMain.rules as Rule_PuzzleAndDragon; 
		if (GUI.Button (new Rect (10, 10+70+70, 100, 70), "Custom : " + rule.stateElapsedTime + " second")) 
		{
			Single.MonoMain.rules.NextState();

			CDefine.DebugLog ("current State : " + rule.state);
		}
	}
#endif

}
