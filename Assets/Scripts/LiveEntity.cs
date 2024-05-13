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
    CharaData data;
    public CharaData GetData()
    {
        return data;
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
    int maxHP;//�ő�̗�
    int hpAmount = 1;//�c��̗͂̊���
    bool shield;//���ꂪtrue�̊Ԃ͋Z�ɂ�閳�G����
    AttackMotionData attackMotionData;
    int attackTimeFrame;
    float attackProgress;
    public float GetAttackProgress()
    {
        return attackProgress;
    }
    float prevAttackProgress;
    AttackMotionData.Cursor[] cursors = { };


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
        //shield�����Z�b�g
        shield = false;

        //�����Ŋe�h���N���X�̌ŗL�X�V�������Ă�
        LiveEntityUpdate();

        //movement��velocity�ɕϊ�
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //�n�ʂƂ̐ڐG������s���O�Ɉ�U���n���Ă��Ȃ���Ԃɂ���
        isLanding = false;

        //prevAttackProgress���X�V
        prevAttackProgress = GetAttackProgress();
        //�U�����[�V�����̐i�s�x�𑝉�
        attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
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
    protected void SetAttackMotion(string name)
    {
        attackMotionData = data.SearchAttackMotion(name);
        attackTimeFrame = Mathf.Max(attackMotionData.GetData().totalFrame, 1);
        attackProgress = 0;
    }

    //�U�����[�V��������
    protected bool IsAttacking()
    {
        return attackTimeFrame < 1 || prevAttackProgress < 1;
    }
    //�U�����[�V���������w��̍U���A�N�V�������s�Ȃ��Ă��邩
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.GetData().name == name;
    }
    //attackProgress���w��̃L�[�|�C���g��ʉ߂�����
    protected bool IsHitKeyPoint(float keyPoint)
    {
        return KX_netUtil.IsIntoRange(
            keyPoint, prevAttackProgress, GetAttackProgress(),
            false, true);
    }
    //attackProgress���w��͈͓̔��A�������͂��͈̔͂�1�t���[�����Œʉ߂�����
    protected bool IsHitKeyPoint(Vector2 keyPoint)
    {
        return KX_netUtil.IsCrossingRange(
            prevAttackProgress, GetAttackProgress(),
            keyPoint.x, keyPoint.y,
            false, false);
    }

    //������Ă�ł���Ԃ͒n�`�ɐG��Ă��������ɑ��������Ȃ��Ȃ�
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }

    // �_���[�W���󂯂�
    public void Damage(int damage)
    {
        if (!shield)
        {
            hpAmount -= damage / maxHP;
        }
    }

    //�����Ă��邩
    public bool IsLive()
    {
        return hpAmount > 0;
    }

    //�ݒ肳�ꂽ���[�V�����f�[�^��ǂݏo���Ď��s�i���s���͏�ɌĂԁj
    void ExecuteAttackMotion()
    {
        if (IsAttacking())
        {
            if (attackMotionData.GetData().meleeAttackKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        meleeAttackKeys.Length; i++)
                {
                    AttackMotionData.AttackKey current =
                        attackMotionData.GetData().meleeAttackKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        AttackMotionData.MeleeAttackData meleeAttackData =
                            attackMotionData.SearchMeleeAttackData(current.dataName);
                        MeleeAttack(meleeAttackData,
                            cursors[attackMotionData.SearchCursorIndex(current.cursorName)]);
                    }
                }
            }

            if (attackMotionData.GetData().shotKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    shotKeys.Length; i++)
                {
                    AttackMotionData.AttackKey current =
                        attackMotionData.GetData().shotKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        AttackMotionData.ShotData shotData =
                            attackMotionData.SearchShotData(current.dataName);
                        Shot(shotData,
                            cursors[attackMotionData.SearchCursorIndex(current.cursorName)]);
                    }
                }
            }

            if (attackMotionData.GetData().shieldKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        shieldKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().shieldKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        shield = true;
                    }
                }
            }

            if (attackMotionData.GetData().shieldKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        shieldKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().shieldKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        DisAllowGroundSet();
                    }
                }
            }

            if (attackMotionData.GetData().seKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    seKeys.Length; i++)
                {
                    AttackMotionData.SEKey current =
                        attackMotionData.GetData().seKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        GetComponent<AudioSource>().clip = current.se;
                        GetComponent<AudioSource>().Play();
                    }
                }
            }
        }
    }

    //�ߐڂ���є͈͍U��
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData,
        AttackMotionData.Cursor cursor)
    {

    }
    //�������U��
    void Shot(AttackMotionData.ShotData shotData,
        AttackMotionData.Cursor cursor)
    {

    }
}