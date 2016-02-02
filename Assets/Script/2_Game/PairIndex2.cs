
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzAndBidurgi;


public struct PairIndex2
{
	public Index2 origin;
	public Index2 direction;

	public PairIndex2(Index2 _origin, Index2 _direction)
	{
		origin = _origin;
		direction = _direction;
	}
}
