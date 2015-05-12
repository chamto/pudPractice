using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace PuzzAndBidurgi
{
	//dungeon ---- section1 +  section2 + section3  ...


	//user
	public class LordCommander
	{
		private Dictionary<int,Monster> _info = new Dictionary<int, Monster> ();
	}

	public class Monster
	{
		int _hp;
		int _def;
		int _atk;

		int _turn;
	}

	public class Section
	{
		private Dictionary<int,Monster> _info = new Dictionary<int, Monster> ();

		//activity condition :  monster ai
	}

	public class Dungeon  
	{
		private Dictionary<int,Section> _info = new Dictionary<int, Section> ();


	}
}



