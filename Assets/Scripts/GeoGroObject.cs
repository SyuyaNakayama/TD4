using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class GeoGroObject : UnLandableObject
{
    [SerializeField]
    Collider currentGround;
    Collider[] touchedGrounds = { };
    Vector3 prevPos;
    public Vector3 GetPrevPos()
    {
        return prevPos;
    }
    protected Vector3 movement;
    public Vector3 GetMovement()
    {
        return movement;
    }
    Vector3 preMovement;
    public Vector3 localGrandMove
    {
        get;
        private set;
    }
    public Vector3 fieldMove
    {
        get;
        private set;
    }
    public Vector3 pushBackMove
    {
        get;
        private set;
    }
    bool isLanding = false; //���n���Ă��邩
    public bool GetIsLanding()
    {
        return isLanding;
    }
    [SerializeField]
    protected bool allowGroundSet = true;
    [SerializeField]
    protected float drag = 0.8f;
    [SerializeField]
    protected KX_netUtil.AxisSwitch dragAxis;
    [SerializeField]
    protected float gravityScale;
    [SerializeField]
    bool noGravity;


    void Awake()
    {
        prevPos = transform.position;

        GGOAwake();
    }

    //�������Z���X�V�����^�C�~���O�Ŗ��t���[���Ă΂��
    //���ӁI�@Update()�Ƃ͌Ă΂��������قȂ邽�ߎ����Y���ɂ��s��ɋC��t���ĉ�����
    void FixedUpdate()
    {
        Collider tempGround = currentGround;
        //�G�ꂽ�R���C�_�[�̂����ł��߂����̂���U���g�̑���Ƃ���
        float nearestGroundDistance = 0;
        bool detected = false;
        for (int i = 0; i < touchedGrounds.Length; i++)
        {
            float currentGroundDistance = Vector3.Magnitude(
                touchedGrounds[i].ClosestPoint(transform.position) - transform.position);
            if ((!detected || currentGroundDistance < nearestGroundDistance))
            {
                tempGround = touchedGrounds[i];
                nearestGroundDistance = currentGroundDistance;
                detected = true;
            }
        }

        //���̃R���C�_�[�����̃R���C�_�[�ƃN���X�^�[�ɂȂ��Ă��邩����
        foreach (MargedGround obj in UnityEngine.Object.FindObjectsOfType<MargedGround>())
        {
            MargedGround.GroundCluster[] groundClusters = obj.GetGroundClusters();
            for (int i = 0; i < groundClusters.Length; i++)
            {
                Collider[] currentColliders = groundClusters[i].colliders;
                for (int j = 0; j < currentColliders.Length; j++)
                {
                    //�N���X�^�[�����������炻�̒��ōł��߂����̂���U���g�̑���Ƃ���
                    if (currentColliders[j] == tempGround)
                    {
                        detected = false;
                        for (int k = 0; k < currentColliders.Length; k++)
                        {
                            if (currentColliders[k] != null)
                            {
                                float currentGroundDistance = Vector3.Magnitude(
                                    currentColliders[k].ClosestPoint(transform.position) - transform.position);
                                if ((!detected || currentGroundDistance < nearestGroundDistance)
                                    && currentColliders[k].GetComponent<UnLandableObject>() == null)
                                {
                                    tempGround = currentColliders[k];
                                    nearestGroundDistance = currentGroundDistance;
                                    detected = true;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        //���̃R���C�_�[�����n�ł�����̂ł���ΐ����Ɏ��g�̑���Ƃ���
        if (tempGround != null && tempGround.GetComponent<UnLandableObject>() == null)
        {
            currentGround = tempGround;
        }

        //�G�ꂽ�R���C�_�[�̏������Z�b�g
        Array.Resize(ref touchedGrounds, 0);

        //����n�ʂɌ�����
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //����������ׂ��ʒu���Z�o����
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //�ǂ��炩�Ƃ����Ώc�����ɑ傫����]����K�v������Ȃ�
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //�ēx����������ׂ��ʒu���Z�o���A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //�ēx����������ׂ��ʒu���Z�o���A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        GGOUpdate();

        //�y�d�v�z��������upreMovement = movement;�v�܂�movement�̒l�����������Ȃ�����
        //movement��velocity�ɕϊ�
        GetComponent<Rigidbody>().velocity =
            transform.rotation * movement * transform.localScale.x;

        Vector3 playerLocalPosPin = transform.InverseTransformPoint(prevPos);
        prevPos = transform.position;

        //�M�~�b�N�ɂ��ړ��Ɋւ���X�V����
        GetComponent<Rigidbody>().velocity += fieldMove;
        playerLocalPosPin += transform.InverseTransformPoint(transform.position + fieldMove * Time.deltaTime);
        localGrandMove = -playerLocalPosPin;
        fieldMove = Vector3.zero;

        GetComponent<Rigidbody>().velocity += pushBackMove;
        pushBackMove = Vector3.zero;

        Vector3 movementDiff = movement - preMovement;
        preMovement = movement;
        //����ȍ~��movement�̒l�����������ėǂ�

        //���n����
        Vector3 pushBackedMovement =
            localGrandMove / Time.deltaTime + movementDiff;

        if (Vector3.Magnitude(pushBackedMovement) < Vector3.Magnitude(movement))
        {
            movement = Vector3.Lerp(movement, pushBackedMovement, 0.5f);
        }

        //��C��R
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
        //�d��
        if (!noGravity)
        {
            movement += new Vector3(0, -gravityScale, 0);
        }
        noGravity = false;

        //�n�ʂƂ̐ڐG������s���O�Ɉ�U���n���Ă��Ȃ���Ԃɂ���
        isLanding = false;
    }

    //���̃I�u�W�F�N�g���R���C�_�[�ɐG��Ă���Ԗ��t���[�����̊֐����Ă΂��i�G��Ă���R���C�_�[�������I�Ɉ����ɓ���j
    //���ӁI�@OnTriggerStay()�ƈ���č��̓��m�̏Փ˔����p�ł�
    void OnCollisionStay(Collision col)
    {
        if (allowGroundSet)
        {
            //����������ׂ��n�`�̌��Ƃ��ēo�^
            Array.Resize(ref touchedGrounds, touchedGrounds.Length + 1);
            touchedGrounds[touchedGrounds.Length - 1] = col.collider;
            // ���n����
            isLanding = true;
        }

        GGOOnCollisionStay(col);
    }

    //���t�g�ɏ���Ă��鎞�╗�ɐ����Ă���̓������������邽�߂̊֐�
    public void AddFieldMove(Vector3 force)
    {
        fieldMove += force;
    }
    //�[���I�ɕǂɉ����ꂽ�悤�ȓ������������邽�߂̊֐�
    public void AddPushBackMove(Vector3 force)
    {
        pushBackMove += force;
    }
    //���̃t���[���̂ݏd�͂̉e�����󂯂Ȃ�����
    public void SetNoGravity()
    {
        noGravity = true;
    }

    protected virtual void GGOUpdate()
    {
    }
    protected virtual void GGOAwake()
    {
    }
    protected virtual void GGOOnCollisionStay(Collision col)
    {
    }
}