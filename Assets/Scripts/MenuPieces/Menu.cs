using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [System.Serializable]
    struct MenuTransitionData
    {
        public string controlMessage;
        public Menu nextMenu;
    }

    public bool active;
    bool prevActive;
    bool prevIsCurrentMenu;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    string menuID;
    [SerializeField]
    Material windowMaterial;
    [SerializeField]
    Color windowColor;
    [SerializeField]
    Image window;
    [SerializeField]
    CMBMenuPiece[] menuPieces;
    public CMBMenuPiece[] GetMemuPieces()
    {
        return KX_netUtil.CopyArray<CMBMenuPiece>(menuPieces);
    }
    [SerializeField]
    MenuTransitionData[] menuTransitionDatas;
    [SerializeField]
    string closeMessage;

    string controlMessage;
    public string GetControlMessage()
    {
        return controlMessage;
    }

    void Awake()
    {
        Load();
    }

    void FixedUpdate()
    {
        //ウインドウの色を変える
        window.material = new Material(windowMaterial);
        window.material.SetColor("_AddColor", windowColor);

        //更新前初期化
        controlMessage = "";
        //アクティブなら表示する
        canvas.enabled = IsCurrentMenu();

        //押されたボタンに応じてメッセージを変える
        if (IsCurrentMenu())
        {
            if (!prevIsCurrentMenu)
            {
                Load();
            }
            else
            {
                Save();
            }

            for (int i = 0; i < menuPieces.Length; i++)
            {
                if (menuPieces[i].GetOutput())
                {
                    controlMessage = menuPieces[i].GetControlMessage();
                    break;
                }
            }
        }
        prevIsCurrentMenu = IsCurrentMenu();

        //メッセージに応じて次のメニューを表示
        if (controlMessage != "")
        {
            for (int i = 0; i < menuTransitionDatas.Length; i++)
            {
                if (controlMessage == menuTransitionDatas[i].controlMessage)
                {
                    menuTransitionDatas[i].nextMenu.active = true;
                    break;
                }
            }
        }

        //メッセージがcloseMessageと同じなら閉じる
        if (controlMessage == closeMessage)
        {
            active = false;
        }

        prevActive = active;
    }

    public bool IsCurrentMenu()
    {
        for (int i = 0; i < menuTransitionDatas.Length; i++)
        {
            if (menuTransitionDatas[i].nextMenu.active
                || menuTransitionDatas[i].nextMenu.prevActive)
            {
                return false;
            }
        }
        return active && prevActive;
    }

    void Save()
    {
        SaveData data = new SaveData();

        Array.Resize(ref data.nameAndFloats, menuPieces.Length);
        for (int i = 0; i < menuPieces.Length; i++)
        {
            SaveData.NameAndFloat nameAndFloat = new SaveData.NameAndFloat();
            nameAndFloat.name = menuPieces[i].GetControlMessage();
            nameAndFloat.value = menuPieces[i].GetOutputValue();
            data.nameAndFloats[i] = nameAndFloat;
        }

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/menusavedata_" + menuID + ".json", json);
    }

    void Load()
    {
        string path = Application.persistentDataPath + "/menusavedata_" + menuID + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            for (int i = 0; i < menuPieces.Length; i++)
            {
                menuPieces[i].Load(data.nameAndFloats);
            }
        }
    }
}