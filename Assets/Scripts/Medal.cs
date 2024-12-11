using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    static Medal[] allInstances = { };
    public static Medal[] GetAllInstances()
    {
        return KX_netUtil.CopyArray<Medal>(allInstances);
    }

    [SerializeField]
    GameObject particle;
    [SerializeField]
    GameObject compParticle;


    void FixedUpdate()
    {
        List<Medal> allInstancesList =
            new List<Medal>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;
    }

    public void OnTriggerEnter(Collider col)
    {
        //プレイヤーのみ取れる
        LiveEntity liveEntity =
            col.gameObject.GetComponent<LiveEntity>();
        if (liveEntity && liveEntity.GetUserControl())
        {
            //ゲット時のパーティクル
            GameObject tempParticle =
                Instantiate(particle, transform.position, transform.rotation);
            tempParticle.transform.localScale = transform.lossyScale;

            //allInstancesから自分を消してみる
            List<Medal> allInstancesList =
                new List<Medal>(allInstances);
            allInstancesList.RemoveAll(where => !where || where == this);
            allInstances = allInstancesList.ToArray();
            //allInstancesが空になったらコンプリート表示
            if (allInstances.Length <= 0)
            {
                GameObject tempCompParticle =
                    Instantiate(compParticle, transform.position, transform.rotation);
                tempCompParticle.transform.localScale = transform.lossyScale;
            }

            Destroy(gameObject);
        }
    }
}
