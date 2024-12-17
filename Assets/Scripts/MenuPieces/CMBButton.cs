using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMBButton : CMBMenuPiece
{
    protected override void MPUpdate()
    {
        if (GetControlMap().IsReleased() &&
            KX_netUtil.IsInsideHitBox(
                GetRectTransform(),
                GetControlMap().GetManager().GetLiveEntity().GetView(),
                GetControlMap().GetInputPosition()))
        {
            output = true;
        }
    }
}