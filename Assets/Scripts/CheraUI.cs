using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheraUI : MonoBehaviour
{
    //�q�̃L�����摜�̃Q�[���I�u�W�F�N�g
    private GameObject[] charaIcon = new GameObject[6];
    //�q�̕\��������摜�ԍ�
    private int[] charaNumber = new int[7];
    //�q�̍ő吔
    private const int charaMaxNum = 5;

    //�q��Image
    private Image[] charaImages = new Image[6];
    //�X�V�p�X�v���C�g
    private Sprite sprite;

    //�v���C���[
    public GameObject playerObject;
    private Player player;
    //��x�����X�V���邽�ߍU�������ǂ����̃t���O
    bool isAttackNow = false;

    void Start()
    {
        player = playerObject.GetComponent<Player>();

        //�q�̃I�u�W�F�N�g�f�[�^�̎擾
        for(int i = 5; i < 10; i++)
        {
            charaIcon[i - 5] = transform.GetChild(i).gameObject;
            //�ԍ�������
            charaNumber[i - 5] = 0;
            charaImages[i - 5] = charaIcon[i - 5].GetComponent<Image>();
        }
        //6�Ԗڂ̉摜�ԍ�����
        charaNumber[6] = 0;
        //�f�o�b�O�p�ɉ��ɔԍ������
        charaNumber[0] = 1;
        charaNumber[1] = 2;
        charaNumber[2] = 3;
        charaNumber[3] = 3;
        charaNumber[4] = 3;

        //�摜�\��
        UpdateSprite();
    }
    void Update()
    {
        CheckIsAttack();
    }
    //�X�v���C�g�摜�̍X�V
    private void UpdateSprite()
    {
        //0�ԂȂ��\���A����ȊO�Ȃ�ԍ��ɑΉ������X�v���C�g�ɍ����ւ�
        for (int i = 0; i < charaMaxNum; i++)
        {
            if (charaNumber[i] > 0)
            {
                charaIcon[i].SetActive(true);
                sprite = Resources.Load<Sprite>("CharaUI/sc" + charaNumber[i]);
                charaImages[i].sprite = sprite;
            }
            else
            {
                charaIcon[i].SetActive(false);
            }
        }
    }
    //�X�L���g�p����UI���ł̏���
    public void ChangeIcon()
    {
        for(int i = 0;i < charaMaxNum; i++)
        {
            charaNumber[i] = charaNumber[i + 1];
        }
        //�S�Ă̍X�V���I�������ɕ\���ؑ�
        UpdateSprite();
    }
    //�v���C���[���U���������Ď�
    private void CheckIsAttack()
    {
        if (player.GetAttackTrigger() && !isAttackNow)
        {
            ChangeIcon();
            isAttackNow = true;
        }

        if(!player.GetAttackTrigger() && isAttackNow)
        {
            isAttackNow=false;
        }
    }
}
