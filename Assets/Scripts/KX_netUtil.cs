using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KX_netUtil : object
{
    [Serializable]
    public enum EaseType
    {
        easeIn,
        easeOut,
        easeInOut
    }
    [Serializable]
    public enum XInputButton
    {
        north,
        south,
        west,
        east,
        l,
        r,
        triggerL,
        triggerR,
        dpadUp,
        dpadDown,
        dpadRight,
        dpadLeft,
        stickL,
        stickR,
        start,
        select,
    }
    [Serializable]
    public enum XInputAxis
    {
        stickLX,
        stickLY,
        stickRX,
        stickRY,
    }

    [Serializable]
    public struct EaseData
    {
        public EaseType type;
        public float pow;
    }
    [Serializable]
    public struct TransformData
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
    }
    [Serializable]
    public struct AxisSwitch
    {
        public bool x;
        public bool y;
        public bool z;
    }
    [Serializable]
    public struct TransformSwitch
    {
        public bool position;
        public bool rotation;
        public bool scale;
    }

    [Serializable]
    public struct TextureSetter
    {
        public string propertyName;
        public Texture2D texture;
    }
    [Serializable]
    public struct ColorSetter
    {
        public string propertyName;
        public Color color;
    }
    [Serializable]
    public struct IntSetter
    {
        public string propertyName;
        public int value;
    }
    [Serializable]
    public struct FloatSetter
    {
        public string propertyName;
        public float value;
    }
    [Serializable]
    public struct PropertySetter
    {
        public TextureSetter[] textureSetters;
        public ColorSetter[] colorSetters;
        public IntSetter[] intSetters;
        public FloatSetter[] floatSetters;

        public PropertySetter(PropertySetter propertySetter)
        {
            textureSetters = CopyArray(
                propertySetter.textureSetters);
            colorSetters = CopyArray(
                propertySetter.colorSetters);
            intSetters = CopyArray(
                propertySetter.intSetters);
            floatSetters = CopyArray(
                propertySetter.floatSetters);
        }
    }

    [Serializable]
    public struct TextureReplacer
    {
        public int targetIndex;
        public Texture2D texture;
    }
    [Serializable]
    public struct ColorReplacer
    {
        public int targetIndex;
        public Color color;
    }
    [Serializable]
    public struct IntReplacer
    {
        public int targetIndex;
        public int value;
    }
    [Serializable]
    public struct FloatReplacer
    {
        public int targetIndex;
        public float value;
    }
    [Serializable]
    public struct PropertyReplacer
    {
        public TextureReplacer[] textureReplacers;
        public ColorReplacer[] colorReplacers;
        public IntReplacer[] intReplacers;
        public FloatReplacer[] floatReplacers;

        public PropertyReplacer(PropertyReplacer propertyReplacer)
        {
            textureReplacers = CopyArray(
                propertyReplacer.textureReplacers);
            colorReplacers = CopyArray(
                propertyReplacer.colorReplacers);
            intReplacers = CopyArray(
                propertyReplacer.intReplacers);
            floatReplacers = CopyArray(
                propertyReplacer.floatReplacers);
        }
    }

    //HP�Q�[�W�Ȃǂ̔w�i�F���Z�o
    public static Color GaugeBlankColor(Color gaugeColor, bool forceHighContrastWhenWhite = false, bool forceHighContrastWhenBlack = false)
    {
        float colorMax = gaugeColor.r;
        float colorMin = gaugeColor.r;
        if (gaugeColor.g > colorMax)
        {
            colorMax = gaugeColor.g;
        }
        if (gaugeColor.g < colorMin)
        {
            colorMin = gaugeColor.g;
        }

        if (gaugeColor.b > colorMax)
        {
            colorMax = gaugeColor.b;
        }
        if (gaugeColor.b < colorMin)
        {
            colorMin = gaugeColor.b;
        }
        if (gaugeColor.r + gaugeColor.g + gaugeColor.b >= 1.5f
        || (gaugeColor.r + gaugeColor.b < gaugeColor.g * 0.5f && gaugeColor.g > 0.6f))
        {
            if (colorMax - colorMin < 0.8f || forceHighContrastWhenBlack)
            {
                return new Color(0, 0, 0, 1);
            }
            else
            {
                return new Color(0.3f, 0.3f, 0.3f, 1);
            }
        }
        if (colorMax - colorMin < 0.8f || forceHighContrastWhenWhite)
        {
            return new Color(1, 1, 1, 1);
        }
        else
        {
            return new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }
    //HP�Q�[�W�Ȃǂ̌������̐F���Z�o
    public static Color DamageGaugeColor(Color gaugeColor)
    {
        Color ret = new Color(1, 0, 0, 1);
        float colorMax = gaugeColor.r;
        float colorMin = gaugeColor.r;
        if (gaugeColor.g > colorMax)
        {
            colorMax = gaugeColor.g;
        }
        if (gaugeColor.g < colorMin)
        {
            colorMin = gaugeColor.g;
        }

        if (gaugeColor.b > colorMax)
        {
            colorMax = gaugeColor.b;
        }
        if (gaugeColor.b < colorMin)
        {
            colorMin = gaugeColor.b;
        }
        if (colorMax - colorMin > 0.8f
        && gaugeColor.r / gaugeColor.b >= 0.7f
        && gaugeColor.r / gaugeColor.g >= 1.4f)
        {
            if (gaugeColor.g / gaugeColor.b >= 1f
            && gaugeColor.g / gaugeColor.r >= 0.5f)
            {
                ret = new Color(0.5f, 0, 1, 1);
            }
            else
            {
                ret = new Color(1, 0.7f, 0, 1);
            }
        }
        return ret;
    }
    //����g�����X�t�H�[�����猩�����̃g�����X�t�H�[���̑��΍��W
    public static Vector3 RelativePosition(Transform observer, Transform target, Vector3 translation)
    {
        return observer.InverseTransformPoint(target.TransformPoint(translation));
    }
    //�J�������猩���g�����X�t�H�[���̃X�N���[�����W
    public static Vector3 RenderPosition(Camera cam, Transform target, Vector3 translation)
    {
        return cam.WorldToScreenPoint(target.TransformPoint(translation));
    }
    //�X�N���[�����W�𐳋K�X�N���[�����W��
    public static Vector3 NormalizeScreenPoint(Vector3 point)
    {
        return new Vector3((point.x / Screen.width - 0.5f) * 2, (point.y / Screen.height - 0.5f) * 2, point.z);
    }
    //���K�X�N���[�����W���X�N���[�����W��
    public static Vector3 InverseNormalizeScreenPoint(Vector3 point)
    {
        return new Vector3((point.x / 2 + 0.5f) * Screen.width, (point.y / 2 + 0.5f) * Screen.height, point.z);
    }
    //rectTransform���rect���X�N���[�����W�ɕϊ�
    public static Rect GetScreenRect(RectTransform rtf, Camera camera)
    {
        Vector3[] corners = new Vector3[4];

        rtf.GetWorldCorners(corners);
        if (camera != null)
        {
            corners[0] = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
            corners[2] = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
        }

        Rect ret = new Rect
        {
            x = corners[0].x,
            y = corners[0].y
        };
        ret.width = corners[2].x - ret.x;
        ret.height = corners[2].y - ret.y;
        return ret;
    }

    public static Vector3 GetRectCenterPosition(RectTransform rect)
    {
        Vector3 ret = rect.transform.position;

        float scaleX = rect.transform.lossyScale.x;
        float scaleY = rect.transform.lossyScale.y;
        float x = rect.rect.width / 2f * scaleX;
        float y = rect.rect.height / 2f * scaleY;
        Vector3 retPlus = rect.transform.rotation * new Vector3(
            Mathf.Lerp(x, -x, rect.pivot.x),
            Mathf.Lerp(y, -y, rect.pivot.y), 0);

        return ret + retPlus;
    }

    public static Vector3 GetRectRelativePosition(RectTransform rect, Vector2 pos)
    {
        Vector3 ret = rect.transform.position;

        float scaleX = rect.transform.lossyScale.x;
        float scaleY = rect.transform.lossyScale.y;
        float x = rect.rect.width / 2f * scaleX;
        float y = rect.rect.height / 2f * scaleY;
        Vector3 retPlus = rect.transform.rotation * new Vector3(
            Mathf.Lerp(x, -x, rect.pivot.x + pos.x),
            Mathf.Lerp(y, -y, rect.pivot.y + pos.y), 0);

        return ret + retPlus;
    }
    //rectTransform��̍��W�����[���h���W�ɕϊ�
    public static Vector3 RectToWorldPoint(RectTransform rtf, Camera camera)
    {
        Vector3 ret = Vector3.zero;

        //UI���W����X�N���[�����W�ɕϊ�
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            camera, rtf.position);
        //�X�N���[�����W�����[���h���W�ɕϊ�
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rtf, screenPos, camera, out ret);

        return ret;
    }
    //2D���W��̓_����`���ɂ��邩
    public static bool IsInRect(Vector2 corner1, Vector2 corner2, Vector2 point)
    {
        if (corner1.x > corner2.x)
        {
            float swap = corner1.x;
            corner1.x = corner2.x;
            corner2.x = swap;
        }

        if (corner1.y > corner2.y)
        {
            float swap = corner1.y;
            corner1.y = corner2.y;
            corner2.y = swap;
        }

        return point.x == Mathf.Clamp(point.x, corner1.x, corner2.x) && point.y == Mathf.Clamp(point.y, corner1.y, corner2.y);
    }

    //�w�肳�ꂽ2D���W�����̋�`�ɐG��Ă��邩
    public static bool IsInsideHitBox(RectTransform rectTransform, Camera camera, Vector2 inputPosition2D)
    {
        Rect rect =
            GetScreenRect(rectTransform, camera);
        Vector2 corner1 =
            new Vector2(rect.xMin, rect.yMin);
        Vector2 corner2 =
            new Vector2(rect.xMax, rect.yMax);

        return IsInRect(
            corner1, corner2, inputPosition2D);
    }
    //�w�肳�ꂽ2D���W�����̋�`���猩�Ăǂ��ɂ��邩
    public static Vector2 InverseTransformPointHitBox(RectTransform rectTransform, Camera camera, Vector2 inputPosition2D)
    {
        Rect rect =
            GetScreenRect(rectTransform, camera);

        return new Vector2(
            RangeMap(inputPosition2D.x, rect.xMin, rect.xMax, -0.5f, 0.5f),
            RangeMap(inputPosition2D.y, rect.yMin, rect.yMax, -0.5f, 0.5f));
    }
    //��̃R���C�_�[���ڐG���Ă��邩�i��܂��ɂ�������ł��Ȃ�����ߐM���Ȃ��łˁj
    public static bool IsHit(Collider col1, Collider col2, float fractionDistance)
    {
        return Vector3.Distance(col1.ClosestPoint(col2.ClosestPoint(col1.gameObject.transform.position)), col2.ClosestPoint(col1.ClosestPoint(col2.ClosestPoint(col1.gameObject.transform.position)))) <= fractionDistance
        || Vector3.Distance(col2.ClosestPoint(col1.ClosestPoint(col2.gameObject.transform.position)), col1.ClosestPoint(col2.ClosestPoint(col1.ClosestPoint(col2.gameObject.transform.position)))) <= fractionDistance;
    }
    //���[���h��ԏ�̓_���R���C�_�[�̒��ɂ��邩
    public static bool IsInsidePosition(Collider col, Vector3 point)
    {
        return col.ClosestPoint(point) == point;
    }
    //�l���͈͓��ɂ��邩
    public static bool IsIntoRange(float value, float rangeHead, float rangeTail,
        bool notEqualHead, bool notEqualTail)
    {
        if ((notEqualHead && value == rangeHead)
            || (notEqualTail && value == rangeTail))
        {
            return false;
        }
        return value == Mathf.Clamp(value, rangeHead, rangeTail);
    }
    //��̐��l�͈͂��d�Ȃ��Ă��邩
    public static bool IsCrossingRange(float range1Head, float range1Tail,
        float range2Head, float range2Tail,
        bool notEqualHead, bool notEqualTail)
    {
        if (range1Head == range1Tail)
        {
            return IsIntoRange(range1Head, range2Head, range2Tail,
                notEqualHead, notEqualTail);
        }
        if (range2Head == range2Tail)
        {
            return IsIntoRange(range2Head, range1Head, range1Tail,
                notEqualHead, notEqualTail);
        }

        return IsIntoRange(range1Head, range2Head, range2Tail,
            false, notEqualTail)
            || IsIntoRange(range1Tail, range2Head, range2Tail,
            notEqualHead, false)
            || IsIntoRange(range2Head, range1Head, range1Tail,
            false, notEqualTail)
            || IsIntoRange(range2Tail, range1Head, range1Tail,
            notEqualHead, false);
    }
    //��̊p�x�i�x���@�j�̍ŏ���
    public static float AngleDiff(float degree1, float degree2)
    {
        float angle1 = degree2 - degree1;
        float angle2 = degree2 + 360 - degree1;
        float angle3 = degree2 - 360 - degree1;
        if (Mathf.Abs(angle1) < Mathf.Abs(angle2) && Mathf.Abs(angle1) < Mathf.Abs(angle3))
        {
            return angle1;
        }
        else
        if (Mathf.Abs(angle2) < Mathf.Abs(angle1) && Mathf.Abs(angle2) < Mathf.Abs(angle3))
        {
            return angle2;
        }
        return angle3;
    }
    //Vector2�����̂܂ܓ������Atan2�֐�
    public static float Atan2(Vector2 vec)
    {
        return Mathf.Atan2(vec.x, vec.y);
    }
    //������������͈̔͂ɕ������A���̊p�x���ǂ̃Z�O�����g�͈͓̔��ɂ��邩
    public static int RadToSegmentIndex(float rad, int divNum)
    {
        int ret = Mathf.RoundToInt(rad / (Mathf.PI * 2) * divNum);
        if (ret < 0)
        {
            ret = ret + divNum;
        }
        return ret;
    }
    //���l��0~1�͈͓̔��Ɏ��߂�
    public static float Saturate(float value)
    {
        return Mathf.Clamp(value, 0, 1);
    }
    //���l�͈̔̓}�b�s���O
    public static float RangeMap(
        float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return outputMin + (value - inputMin)
            * (outputMax - outputMin) / (inputMax - inputMin);
    }
    //���̒l�ɂ��Ή������ݏ�֐�
    public static float SignedPow(float value, float pow)
    {
        int sign = Mathf.RoundToInt(Mathf.Sign(value));
        return Mathf.Pow(Mathf.Abs(value), pow) * sign;
    }
    //�O�p�g
    public static float TriWave(float time)
    {
        time /= 2;
        time = time % Mathf.PI * 4;
        if (time < Mathf.PI)
        {
            return time / (Mathf.PI);
        }
        else if (time < Mathf.PI * 3)
        {
            return (Mathf.PI * 2 - time) / (Mathf.PI);
        }
        else
        {
            return (time - Mathf.PI * 4) / (Mathf.PI);
        }
    }
    //�ό`�\�ȗ]���g�i�g���@��͏��Ȃ������j
    public static float StreachedCos(
        float num, float mul, float seg1Pow = 1, float seg2Pow = 1)
    {
        num = Mathf.Repeat(num, Mathf.PI * 2);
        num *= (0.5f + mul / 2);
        if (num > Mathf.PI)
        {
            num = Mathf.PI + (num - Mathf.PI) / mul;
        }

        float ret = Mathf.Cos(num);

        if (num <= Mathf.PI)
        {
            ret = SignedPow(ret, seg1Pow);
        }
        else
        {
            ret = SignedPow(ret, seg2Pow);
        }

        return ret;
    }
    //�C�[�W���O
    public static float Ease(float progress, EaseType easeType, float powNum)
    {
        float ret = 0;
        float currentProgress = Mathf.Clamp(progress, 0, 1);

        switch (easeType)
        {
            case EaseType.easeIn:
                ret = Mathf.Pow(currentProgress, powNum);
                break;
            case EaseType.easeOut:
                ret = 1 - Mathf.Pow(1 - currentProgress, powNum);
                break;
            case EaseType.easeInOut:
                if (currentProgress < 0.5f)
                {
                    ret = Mathf.Pow(currentProgress * 2, powNum) / 2;
                }
                else
                {
                    ret = 1 - Mathf.Pow((1 - currentProgress) * 2, powNum) / 2;
                }
                break;
        }

        return ret;
    }
    //PropertyReplacer��p����PropertySetter��ҏW���ĕԂ�
    public static PropertySetter ReplacePropertySetter(
        PropertySetter propertySetter, PropertyReplacer propertyReplacer)
    {
        PropertySetter ret = propertySetter;

        for (int i = 0; i < propertyReplacer.textureReplacers.Length; i++)
        {
            TextureReplacer current = propertyReplacer.textureReplacers[i];
            Array.Resize(ref ret.textureSetters,
                Mathf.Max(ret.textureSetters.Length, current.targetIndex));
            ret.textureSetters[current.targetIndex].texture =
                current.texture;
        }
        for (int i = 0; i < propertyReplacer.colorReplacers.Length; i++)
        {
            ColorReplacer current = propertyReplacer.colorReplacers[i];
            Array.Resize(ref ret.colorSetters,
                Mathf.Max(ret.colorSetters.Length, current.targetIndex));
            ret.colorSetters[current.targetIndex].color =
                current.color;
        }
        for (int i = 0; i < propertyReplacer.intReplacers.Length; i++)
        {
            IntReplacer current = propertyReplacer.intReplacers[i];
            Array.Resize(ref ret.intSetters,
                Mathf.Max(ret.intSetters.Length, current.targetIndex));
            ret.intSetters[current.targetIndex].value =
                current.value;
        }
        for (int i = 0; i < propertyReplacer.floatReplacers.Length; i++)
        {
            FloatReplacer current = propertyReplacer.floatReplacers[i];
            Array.Resize(ref ret.floatSetters,
                Mathf.Max(ret.floatSetters.Length, current.targetIndex));
            ret.floatSetters[current.targetIndex].value =
                current.value;
        }

        return ret;
    }
    //�}�e���A���̃v���p�e�B���ꊇ�ŕҏW
    public static void ApplyMaterialPropertySetter(
        Material material, PropertySetter propertySetter)
    {
        for (int i = 0; i < propertySetter.textureSetters.Length; i++)
        {
            TextureSetter current = propertySetter.textureSetters[i];
            material.SetTexture(current.propertyName, current.texture);
        }
        for (int i = 0; i < propertySetter.colorSetters.Length; i++)
        {
            ColorSetter current = propertySetter.colorSetters[i];
            material.SetColor(current.propertyName, current.color);
        }
        for (int i = 0; i < propertySetter.intSetters.Length; i++)
        {
            IntSetter current = propertySetter.intSetters[i];
            material.SetInteger(current.propertyName, current.value);
        }
        for (int i = 0; i < propertySetter.floatSetters.Length; i++)
        {
            FloatSetter current = propertySetter.floatSetters[i];
            material.SetFloat(current.propertyName, current.value);
        }
    }
    //�z����R�s�[
    public static T[] CopyArray<T>(T[] array)
    {
        T[] ret = new T[array.Length];
        Array.Copy(array, ret, array.Length);
        return ret;
    }
    //�C���f�b�N�X���z��̒����w���Ă��邩
    public static bool IsValidIndex<T>(int index, T[] array)
    {
        return index >= 0 && index < array.Length;
    }
    //InputSystem����L�[�������Ŏw�肵�ē��͂��擾����
    public static bool GetISKey(Key key)
    {
        return key != 0 && key != (Key)111
            && Keyboard.current[key].IsPressed();
    }
    //InputSystem���炢���ꂩ�̃L�[�������ꂽ�����擾
    public static bool ISAnyKey()
    {
        return Keyboard.current.anyKey.IsPressed();
    }
    //InputSystem����}�E�X�{�^���������Ŏw�肵�ē��͂��擾����
    public static bool GetISMouseButton(string buttonName)
    {
        return Mouse.current[buttonName].IsPressed();
    }
    //InputSystem����}�E�X�̍��W���擾
    public static Vector2 GetISMousePosition()
    {
        return Mouse.current.position.ReadValue();
    }
    //InputSystem����{�^���������Ŏw�肵�ē��͂��擾����
    public static bool GetISJoyButton(int joyStickIndex, string buttonName)
    {
        return Joystick.all[joyStickIndex][buttonName].IsPressed();
    }
    //InputSystem���炢���ꂩ�̃{�^���������ꂽ�����擾
    public static bool ISAnyJoyButton(int joyStickIndex)
    {
        Joystick joystick = Joystick.all[joyStickIndex];
        // ���삳�ꂽ�{�^���Ȃǂ̏����擾
        Vector2 dpadValue = joystick.hatswitch.ReadValue();

        return joystick.trigger.IsPressed()
            || dpadValue.magnitude > 0;
    }
    //InputSystem����{�^���������Ŏw�肵�ē��͂��擾����
    public static bool GetISPadButton(int gamepadIndex, XInputButton button)
    {
        if (IsValidIndex<Gamepad>(gamepadIndex, Gamepad.all.ToArray()))
        {
            Gamepad gamepad = Gamepad.all[gamepadIndex];

            Vector2 dpadValue = gamepad.dpad.ReadValue();
            float leftTriggerValue = gamepad.leftTrigger.ReadValue();
            float rightTriggerValue = gamepad.rightTrigger.ReadValue();
            //���ɂ������@��������܂ł��̂����ɂ���
            return (button == XInputButton.north && gamepad.buttonNorth.IsPressed())
                || (button == XInputButton.south && gamepad.buttonSouth.IsPressed())
                || (button == XInputButton.west && gamepad.buttonWest.IsPressed())
                || (button == XInputButton.east && gamepad.buttonEast.IsPressed())
                || (button == XInputButton.l && gamepad.leftShoulder.IsPressed())
                || (button == XInputButton.r && gamepad.rightShoulder.IsPressed())
                || (button == XInputButton.stickL && gamepad.leftStickButton.IsPressed())
                || (button == XInputButton.stickR && gamepad.rightStickButton.IsPressed())
                || (button == XInputButton.triggerL && leftTriggerValue > 0)
                || (button == XInputButton.triggerR && rightTriggerValue > 0)
                || (button == XInputButton.start && gamepad.startButton.IsPressed())
                || (button == XInputButton.select && gamepad.selectButton.IsPressed())
                || (button == XInputButton.dpadUp && dpadValue.y > 0)
                || (button == XInputButton.dpadDown && dpadValue.y < 0)
                || (button == XInputButton.dpadRight && dpadValue.x > 0)
                || (button == XInputButton.dpadLeft && dpadValue.x < 0);
        }
        return false;
    }
    //InputSystem���炢���ꂩ�̃{�^���������ꂽ�����擾
    public static bool ISAnyPadButton(int gamepadIndex)
    {
        if (IsValidIndex<Gamepad>(gamepadIndex, Gamepad.all.ToArray()))
        {
            Gamepad gamepad = Gamepad.all[gamepadIndex];

            Vector2 dpadValue = gamepad.dpad.ReadValue();
            float leftTriggerValue = gamepad.leftTrigger.ReadValue();
            float rightTriggerValue = gamepad.rightTrigger.ReadValue();
            //���ɂ������@��������܂ł��̂����ɂ���
            return gamepad.buttonNorth.IsPressed() || gamepad.buttonSouth.IsPressed()
                || gamepad.buttonWest.IsPressed() || gamepad.buttonEast.IsPressed()
                || gamepad.leftShoulder.IsPressed() || gamepad.rightShoulder.IsPressed()
                || gamepad.leftStickButton.IsPressed()
                || gamepad.rightStickButton.IsPressed()
                || leftTriggerValue > 0 || rightTriggerValue > 0
                || gamepad.startButton.IsPressed() || gamepad.selectButton.IsPressed()
                || dpadValue.magnitude > 0;
        }
        return false;
    }
    //InputSystem���炢���ꂩ�̃A�i���O�����͂��s�Ȃ��������擾
    public static bool ISAnyPadAxis(int gamepadIndex)
    {
        Gamepad gamepad = Gamepad.all[gamepadIndex];

        Vector2 leftStickValue = gamepad.leftStick.ReadValue();
        Vector2 rightStickValue = gamepad.rightStick.ReadValue();
        //���ɂ������@��������܂ł��̂����ɂ���
        return leftStickValue.magnitude > 0
            || rightStickValue.magnitude > 0;
    }
    //InputSystem���玲�������Ŏw�肵�ē��͂��擾����
    public static float GetISPadAxis(int gamepadIndex, XInputAxis axis)
    {
        float ret = 0;
        if (IsValidIndex<Gamepad>(gamepadIndex, Gamepad.all.ToArray()))
        {
            Gamepad gamepad = Gamepad.all[gamepadIndex];

            Vector2 leftStickValue = gamepad.leftStick.ReadValue();
            Vector2 rightStickValue = gamepad.rightStick.ReadValue();
            //���ɂ������@��������܂ł��̂����ɂ���
            switch (axis)
            {
                case XInputAxis.stickLX:
                    ret = leftStickValue.x;
                    break;
                case XInputAxis.stickLY:
                    ret = leftStickValue.y;
                    break;
                case XInputAxis.stickRX:
                    ret = rightStickValue.x;
                    break;
                case XInputAxis.stickRY:
                    ret = rightStickValue.y;
                    break;
            }
        }
        return ret;
    }
}