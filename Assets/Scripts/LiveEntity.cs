using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveEntity : MonoBehaviour
{
    public struct AxisSwitch
    {
        public bool x;
        public bool y;
        public bool z;
    }

    protected Vector3 movement;
    protected AxisSwitch dragAxis;
    protected 

    void Start()
    {
        
    }

    //�������Z���X�V�����^�C�~���O�Ŗ��t���[���Ă΂��
    //���ӁI�@Update()�Ƃ͌Ă΂��������قȂ邽�ߎ�������ɂ��s��ɋC��t���ĉ�����
    void FixedUpdate()
    {
        //�����Ŋe�h���N���X�̌ŗL�X�V�������Ă�
        LiveEntityUpdate();

        //������
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //�d�͋y�ы�C��R

        if (dragAxis.x && dragAxis.y && dragAxis.z)
        {
            movement *= 0.8f;
        }
        else
        {
            if(dragAxis.x)
            {
                movement.x *= 0.8f;
            }
            if (dragAxis.y)
            {
                movement.y *= 0.8f;
            }
            if (dragAxis.z)
            {
                movement.z *= 0.8f;
            }
        }
        movement += new Vector3(0, -0.5f, 0);
    }

    //���̃I�u�W�F�N�g���R���C�_�[�ɐG��Ă���Ԗ��t���[�����̊֐����Ă΂��i�G��Ă���R���C�_�[�������I�Ɉ����ɓ���j
    void OnCollisionStay(Collision col)
    {
        //����������ׂ��ʒu���Z�o���A
        Vector3 localClosestPoint = transform.InverseTransformPoint(
            col.collider.ClosestPoint(transform.position));
        //x���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
        transform.Rotate(
            -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y) / Mathf.Deg2Rad, 0, 0, Space.Self);

        //�ēx����������ׂ��ʒu���Z�o���A
        localClosestPoint = transform.InverseTransformPoint(
            col.collider.ClosestPoint(transform.position));
        //z���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
        transform.Rotate(0, 0, 
            Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y) / Mathf.Deg2Rad, Space.Self);
    }

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityUpdate()
    {

    }
}
