using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒLƒ…ƒ‹ƒt
public class Cub_E : Enemy
{
    protected override void LiveEntityUpdate()
    {
        //y²‚É‚Í‹ó‹C’ïR‚ª‚©‚©‚ç‚È‚¢‚æ‚¤‚Éİ’è
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
    }
}