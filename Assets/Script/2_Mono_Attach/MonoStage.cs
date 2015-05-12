using UnityEngine;
using System.Collections;
using PuzzAndBidurgi;

public class MonoStage : MonoBehaviour {


	void Awake()
	{
		//CDefine.DebugLog ("---------awake"); //chamto test
		CSingleton<CDropManager>.Instance.Init();
	}

	// Use this for initialization
	void Start () 
	{
		//CDefine.DebugLog ("---------start"); //chamto test


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	static public MonoStage Create(string stageName)
	{
		GameObject obj = new GameObject ();
		obj.name = stageName;
		GameObject zero_mono = GameObject.Find ("0_Mono");
		if (null != zero_mono) 
		{
			obj.transform.parent = zero_mono.transform;		
		}
		return obj.AddComponent<MonoStage> ();
	}
}
