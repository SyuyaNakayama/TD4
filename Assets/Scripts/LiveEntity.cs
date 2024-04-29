using UnityEngine;

public class LiveEntity : MonoBehaviour
{
    public struct AxisSwitch
    {
        public bool x;
        public bool y;
        public bool z;
    }

    [SerializeField]
    string teamID;
    public string GetTeamID()
    {
        return teamID;
    }
    protected float drag = 0.8f;
    protected float gravityScale = 0.5f;
    protected Vector3 movement;
    protected AxisSwitch dragAxis;
    Vector3 prevPos;
    Quaternion prevRot;
    Collider currentGround;
    bool allowGroundSet;
    bool isLanding = false; //���n���Ă��邩
    public bool GetIsLanding()
    {
        return isLanding;
    }
    string attackMotionID = "";
    int attackMotionFrame;
    float attackProgress;
    public float GetAttackProgress()
    {
        return attackProgress;
    }

    void Awake()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    //�������Z���X�V�����^�C�~���O�Ŗ��t���[���Ă΂��
    //���ӁI�@Update()�Ƃ͌Ă΂��������قȂ邽�ߎ����Y���ɂ��s��ɋC��t���ĉ�����
    void FixedUpdate()
    {
        if (currentGround != null)
        {
            //����������ׂ��ʒu���Z�o���A
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //x���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
            transform.Rotate(
                -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y) / Mathf.Deg2Rad, 0, 0, Space.Self);

            //�ēx����������ׂ��ʒu���Z�o���A
            localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //z���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
            transform.Rotate(0, 0,
                Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y) / Mathf.Deg2Rad, Space.Self);
        }

        //�o�E���h�h�~�̂��ߕύX�O��movement��ۑ�
        Vector3 prevMovement = movement;
        //�O�t���[������̈ړ��ʂ�movement�ɕϊ�
        movement = Quaternion.Inverse(prevRot) * ((transform.position - prevPos) / Time.deltaTime);
        prevPos = transform.position;
        prevRot = transform.rotation;
        //�o�E���h�h�~����
        if (Mathf.Sign(prevMovement.y) != Mathf.Sign(movement.y))
        {
            movement.y = 0;
        }

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
        movement += new Vector3(0, -gravityScale, 0);

        //allowGroundSet�����Z�b�g
        allowGroundSet = true;

        //�����Ŋe�h���N���X�̌ŗL�X�V�������Ă�
        LiveEntityUpdate();

        //movement��velocity�ɕϊ�
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //�n�ʂƂ̐ڐG������s���O�Ɉ�U���n���Ă��Ȃ���Ԃɂ���
        isLanding = false;

        //�U�����[�V�����̐i�s�x�𑝉�
        attackProgress += 1 / Mathf.Max((float)attackMotionFrame, 1);
        //�i�s�x���P�𒴂�����U�����[�V�������~�߂�
        if (attackProgress > 1)
        {
            attackMotionID = "";
        }
        attackProgress = Mathf.Clamp(attackProgress, 0, 1);
    }

    //���̃I�u�W�F�N�g���R���C�_�[�ɐG��Ă���Ԗ��t���[�����̊֐����Ă΂��i�G��Ă���R���C�_�[�������I�Ɉ����ɓ���j
    //���ӁI�@OnTriggerStay()�ƈ���č��̓��m�̏Փ˔����p�ł�
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.GetComponent<LiveEntity>() == null && allowGroundSet)
        {
            //����������ׂ��n�`�Ƃ��ēo�^
            currentGround = col.collider;
            // ���n����
            isLanding = true;
        }

        //�����Ŋe�h���N���X�̌ŗL�ڐG�������Ă�
        LiveEntityCollision();
    }

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityUpdate()
    {
    }

    //�e�h���N���X�̌ŗL�ڐG�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityCollision()
    {
    }

    //�U�����[�V�����Ɉڍs
    protected void SetAttackMotion(string setAttackMotionID, int setAttackMotionFrame)
    {
        attackMotionID = setAttackMotionID;
        attackMotionFrame = Mathf.Max(setAttackMotionFrame, 1);
        attackProgress = 0;
    }

    //�U�����[�V��������
    public bool IsAttacking()
    {
        return attackProgress < 1 && attackMotionID != "";
    }

    //������Ă�ł���Ԃ͒n�`�ɐG��Ă��������ɑ��������Ȃ��Ȃ�
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }
}