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
        //���Ă�����\���Ɠ����蔻�������
        GetComponent<MeshRenderer>().enabled = !breaked;
        GetComponent<Collider>().enabled = !breaked;
        //��ꂽ�u�Ԃɔj�Ђ��U�炷
        if (breaked && !prevBreaked)
        {
            particle.Play();
        }
        prevBreaked = breaked;
    }
    void OnTriggerStay(Collider other)
    {
        //�U�����肪������̂ɐG�ꂽ�����
        if (other.gameObject.GetComponent<AttackArea>() != null)
        {
            breaked = true;
        }
    }
}