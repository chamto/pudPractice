using UnityEngine;
using System;
using PuzzAndBidurgi;

public class MonoMain : MonoBehaviour 
{
	
	//==============: member variables :==============
	public GameObject m_objRoot = null;

	private BaseRules m_rules = null;
	//==============: member Property :==============
	public BaseRules rules
	{
		get 
		{
			return m_rules;
		}
	}


	//==============: member method :==============
	void Awake()
	{
		CSingleton<CResoureManager>.Instance.Init();
		//CSingleton<CDropManager>.Instance.Init();
		MonoStage.Create ("0_Stage_Practics");
		m_rules = Rule_PuzzleAndDragon.Factory ();

	}
	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		//CDefine.DebugLog ("Update Time : " + Time.deltaTime);
		CSingleton<CDropManager>.Instance.Update ();

		if (null != m_rules)
						m_rules.Update ();
		
	}

	//ref : http://unity3d.com/learn/tutorials/modules/beginner/scripting/update-and-fixedupdate
	void FixedUpdate()
	{

		//CDefine.DebugLog ("FixedUpdate Time : " + Time.deltaTime);
	}


	

}