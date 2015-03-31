using UnityEngine;
using System.Collections;

public class TestFuncMinDist : MonoBehaviour 
{

	public LineRenderer lineRender;
	public GameObject	lineSrc;
	public GameObject	lineDest;
	public GameObject	point;
	public float t_c=0;
	public float minDistSquared=0;
	public GUIText		guiText2;

	private ML.LineSegment3 lineSegment; 


	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		lineSegment.origin = lineSrc.transform.position;
		lineSegment.last = lineDest.transform.position;
		minDistSquared = lineSegment.MinimumDistanceSquared (point.transform.position, out t_c);
		lineRender.SetPosition (0, lineSrc.transform.position);
		lineRender.SetPosition (1, lineDest.transform.position);
		guiText2.text = "t_c : " + t_c.ToString() + "  minDistSqu : " + minDistSquared.ToString ();
	}
}
