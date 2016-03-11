using UnityEngine;
using System.Collections;

public class TestClosestPoints : MonoBehaviour 
{

	public GameObject _line1Start = null;
	public GameObject _line1End = null;
	public GameObject _line2Start = null;
	public GameObject _line2End = null;
	public GameObject _cpStart = null;
	public GameObject _cpEnd = null;

	public LineRenderer _lr_line1 = null;
	public LineRenderer _lr_line2 = null;
	public LineRenderer _lr_closestPoints = null;
	public LineRenderer _lr_lineSegementDistance = null;
	public TextMesh		_textMesh = null;

	ML.Vector3 _clPoint1, _clPoint2;

	// Use this for initialization
	void Start () 
	{

		_clPoint1 = ML.Vector3.Zero;
		_clPoint2 = ML.Vector3.Zero;

	}

	public void UpdateDistanceSquared()
	{
		float s_c=0, t_c=0;
		ML.LineSegment3 ls1, ls2;
		ls1.origin = _line1Start.transform.position;
		ls1.direction = _line1End.transform.position - _line1Start.transform.position;
		ls1.direction = ls1.direction.Normalize ();
		ls1.last = _line1End.transform.position;

		ls2.origin = _line2Start.transform.position;
		ls2.direction = _line2End.transform.position - _line2Start.transform.position;
		ls2.direction = ls2.direction.Normalize ();
		ls2.last = _line2End.transform.position;

		//두 선분의 최소 거리를 구한다.
		float distance = ML.LineSegment3.DistanceSquared (ls1, ls2, out s_c, out t_c);
		distance = Mathf.Sqrt (distance);
		_textMesh.text = distance.ToString ();

		//두 선분의 가장 가까운 점을 구한다. = 선분시작점 + 스칼라값 * 방향
		//두 선분 위에 있는 점을 시각적으로 연결한다.
		//이 연결한 선분의 길이가 최소거리이다.
		Vector3 lsd1 , lsd2;
		lsd1 = ls1.origin + (s_c * ls1.direction);
		_lr_lineSegementDistance.SetPosition(0, lsd1);
		lsd2 = ls2.origin + (t_c * ls2.direction);
		_lr_lineSegementDistance.SetPosition(1, lsd2);

		//위에서 구한 선분의 중간위치에 글자를 위치시킨다.
		//z값도 반으로 나뉘므로 특정값으로 고정한다.
		_textMesh.transform.position = (lsd1 + lsd2) /2;
		_textMesh.transform.position = new Vector3 (_textMesh.transform.position.x, _textMesh.transform.position.y, 
		                                           5);
	}
	
	public void UpdateClosestPoints_LineSegment3()
	{

		ML.LineSegment3 ls1, ls2;
		ls1.origin = _line1Start.transform.position;
		ls1.direction = _line1End.transform.position - _line1Start.transform.position;
		ls1.direction = ls1.direction.Normalize ();
		ls1.last = _line1End.transform.position;
		
		ls2.origin = _line2Start.transform.position;
		ls2.direction = _line2End.transform.position - _line2Start.transform.position;
		ls2.direction = ls2.direction.Normalize ();
		ls2.last = _line2End.transform.position;
		

		ML.LineSegment3.ClosestPoints (out _clPoint1, out _clPoint2, ls1, ls2);
		
		
		_lr_closestPoints.SetPosition (0, _clPoint1);
		_lr_closestPoints.SetPosition (1, _clPoint2);
		
		_cpStart.transform.position = _clPoint1;
		_cpEnd.transform.position = _clPoint2;
	}

	public void UpdateClosestPoints_Line3()
	{
		
		ML.Line3 _line1, _line2;
		_line1.origin = _line1Start.transform.position;
		_line1.direction = _line1End.transform.position - _line1Start.transform.position;
		_line2.origin = _line2Start.transform.position;
		_line2.direction = _line2End.transform.position - _line2Start.transform.position;
		ML.Line3.ClosestPoints (out _clPoint1, out _clPoint2, _line1, _line2);


		_lr_closestPoints.SetPosition (0, _clPoint1);
		_lr_closestPoints.SetPosition (1, _clPoint2);

		_cpStart.transform.position = _clPoint1;
		_cpEnd.transform.position = _clPoint2;
	}

	void Update () 
	{
		_lr_line1.SetPosition (0, _line1Start.transform.position);
		_lr_line1.SetPosition (1, _line1End.transform.position);
		_lr_line2.SetPosition (0, _line2Start.transform.position);
		_lr_line2.SetPosition (1, _line2End.transform.position);

		//비교1 : 두선분의 가장 가까운 점을 구한다.
		//UpdateClosestPoints_LineSegment3 ();

		//비교2 : 두직선의 가장 가까운 점을 구한다.
		UpdateClosestPoints_Line3 ();


		UpdateDistanceSquared ();
	}
}



