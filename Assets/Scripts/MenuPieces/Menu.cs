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
    [SerializeField]
    ControlMapMenu controlMap;
    public ControlMapMenu GetControlMap()
    {
        return controlMap;
    }
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
    MenuTransitionData[] menuTransitionDatas;
    [SerializeField]
    string closeMessage;

    CMBMenuPiece[] menuPieces = { };
    public CMBMenuPiece[] GetMemuPieces()
    {
        return KX_netUtil.CopyArray<CMBMenuPiece>(menuPieces);
    }
    string controlMessage;
    public string GetControlMessage()
    {
        return controlMessage;
    }
    bool prevActive;
    bool prevIsCurrentMenu;
    public bool GetPrevIsCurrentMenu()
    {
        return prevIsCurrentMenu;
    }

    void Awake()
    {
        //����MenuPiece�������ŒT��
        Array.Resize(ref menuPieces, 0);
        for (int i = 0; i < transform.childCount; i++)
        {
            CMBMenuPiece current =
                transform.GetChild(i).GetComponent<CMBMenuPiece>();

            if (current)
            {
                Array.Resize(ref menuPieces, menuPieces.Length + 1);
                menuPieces[menuPieces.Length - 1] = current;
            }
        }

        Load();
    }

    void FixedUpdate()
    {
        //�E�C���h�E�̐F��ς���
        window.material = new Material(windowMaterial);
        window.material.SetColor("_AddColor", windowColor);

        //�X�V�O������
        controlMessage = "";
        //�A�N�e�B�u�Ȃ�\������
        canvas.enabled = IsCurrentMenu();

        //����MenuPiece�������ŒT��
        Array.Resize(ref menuPieces, 0);
        for (int i = 0; i < transform.childCount; i++)
        {
            CMBMenuPiece current =
                transform.GetChild(i).GetComponent<CMBMenuPiece>();

            if (current)
            {
                Array.Resize(ref menuPieces, menuPieces.Length + 1);
                menuPieces[menuPieces.Length - 1] = current;
            }
        }

        //�����ꂽ�{�^���ɉ����ă��b�Z�[�W��ς���
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

            MenuUpdate();

            for (int i = 0; i < menuPieces.Length; i++)
            {
                if (menuPieces[i].GetOutput())
                {
                    controlMessage = menuPieces[i].GetControlMessage();
                    break;
                }
            }

            if (prevIsCurrentMenu)
            {
                //���b�Z�[�W�ɉ����Ď��̃��j���[��\���@
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

                //���b�Z�[�W��closeMessage�Ɠ����Ȃ����
                if (controlMessage == closeMessage)
                {
                    active = false;
                }
            }
        }
        prevIsCurrentMenu = IsCurrentMenu();
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

    protected virtual void MenuUpdate()
    {
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