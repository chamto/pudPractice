
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;


[System.Serializable]
public struct Index3
{

	static int MAX_COLUMN = 46340;
	static int MIN_COLUMN = -46341;

	public int ix;
	public int iy;
	public int iz;


	static public Index3 Zero
	{
		get 
		{
			return new Index3(0,0,0);
		}
	}
	
	static public Index3 Max
	{
		get 
		{
			return new Index3(MAX_COLUMN-1, MAX_COLUMN-1, MAX_COLUMN-1);
		}
	}
	
	static public Index3 None
	{
		get
		{
			return new Index3(MAX_COLUMN + 100, MAX_COLUMN + 100, MAX_COLUMN + 100);
		}
	}
	
	static public Index3 Up
	{
		get
		{
			return new Index3(0, 1, 0);
		}
	}
	
	static public Index3 Down
	{
		get
		{
			return new Index3(0, -1, 0);
		}
	}
	
	static public Index3 Left
	{
		get
		{
			return new Index3(-1, 0, 0);
		}
	}
	
	static public Index3 Right
	{
		get
		{
			return new Index3(1, 0, 0);
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
			case 2:
				return this.iz;
				
			default:
				throw new IndexOutOfRangeException ("Invalid Index3 index!");
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
			case 2:
				this.iz = value;
				break;
				
			default:
				throw new IndexOutOfRangeException ("Invalid Index3 index!");
			}
		}
	}
	
	public Index3(int ix, int iy, int iz)
	{
		this.ix = ix;
		this.iy = iy;
		this.iz = iz;
	}
	
	static bool ValidMaxColumn(int maxColumn)
	{
		if (MIN_COLUMN <= maxColumn && maxColumn <= MAX_COLUMN) 
		{
			return true;
		}
		
		return false;
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
	
	static public bool operator != (Index3 ixy1 , Index3 ixy2)
	{
		if (ixy1.ix != ixy2.ix || ixy1.iy != ixy2.iy || ixy1.iz != ixy2.iz)
			return true;
		
		return false;
	}
	
	static public bool operator == (Index3 ixy1 , Index3 ixy2)
	{
		if (ixy1.ix == ixy2.ix && ixy1.iy == ixy2.iy && ixy1.iz == ixy2.iz)
			return true;
		
		return false;
	}


	public static  implicit operator Index2(Index3 a)
	{
		return new Index2 (a.ix , a.iy );
	}
	public static  implicit operator Index3(Index2 a)
	{
		return new Index3 (a.ix , a.iy , 0);
	}

	
	public override bool Equals (object other)
	{
		if (!(other is Index3))
		{
			return false;
		}
		Index3 ixy = (Index3)other;
		return Equals (ixy);
	}
	
	public bool Equals(Index3 ixy)
	{
		if (this.ix == ixy.ix && this.iy == ixy.iy && this.iz == ixy.iz)
			return true;
		
		return false;
	}
	
	override public int GetHashCode()
	{
		return ix ^ iy ^ iz;
	}
	
	override public string ToString()
	{
		return ix + "," + iy + "," + iz;
	}
}
