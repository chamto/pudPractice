using UnityEngine;
using System.Collections;

public class TestRandom : MonoBehaviour 
{

	public GameObject cube1 = null;
	public GameObject cube2 = null;
	public GUIText text = null;

	System.Random rndDrop = new System.Random();
	int rand = 0;

	int tryNumber = 0;
	int trySuccess = 0;

	// Use this for initialization
	void Start () 
	{



	}

	int accumulation = 0;

	// Update is called once per frame
	void Update () 
	{

		rand = rndDrop.Next (0, 4);

		Debug.Log (rand);
		if (null != cube1) 
		{
			if(1 == rand)
				cube1.gameObject.renderer.material.color = Color.blue;
			else
				cube1.gameObject.renderer.material.color = Color.white;
		}

		if (1 != rand)
			accumulation = 0;
		else
			accumulation++;

		if (null != cube2) 
		{

			if(4 <= accumulation)
			{
				Debug.LogWarning ("-----------accumulation-"+accumulation+"-!-------------");
				cube2.gameObject.renderer.material.color = Color.red;
			}
			else
				cube2.gameObject.renderer.material.color = Color.white;
		}

	}

#if UNITY_EDITOR
	void OnGUI()
	{
		if (GUI.Button (new Rect (10, 10, 100, 70), "Reinforce 25%")) 
		{
			tryNumber++;
			//rand = rndDrop.Next (0, 4);

			if(null != text)
			{
				if(1 == rand)
				{
					trySuccess++;
					text.text = "success try:" + (((float)trySuccess / (float)tryNumber)*100.0f) + "%";
				}else
				{
					text.text = "failure";
				}
			}


		}
	}
#endif
}
