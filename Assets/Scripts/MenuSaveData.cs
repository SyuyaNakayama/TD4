using System;

public class MenuSaveData : object
{
    [Serializable]
    public struct NameAndValue
    {
        public string name;
        public float value;
    }

    public NameAndValue[] nameAndValues;
}