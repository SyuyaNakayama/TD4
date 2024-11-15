using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CMBMenuPiece : MonoBehaviour
{
    [SerializeField]
    Menu menu;
    [SerializeField]
    RectTransform rectTransform;
    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }
    [SerializeField]
    Image image;
    [SerializeField]
    Material material;
    [SerializeField]
    ControlMapMenu controlMap;
    public ControlMapMenu GetControlMap()
    {
        return controlMap;
    }
    [SerializeField]
    string controlMessage;
    public string GetControlMessage()
    {
        return controlMessage;
    }
    [SerializeField]
    TMP_Text tmp;
    public TMP_Text GetTMP()
    {
        return tmp;
    }
    [SerializeField]
    TMP_Text tmpE;
    public TMP_Text GetTMPE()
    {
        return tmpE;
    }
    [SerializeField]
    string text;
    public string GetText()
    {
        return text;
    }
    [SerializeField]
    string textE;
    public string GetTextE()
    {
        return textE;
    }
    [SerializeField]
    Color color;
    public Color GetColor()
    {
        return color;
    }

    protected bool output;
    public bool GetOutput()
    {
        return output;
    }

    //見た目の更新処理しか扱わないのでFixedUpdateじゃなくて良い
    void Update()
    {
        tmp.text = text;
        tmpE.text = textE;
    }

    void FixedUpdate()
    {
        output = false;
        if (!menu || menu.active)
        {
            image.material = new Material(material);
            image.material.SetColor("_AddColor", GetColor());

            MPUpdate();
        }
    }

    protected virtual void MPUpdate()
    {
    }

    public virtual float GetOutputValue()
    {
        return 0;
    }

    public void Load(SaveData.NameAndFloat[] nameAndFloats)
    {
        if (nameAndFloats != null)
        {
            //自身の設定項目が記された箇所を探す
            for (int i = 0; i < nameAndFloats.Length; i++)
            {
                if (nameAndFloats[i].name == controlMessage)
                {
                    //見つけた箇所に従って値を調整
                    MPLoad(nameAndFloats[i]);
                    return;
                }
            }
        }
    }

    protected virtual void MPLoad(SaveData.NameAndFloat nameAndFloat)
    {

    }
}