using System;
using System.Collections;
using UnityEngine;

public class KX_netUtil : object
{
    //HPゲージなどの背景色を算出
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
    //HPゲージなどの減少幅の色を算出
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
    //あるトランスフォームから見た他のトランスフォームの相対座標
    public static Vector3 RelativePosition(Transform observer, Transform target, Vector3 translation)
    {
        return observer.InverseTransformPoint(target.TransformPoint(translation));
    }
    //カメラから見たトランスフォームのスクリーン座標
    public static Vector3 RenderPosition(Camera cam, Transform target, Vector3 translation)
    {
        return cam.WorldToScreenPoint(target.TransformPoint(translation));
    }
    //スクリーン座標を正規スクリーン座標に
    public static Vector3 NormalizeScreenPoint(Vector3 point)
    {
        return new Vector3((point.x / Screen.width - 0.5f) * 2, (point.y / Screen.height - 0.5f) * 2, point.z);
    }
    //正規スクリーン座標をスクリーン座標に
    public static Vector3 InverseNormalizeScreenPoint(Vector3 point)
    {
        return new Vector3((point.x / 2 + 0.5f) * Screen.width, (point.y / 2 + 0.5f) * Screen.height, point.z);
    }
    //二次元座標上の点が矩形内にあるか
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
    //二つのコライダーが接触しているか（大まかにしか判定できないから過信しないでね）
    public static bool IsHit(Collider col1, Collider col2, float fractionDistance)
    {
        return Vector3.Distance(col1.ClosestPoint(col2.ClosestPoint(col1.gameObject.transform.position)), col2.ClosestPoint(col1.ClosestPoint(col2.ClosestPoint(col1.gameObject.transform.position)))) <= fractionDistance
        || Vector3.Distance(col2.ClosestPoint(col1.ClosestPoint(col2.gameObject.transform.position)), col1.ClosestPoint(col2.ClosestPoint(col1.ClosestPoint(col2.gameObject.transform.position)))) <= fractionDistance;
    }
    //ワールド空間上の点がコライダーに接触しているか
    public static bool IsInsidePosition(Collider col, Vector3 point)
    {
        return col.ClosestPoint(point) == point;
    }
    //値が範囲内にあるか
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
    //二つの数値範囲が重なっているか
    public static bool IsCrossingRange(float range1Head, float range1Tail,
        float range2Head, float range2Tail,
        bool notEqualHead, bool notEqualTail)
    {
        return IsIntoRange(range1Head, range2Head, range2Tail,
            false, notEqualTail)
            || IsIntoRange(range1Tail, range2Head, range2Tail,
            notEqualHead, false)
            || IsIntoRange(range2Head, range1Head, range1Tail,
            false, notEqualTail)
            || IsIntoRange(range2Tail, range1Head, range1Tail,
            notEqualHead, false);
    }
    //二つの角度（度数法）の最小差
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
    //数値を0~1の範囲内に収める
    public static float Saturate(float value)
    {
        return Mathf.Clamp(value, 0, 1);
    }
    //数値の範囲マッピング
    public static float RangeMap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return outputMin + (value - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);
    }
    //負の値にも対応した累乗関数
    public static float SignedPow(float value, float pow)
    {
        int sign = Mathf.RoundToInt(Mathf.Sign(value));
        return Mathf.Pow(Mathf.Abs(value), pow) * sign;
    }
    //三角波
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
    //変形可能な余弦波（使う機会は少ないかも）
    public float StreachedCos(float num, float mul, float seg1Pow = 1, float seg2Pow = 1)
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
}
