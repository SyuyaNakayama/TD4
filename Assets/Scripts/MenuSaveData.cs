using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSaveData : object
{
    [System.Serializable]
    public struct NameAndValue
    {
        public string name;
        public float value;
    }

    public NameAndValue[] nameAndValues;
}
