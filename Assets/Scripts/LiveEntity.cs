using JetBrains.Annotations;
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

    public float drag = 0.8f;
    protected Vector3 movement;
    protected AxisSwitch dragAxis;
    Vector3 prevPos;
    Quaternion prevRot;

    void Awake()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    //�������Z���X�V�����^�C�~���O�Ŗ��t���[���Ă΂��
    //���ӁI�@Update()�Ƃ͌Ă΂��������قȂ邽�ߎ����Y���ɂ��s��ɋC��t���ĉ�����
    void FixedUpdate()
    {
        //�O�t���[������̈ړ��ʂ�movement�ɕϊ�
        movement = Quaternion.Inverse(prevRot) * ((transform.position - prevPos) / Time.deltaTime);
        prevPos = transform.position;
        prevRot = transform.rotation;

        //�d�͋y�ы�C��R
        if (dragAxis.x && dragAxis.y && dragAxis.z)
        {
            movement *= drag;
        }
        else
        {
            if (dragAxis.x)
            {
                movement.x *= drag;
            }
            if (dragAxis.y)
            {
                movement.y *= drag;
            }
            if (dragAxis.z)
            {
                movement.z *= drag;
            }
        }
        movement += new Vector3(0, -0.5f, 0);

        //�����Ŋe�h���N���X�̌ŗL�X�V�������Ă�
        LiveEntityUpdate();

        //movement��velocity�ɕϊ�
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;
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

        //�����Ŋe�h���N���X�̌ŗL�R���C�_�[�������Ă�
        LiveEntityCollision();
    }

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityUpdate()
    {

    }

    //�e�h���N���X�̌ŗL�R���C�_�[�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityCollision()
    {

    }
}
