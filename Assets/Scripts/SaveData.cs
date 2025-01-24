using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SaveData : object
{
	[Serializable]
	public struct NameAndInt
	{
		public string name;
		public int value;
	}
	[Serializable]
	public struct NameAndIntArray
	{
		public string name;
		public int[] values;
	}

	[Serializable]
	public struct NameAndFloat
	{
		public string name;
		public float value;
	}
	[Serializable]
	public struct NameAndFloatArray
	{
		public string name;
		public float[] values;
	}

	[Serializable]
	public struct NameAndString
	{
		public string name;
		public string value;
	}
	[Serializable]
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
