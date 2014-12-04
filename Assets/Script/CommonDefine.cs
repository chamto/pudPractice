 
// ------------------------------------------------------------------------------
// author : 20131216 chamto
//
// ------------------------------------------------------------------------------
using System;

namespace PuzzAndBidurgi
{

	/// <summary>
	/// Info of matrix in board 
	/// </summary>
	public struct PairInt
	{
		public int column;
		public int row;

		public PairInt(int _column , int _row)
		{
			this.column = _column;
			this.row = _row;
		}
	}

	public static class ConstBoard
	{
		public const uint Max_Column = 5;
		public const uint Max_Row = 6;
		public const uint Max_Count_5x6 = 5 * 6;
	}

	public static class ConstDrop
	{
		public const float UI_Width = 1.15f;
		public const float UI_Height = 1.15f;
	}

	public static class ConstRolePuzzleAndDragon
	{
		public const int DropMoveTime = 5; //second
	}


}

