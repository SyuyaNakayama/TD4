using UnityEngine;
using System.Collections.Generic;

public class Player : CharacterCassette
{
    public const int maxTeamNum = 5;
    public const int maxAttackReactionframe = 10;
    const float cameraControlSpeed = 3;
    const float moveSpeed = 1.5f;
    const float jumpPower = 10.0f;
    const float maxPlayerRotSpeed = 3;
    const float playerRotSpeedDiffuse = 0.5f;

    [SerializeField]
    CharaData[] weapons;
    public CharaData[] GetWeapons()
    {
        return weapons;
    }

    bool prevJumpInput;
    bool attackTrigger;
    int attackReactionFrame;
    public int GetAttackReactionFrame()
    {
        return attackReactionFrame;
    }
    int currentWeaponIndex;
    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
    CharaData latestUsedWeapon;
    AnimationObject currentWeaponVisual;
    float playerRotSpeed;

    protected override void CharaUpdate()
    {
        List<CharaData> weaponList = new List<CharaData>(weapons);
        weaponList.Remove(null);
        weapons = weaponList.ToArray();

        //�ړ��p�̕������͂ňړ�
        Vector2 moveInputVec = GetLiveEntity().GetControlMap().GetMoveInputVec();
        if (moveInputVec.x != 0 || moveInputVec.y != 0)
        {
            GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + new Vector3(
                moveInputVec.x,
                0,
                moveInputVec.y).normalized
                * moveSpeed);
            GetLiveEntity().SetDirection(Mathf.Atan2(
                moveInputVec.x, moveInputVec.y)
                / Mathf.Deg2Rad);
        }

        //�W�����v�̓��͂ŃW�����v
        bool jumpInput = GetLiveEntity().GetControlMap().GetJumpInput();
        if (jumpInput && !prevJumpInput)
        {
            GetLiveEntity().SetMovement(new Vector3(
                GetLiveEntity().GetMovement().x,
                jumpPower,
                GetLiveEntity().GetMovement().z));
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
        prevJumpInput = jumpInput;

        attackReactionFrame = Mathf.Max(0, attackReactionFrame - 1);

        bool usingWeapon =
            IsAttacking() && !IsAttacking(GetData().GetDefaultAttackMotionName());
        //�U���̓��͂ōU��
        bool attackInput = GetLiveEntity().GetControlMap().GetWeaponInput();
        if (attackInput && !attackTrigger && !usingWeapon)
        {
            attackReactionFrame = maxAttackReactionframe;
            if (weapons.Length > 0)
            {
                latestUsedWeapon = weapons[currentWeaponIndex];
                SetAttackMotion(
                    latestUsedWeapon.SearchAttackMotion(
                    latestUsedWeapon.GetWeaponedAttackMotionName()));
                currentWeaponIndex++;
            }
        }
        attackTrigger = attackInput;
        currentWeaponIndex = Mathf.RoundToInt(Mathf.Repeat(currentWeaponIndex,
            weapons.Length));

        //����g�p���̃A�j���[�V����
        if (usingWeapon)
        {
            //�K�؂ȕ���I�u�W�F�N�g��������ΐ���
            if (!currentWeaponVisual)
            {
                GameObject weaponObj = Instantiate(
                    GetLiveEntity().GetLib().FindCharacterCassette(
                    latestUsedWeapon.name).GetVisual().gameObject,
                    transform.position, transform.rotation, transform);
                currentWeaponVisual = weaponObj.GetComponent<AnimationObject>();
            }

            //�U���A�j���[�V�����𕐊�ɓK�p
            currentWeaponVisual.animationName = GetVisual().animationName;
            currentWeaponVisual.animationProgress = GetVisual().animationProgress;
            //���g�͌ŗL�A�j���[�V����
            GetVisual().animationName = "attack";

            AddUnits(currentWeaponVisual.gameObject);
        }
        else
        {
            currentWeaponVisual = null;
        }
        //�g�p���Ă��Ȃ�����I�u�W�F�N�g��T���ď���
        for (int i = 0; i < transform.childCount; i++)
        {
            AnimationObject currentAO =
                transform.GetChild(i).GetComponent<AnimationObject>();
            if (currentAO && currentAO != GetVisual()
                && currentAO != currentWeaponVisual)
            {
                Destroy(currentAO.gameObject);
            }
        }

        //�J�����p�̏㉺���͂ŃJ�����̋p����
        Vector2 camInputVec = GetLiveEntity().GetControlMap().GetCamInputVec();
        transform.Rotate(
            0, camInputVec.x * cameraControlSpeed, 0, Space.Self);
        float playerCameraAngle =
            GetLiveEntity().GetCameraAngle() + camInputVec.y * cameraControlSpeed;

        playerCameraAngle = Mathf.Clamp(
            playerCameraAngle, LiveEntity.MinCameraAngle, LiveEntity.MaxCameraAngle);
        GetLiveEntity().SetCameraAngle(playerCameraAngle);
        //�J�����p�̍��E���͂ŉ�]
        playerRotSpeed = Mathf.Clamp(
            playerRotSpeed * playerRotSpeedDiffuse + camInputVec.x,
            -maxPlayerRotSpeed, maxPlayerRotSpeed);
        GetLiveEntity().transform.Rotate(0, playerRotSpeed, 0, Space.Self);
        //���@�͋t��]���A��]�𑊎E
        GetLiveEntity().SetDirection(
            GetLiveEntity().GetDirection() - playerRotSpeed);
    }

    public void EquipCharacter(CharaData charaData)
    {
        if (GetLiveEntity().GetAllowedItemEffect())
        {
            if (weapons.Length < maxTeamNum)
            {
                List<CharaData> list = new List<CharaData>(weapons);
                list.Insert(Mathf.Clamp(currentWeaponIndex, 0, weapons.Length),
                    charaData);
                weapons = list.ToArray();
            }
            else
            {
                weapons[currentWeaponIndex] = charaData;
            }
        }
    }
}