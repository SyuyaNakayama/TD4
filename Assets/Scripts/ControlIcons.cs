using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateControlIcons")]
public class ControlIcons : ScriptableObject
{
    [Serializable]
    public struct ButtonIcon
    {
        public string name;
        public string label;
        public Sprite nutral;
        public Sprite pressed;
    }
    [Serializable]
    public struct DirectionIcon
    {
        public string name;
        public string label;
        public Sprite nutral;
        public Sprite up;
        public Sprite upRight;
        public Sprite right;
        public Sprite downRight;
        public Sprite down;
        public Sprite downLeft;
        public Sprite left;
        public Sprite upLeft;
    }

    [SerializeField]
    ButtonIcon[] buttonIcons = { };
    public ButtonIcon SearchButtonIcon(string name)
    {
        for (int i = 0; i < buttonIcons.Length; i++)
        {
            if (name == buttonIcons[i].name)
            {
                return buttonIcons[i];
            }
        }
        return new ButtonIcon();
    }
    [SerializeField]
    DirectionIcon[] directionIcons = { };
    public DirectionIcon SearchDirectionIcon(string name)
    {
        for (int i = 0; i < directionIcons.Length; i++)
        {
            if (name == directionIcons[i].name)
            {
                return directionIcons[i];
            }
        }
        return new DirectionIcon();
    }
}