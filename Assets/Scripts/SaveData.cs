using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SaveData : object
{
	[System.Serializable]
	public struct NameAndInt
	{
		public string name;
		public int value;
	}
	[System.Serializable]
	public struct NameAndIntArray
	{
		public string name;
		public int[] values;
	}

	[System.Serializable]
	public struct NameAndFloat
	{
		public string name;
		public float value;
	}
	[System.Serializable]
	public struct NameAndFloatArray
	{
		public string name;
		public float[] values;
	}

	[System.Serializable]
	public struct NameAndString
	{
		public string name;
		public string value;
	}
	[System.Serializable]
	public struct NameAndStringArray
	{
		public string name;
		public string[] values;
	}

	public NameAndInt[] nameAndInts;
	public NameAndIntArray[] nameAndIntArrays;
	public NameAndFloat[] nameAndFloats;
	public NameAndFloatArray[] nameAndFloatArrays;
	public NameAndString[] nameAndStrings;
	public NameAndStringArray[] nameAndStringArrays;

	public float coins;
	public int cassetteNum;
	public float[] life = { };
	public string[] inventoryCharaID = { };
	public int[] teamCharaIndex = { };
	public int[] squadCassetteNum = { };
}
