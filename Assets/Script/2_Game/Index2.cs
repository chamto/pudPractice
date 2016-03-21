
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;



[System.Serializable]
public struct Index2
{

	//max_column = root(int_max + 1)
	//min_column = root(int_min - 1)
	static int MAX_COLUMN = 46340;
	static int MIN_COLUMN = -46341;

	public int ix;
	public int iy;

	static public Index2 Zero
	{
		get 
		{
			return new Index2(0,0);
		}
	}

	static public Index2 Max
	{
		get 
		{
			return new Index2(MAX_COLUMN-1,MAX_COLUMN-1);
		}
	}

	static public Index2 None
	{
		get
		{
			return new Index2(MAX_COLUMN + 100, MAX_COLUMN + 100);
		}
	}

	static public Index2 Up
	{
		get
		{
			return new Index2(0, 1);
		}
	}

	static public Index2 Down
	{
		get
		{
			return new Index2(0, -1);
		}
	}

	static public Index2 Left
	{
		get
		{
			return new Index2(-1, 0);
		}
	}

	static public Index2 Right
	{
		get
		{
			return new Index2(1, 0);
		}
	}
	static public Index2 RightUp
	{
		get
		{
			return new Index2(1, 1);
		}
	}
	static public Index2 LeftUp
	{
		get
		{
			return new Index2(-1, 1);
		}
	}

	public Index2 X_AxisSeparation
	{
		get
		{
			return new Index2(this.ix, 0);
		}
	}

	public Index2 Y_AxisSeparation
	{
		get
		{
			return new Index2(0, this.iy);
		}
	}

	public int this [int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return this.ix;
			case 1:
				return this.iy;
			
			default:
				throw new IndexOutOfRangeException ("Invalid Index2 index!");
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				this.ix = value;
				break;
			case 1:
				this.iy = value;
				break;
			
			default:
				throw new IndexOutOfRangeException ("Invalid Index2 index!");
			}
		}
	}

	public Index2(int ix, int iy)
	{
		this.ix = ix;
		this.iy = iy;
	}

	public int LengthSquared()
	{
		return this.ix * this.ix + this.iy * this.iy;
	}


	static bool ValidMaxColumn(int maxColumn)
	{
		if (MIN_COLUMN <= maxColumn && maxColumn <= MAX_COLUMN) 
		{
			return true;
		}

		return false;
	}

	static public Index2 Vector3ToIndex2(Vector3 vt3 , float gridSize_x, float gridSize_y)
	{
		Index2 result;

		result.ix = (int)(vt3.x / gridSize_x);
		result.iy = (int)(vt3.y / gridSize_y);

		return result;
	}

	static public Index2 Index1ToIndex2(int index1 , int maxColumn)
	{
		if (false == ValidMaxColumn (maxColumn)) 
		{
			Debug.LogError("exception !! : invalid value that maxcolumn");
			return new Index2();
		}

		if (0 == maxColumn) 
		{
			Debug.LogError ("exception !! : divide by 0 ");
			return new Index2();
		}

		Index2 value;
		value.ix = index1 % maxColumn;
		value.iy = index1 / maxColumn;

		return value;
	}

	static public int Index2ToIndex1(Index2 ixyPair , int maxColumn)
	{
		return ixyPair.ix + ixyPair.iy * maxColumn;
	}

	static public bool operator != (Index2 ixy1 , Index2 ixy2)
	{
		if (ixy1.ix != ixy2.ix || ixy1.iy != ixy2.iy)
			return true;

		return false;
	}

	static public bool operator == (Index2 ixy1 , Index2 ixy2)
	{
		if (ixy1.ix == ixy2.ix && ixy1.iy == ixy2.iy)
			return true;

		return false;
	}

	public static Index2 operator - (Index2 a, int d)
	{
		return new Index2 (a.ix - d, a.iy - d);
	}

	public static Index2 operator - (Index2 a, Index2 b)
	{
		return new Index2 (a.ix - b.ix, a.iy - b.iy);
	}
	
	public static Index2 operator - (Index2 a)
	{
		return new Index2 (-a.ix, -a.iy);
	}

	public static Index2 operator + (Index2 a, int d)
	{
		return new Index2 (a.ix + d, a.iy + d);
	}

	public static Index2 operator + (Index2 a, Index2 b)
	{
		return new Index2 (a.ix + b.ix, a.iy + b.iy);
	}

	public static Index2 operator * (int d, Index2 a)
	{
		return new Index2 (a.ix * d, a.iy * d);
	}
	
	public static Index2 operator * (Index2 a, int d)
	{
		return new Index2 (a.ix * d, a.iy * d);
	}
	
	public override bool Equals (object other)
	{
		if (!(other is Index2))
		{
			return false;
		}
		Index2 ixy = (Index2)other;
		return Equals (ixy);
	}

	public bool Equals(Index2 ixy)
	{
		if (this.ix == ixy.ix && this.iy == ixy.iy)
			return true;

		return false;
	}

	override public int GetHashCode()
	{
		return ix ^ iy;
	}

	override public string ToString()
	{
		return ix + "," + iy;
	}

}


