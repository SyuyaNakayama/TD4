using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : UnLandableObject
{
    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    bool breaked = false;
    bool prevBreaked;

    void FixedUpdate()
    {
        //‰ó‚ê‚Ä‚¢‚½‚ç•\¦‚Æ“–‚½‚è”»’è‚ğÁ‚·
        GetComponent<MeshRenderer>().enabled = !breaked;
        GetComponent<Collider>().enabled = !breaked;
        //‰ó‚ê‚½uŠÔ‚É”j•Ğ‚ğU‚ç‚·
        if (breaked && !prevBreaked)
        {
            particle.Play();
        }
        prevBreaked = breaked;
    }
    void OnTriggerStay(Collider other)
    {
        //UŒ‚”»’è‚ª‚ ‚é‚à‚Ì‚ÉG‚ê‚½‚ç‰ó‚ê‚é
        if (other.gameObject.GetComponent<AttackArea>() != null)
        {
            breaked = true;
        }
    }
}