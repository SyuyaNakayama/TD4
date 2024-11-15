using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateCharaDataLib")]
public class CharaDataLib : ScriptableObject
{
    public CharacterCassette[] dataLib;

    public CharacterCassette FindCharacterCassette(string searchName)
    {
        if (searchName != "")
        {
            for (int i = 0; i < dataLib.Length; i++)
            {
                if (dataLib[i] != null && dataLib[i].GetData().name == searchName)
                {
                    return dataLib[i];
                }
            }
        }
        return dataLib[0];
    }
}
