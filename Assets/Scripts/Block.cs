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
        //壊れていたら表示と当たり判定を消す
        renderer.enabled = !breaked;
        collider.enabled = !breaked;
        //壊れた瞬間に音を鳴らし破片を散らす
        if (breaked && !prevBreaked)
        {
            particle.Play();
            audioSource.Play();
        }
        prevBreaked = breaked;
    }
    void OnTriggerStay(Collider other)
    {
        //攻撃判定があるものに触れたら壊れる
        if (other.gameObject.GetComponent<AttackArea>() != null)
        {
            breaked = true;
        }
    }
}