using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : UnLandableObject
{
    [SerializeField]
    Renderer renderer;
    [SerializeField]
    Collider collider;
    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    bool breaked = false;
    public bool GetBreaked()
    {
        return breaked;
    }
    bool prevBreaked;

    void FixedUpdate()
    {
        //‰ó‚ê‚Ä‚¢‚½‚ç•\¦‚Æ“–‚½‚è”»’è‚ğÁ‚·
        renderer.enabled = !breaked;
        collider.enabled = !breaked;
        //‰ó‚ê‚½uŠÔ‚É‰¹‚ğ–Â‚ç‚µ”j•Ğ‚ğU‚ç‚·
        if (breaked && !prevBreaked)
        {
            particle.Play();
            audioSource.Play();
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