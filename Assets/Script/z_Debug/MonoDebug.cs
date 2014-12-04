using UnityEngine;
using UnityEditor;
using System.Collections;

public class MonoDebug : MonoBehaviour 
{

	public LineRenderer lineRender = null;
	public GameObject cube01 = null;
	public GameObject cube02 = null;
	public GameObject cube03 = null;
	public GameObject cube04 = null;
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
		if (GUI.Button (new Rect (10, 10, 100, 70), "Time -")) 
		{
			Time.timeScale -= 0.1f;
			CDefine.DebugLog ("Time.timeScale : " + Time.timeScale);
		}

		if (GUI.Button (new Rect (10, 10+70, 100, 70), "Time +")) 
		{
			Time.timeScale += 0.1f;
			CDefine.DebugLog ("Time.timeScale : " + Time.timeScale);
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
