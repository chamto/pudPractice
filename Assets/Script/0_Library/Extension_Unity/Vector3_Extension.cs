// ------------------------------------------------------------------------------
// author :  chamto 
// date   :  20140822
// update :  
// description : 유니티 ML벡터3 객체의 확장기능을 모아 놓은 객체이다.
// ------------------------------------------------------------------------------
using UnityEngine;

namespace ML
{
	public partial struct Vector3
	{

		///‘=‘ 연산자 재정의.  복사생성자를 재정의함
		///ex) ML.Vector3 = UnityEngine.Vector3 
		public static implicit operator Vector3(UnityEngine.Vector3 uniV3)
		{
			return new ML.Vector3(uniV3.x, uniV3.y, uniV3.z);
		}

		public static implicit operator UnityEngine.Vector3(ML.Vector3 mlV3)
		{
			return new UnityEngine.Vector3(mlV3.x, mlV3.y, mlV3.z);
		}

	}


}