using System;
using UnityEngine;

public class Sensor : MonoBehaviour
{

    //�⑫�@C#�ł̓N���X�^�̕ϐ��͊�{�I�Ƀ|�C���^�ϐ��Ƃ��Ĉ����܂��@�܂�null�ɂ��Ȃ�܂�

    LiveEntity[] targets = { };
    public LiveEntity[] GetTargets()
    {
        return targets;
    }
    LiveEntity[] tempTargets = { };

    //�������Z���X�V�����^�C�~���O�Ŗ��t���[���Ă΂��
    //���ӁI�@Update()�Ƃ͌Ă΂��������قȂ邽�ߎ����Y���ɂ��s��ɋC��t���ĉ�����
    void FixedUpdate()
    {
        //�擾�p�̔z��ɑS�v�f���
        targets = tempTargets;
        //�ēx�X�V���鏀���Ƃ��ăN���A
        Array.Resize(ref tempTargets, 0);
    }

    //���̃I�u�W�F�N�g���R���C�_�[�ɐG��Ă���Ԗ��t���[�����̊֐����Ă΂��i�G��Ă���R���C�_�[�������I�Ɉ����ɓ���j
    //���ӁI�@OnCollisionStay()�ƈ���ăg���K�[�^�̐ڐG�����p�ł�
    private void OnTriggerStay(Collider other)
    {
        LiveEntity tempLiveEntity =
            other.GetComponent<LiveEntity>();
        //�G�ꂽ�I�u�W�F�N�g��LiveEntity�R���|�[�l���g������Ȃ�
        if (tempLiveEntity != null)
        {
            //�X�V�p�̔z��ɑ��
            Array.Resize(ref tempTargets, tempTargets.Length + 1);
            tempTargets[tempTargets.Length - 1] = tempLiveEntity;
        }

    }
}
