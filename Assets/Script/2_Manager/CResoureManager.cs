using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PuzzAndBidurgi
{
	/// <summary>
	/// Enum texture kind.
	/// </summary>
	public enum eResKind
	{
		None = -1,

		//기본 드랍 5종
		TextureDropRed = 0,
		TextureDropGreen,
		TextureDropBlue,
		TextureDropLight,
		TextureDropDart,

		//회복드랍
		TextureDropHeart,

		//방해드롭
		TextureDropObstruction,
		TextureDropPosion,

		Max,

		Red = TextureDropRed,
		Green = TextureDropGreen,
		Blue = TextureDropBlue,
		Light = TextureDropLight,
		Dart = TextureDropDart,
		Heart = TextureDropHeart,
	}

	/// <summary>
	/// Struct resource infomation.
	/// </summary>
	public struct SResInfo
	{
		public string strPath;
		public string strFileName;

		public SResInfo(string in_strPath , string in_strFileName)
		{
			strPath = in_strPath;
			strFileName = in_strFileName;
		}

		public string GetFullPath()
		{
			return string.Format("{0}/{1}",strPath,strFileName);
			//return String.Format("{0}/{1}",strPath,strFileName);
		}
	}

	public struct SResDefine
	{
		//pf : prefab resource
		public const string pfDROPINFO = "dropInfo";
		//sp : sprite resource
		public const ushort spINDEX_ZERO = 0;
	}


	/// <summary>
	/// Manage resources
	/// </summary>
	public class CResoureManager
	{
		//==============: member enum :==============
		public enum eError : ushort
		{
			Success = 0,
			Fail = 1,
		}

		//==============: member variables :==============
		Dictionary<ushort, SResInfo> 	m_dictTextureInfo = new Dictionary<ushort, SResInfo>();

		private int m_sequenceId = 0;

		/// <summary>
		/// Init Resources
		/// </summary>
		public ushort Init()
		{

			//Init TextureInfo 
			m_dictTextureInfo.Add((ushort)eResKind.TextureDropRed , new SResInfo("Texture/drop","dropRed"));
			m_dictTextureInfo.Add((ushort)eResKind.TextureDropGreen , new SResInfo("Texture/drop","dropGreen"));
			m_dictTextureInfo.Add((ushort)eResKind.TextureDropBlue , new SResInfo("Texture/drop","dropBlue"));
			m_dictTextureInfo.Add((ushort)eResKind.TextureDropLight , new SResInfo("Texture/drop","dropLight"));
			m_dictTextureInfo.Add((ushort)eResKind.TextureDropDart , new SResInfo("Texture/drop","dropDark"));
			m_dictTextureInfo.Add((ushort)eResKind.TextureDropHeart , new SResInfo("Texture/drop","dropHeart"));


			return (ushort)eError.Success;
		}


		public string GetFileName(eResKind eKey)
		{
			SResInfo info;
			m_dictTextureInfo.TryGetValue((ushort)eKey,out info);
			return info.strFileName;
		}

		public bool TryGetTextureInfo(eResKind in_eKey , out SResInfo out_info)
		{
			return m_dictTextureInfo.TryGetValue((ushort)in_eKey,out out_info);
		}

		public Sprite LoadSprites(eResKind in_eKey , ushort index)
		{
			SResInfo info;
			if(m_dictTextureInfo.TryGetValue((ushort)in_eKey,out info) )
			{
				//CDefine.DebugLog(info.GetFullPath() + "------------"); //chamto test
				//return Resources.Load<Sprite>(info.GetFullPath());

				Sprite[] sp = Resources.LoadAll<Sprite>(info.GetFullPath());
				if(0 < sp.Length && index <= sp.Length) return sp[index];

			}

			CDefine.DebugLogWarning("Failed LoadSprite");
			return null;
		}



		//==============: static function :==============

		static public GameObject CreatePrefab(string path)
		{
			const string root = "Prefab/";
			return MonoBehaviour.Instantiate(Resources.Load(root + path)) as GameObject;
		}

		//==============: get,set function :==============
		public int GetSequenceId()
		{
			return ++m_sequenceId;
		}

	}


}
