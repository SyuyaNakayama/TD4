using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class MedalCounter : MonoBehaviour
{
    //���[���h���Ƃ̃��_���ő�l
    private const int maxMedal = 20;
    //���[���h��
    private const int maxWorld = 10;
    //�X�e�[�W��
    private const int maxStage = 5;
    //�X�e�[�W���Ƃ̃��_��
    private const int maxMedalInStage = 10;
    //���_���̎擾��
    private int[,,] getMedal = new int[11, 6, 11];
    //���[���h���Ƃ̃��_���擾��
    private static uint[] getMedalWorld = new uint[11];
    //���[�v�p�̃X�e�[�W�ԍ��ƃV�[�����̃X�e�[�W�ԍ�
    private int roopWorldNum = 0;
    private int roopStageNum = 0;
    public int thisWorldNum = 0;
    public int thisStageNum = 0;

    //�@�ړ��惁�_�����\��UI
    [SerializeField]
    TMP_Text textComponent;

    //static uint medalCount; // ���_���̊l������

    //�������݂̃��[�h
    private bool isAppend = false;
    void Start()
    {
        //������
        InitializeData();
        //�f�[�^�̗L���̃`�F�b�N
        CheckGetData();
        CheckCountData();
    }
    //�f�[�^������
    private void InitializeData()
    {
        for (int i = 0; i < maxWorld; i++)
        {
            //�X�e�[�W���Ƃ̎擾��
            for (int j = 0; j < maxStage; j++)
            {
                for (int k = 0; k < maxMedalInStage; k++)
                {
                    getMedal[i, j, k] = 0;
                }
            }
            //���[���h���݌v�擾��
            getMedalWorld[i] = 0;
        }
    }
    //�f�[�^�̗L���m�F�A�Ȃ��ꍇ�͐V�K�쐬����
    private void CheckGetData()
    {
        //�f�[�^�i�[�p�X ���g���q�͑��݂��Ȃ����̂ł�OK
        //���_���̎擾�󋵃f�[�^
        string getPath = Application.persistentDataPath + "/medalGetData.mds";
        if (System.IO.Directory.Exists(Application.persistentDataPath))
        {
            if (!System.IO.File.Exists(getPath))
            {
                using (var fs = new StreamWriter(getPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    for (int i = 0; i < maxWorld; i++)
                    {
                        for (int j = 0; j < maxStage; j++)
                        {
                            for (int k = 0; k < maxMedalInStage; k++)
                            {
                                fs.Write(getMedal[i, j, k]);
                            }
                            fs.Write("\n");
                        }
                    }
                }
            }
            else
            {
                //�f�[�^������Ȃ炻���ǂݍ���
                using (var fs = new StreamReader(getPath, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    while (fs.Peek() != -1)
                    {
                        string data = fs.ReadLine();
                        for (int i = 0; i < maxMedalInStage; i++)
                        {
                            getMedal[roopWorldNum, roopStageNum, i] = int.Parse(data[i].ToString());
                        }
                        if (roopStageNum < maxStage)
                        {
                            roopStageNum++;
                        }else
                        {
                            roopStageNum = 0;
                            if (roopWorldNum < maxWorld)
                            {
                                roopWorldNum++;
                            }
                        }
                    }
                }
            }
        }
        //���ł��g�p���邽�߃��Z�b�g
        roopWorldNum = 0;
        roopStageNum = 0;
    }
    private void CheckCountData()
    {
        //���_���̗݌v�l���󋵃f�[�^
        string countPath = Application.persistentDataPath + "/medalCountData.mds";
        if (System.IO.Directory.Exists(Application.persistentDataPath))
        {
            if (!System.IO.File.Exists(countPath))
            {
                using (var fs = new StreamWriter(countPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    for (int i = 0; i < maxWorld; i++)
                    {
                        fs.Write("00\n");
                    }
                }
            }
            else
            {
                //�f�[�^������Ȃ炻���ǂݍ���
                using (var fs = new StreamReader(countPath, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    while (fs.Peek() != -1)
                    {
                        string data = fs.ReadLine();
                        //�\�̈ʂƈ�̈ʂł��ꂼ�ꑫ��
                        getMedalWorld[roopWorldNum] = uint.Parse(data[0].ToString()) * 10;
                        getMedalWorld[roopWorldNum] += uint.Parse(data[1].ToString());
                        if (roopWorldNum < maxWorld)
                        {
                            roopWorldNum++;
                        }
                    }
                }
            }
        }
        //���ł��g�p���邽�߃��Z�b�g
        roopWorldNum = 0;
        //UI�X�V
        textComponent.text = getMedalWorld[thisWorldNum].ToString();
    }
    //�f�[�^�ۑ�
    public void Save()
    {
        //�f�[�^�i�[�p�X ���g���q�͑��݂��Ȃ����̂ł�OK
        string getPath = Application.persistentDataPath + "/medalGetData.mds";
        string countPath = Application.persistentDataPath + "/medalCountData.mds";
        //�擾�f�[�^
        using (var fs = new StreamWriter(getPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; i < maxWorld; i++)
            {
                for (int j = 0; j < maxStage; j++)
                {
                    for (int k = 0; k < maxMedalInStage; k++)
                    {
                        fs.Write(getMedal[i, j, k]);
                    }
                    fs.Write("\n");
                }
            }
        }
        //�J�E���g�f�[�^
        using (var fs = new StreamWriter(countPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; i < maxWorld; i++)
            {
                //2�P�^���邩�ǂ����ŏ�����ς���
                if (getMedalWorld[i] > 9)
                {
                    fs.Write(getMedalWorld[i] + "\n");
                }
                else
                {
                    fs.Write("0" + getMedalWorld[i] + "\n");
                }
            }
        }
    }
    //�f�[�^�폜
    public void DeleteData()
    {
        //�f�[�^�i�[�p�X ���g���q�͑��݂��Ȃ����̂ł�OK
        string getPath = Application.persistentDataPath + "/medalGetData.mds";
        string countPath = Application.persistentDataPath + "/medalCountData.mds";
        using (var fs = new StreamWriter(getPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; i < maxWorld; i++)
            {
                for (int j = 0; j < maxStage; j++)
                {
                    for (int k = 0; k < maxMedalInStage; k++)
                    {
                        fs.Write(0);
                        if (k >= maxMedalInStage)
                        {
                            fs.Write("\n");
                        }
                    }
                }
            }
        }
        using (var fs = new StreamWriter(countPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; i < maxWorld; i++)
            {
                fs.Write("00\n");
            }
        }
    }
    //���_���擾����
    public void AcquisitionMedal(int medalNum)
    {
        getMedal[thisWorldNum, thisStageNum, medalNum] = 1;
    }
    //�f�[�^�擾
    public bool GetMedalData(int medalNum)
    {
        if (getMedal[thisWorldNum, thisStageNum, medalNum] == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // ���_���̊l��
    public void AddMedalCount()
    {
        getMedalWorld[thisWorldNum]++;
        textComponent.text = getMedalWorld[thisWorldNum].ToString();
    }
}
