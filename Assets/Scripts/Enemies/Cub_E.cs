using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒLƒ…ƒ‹ƒt
public class Cub_E : Enemy
{
    Vector3 targetCursor;
    protected override void LiveEntityUpdate()
    {
        //y²‚É‚Í‹ó‹C’ïR‚ª‚©‚©‚ç‚È‚¢‚æ‚¤‚Éİ’è
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
        //d—Í‚ğ‹­‚ß‚Éİ’è
        gravityScale = 1;

        if (IsAttacking())
        {
            if (GetAttackProgress() > 0.5f)
            {

            }
        }
        else
        {
            //UŒ‚“®ì’†‚Å‚È‚¢‚ÉŠl•¨‚ğŒ©‚Â‚¯‚½‚çUŒ‚“®ì‚Ö
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion("upperAim", 60);
            }
        }
    }
}