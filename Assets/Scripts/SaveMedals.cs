using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class SaveMedals : MonoBehaviour
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
    private bool[,,] getMedal = new bool[11, 6, 11];
    //���[���h���Ƃ̃��_���擾��
    private int[] getMedalWorld = new int[11];
    //���[�v�p�̃X�e�[�W�ԍ��ƃV�[�����̃X�e�[�W�ԍ�
    private int roopWorldNum = 0;
    private int roopStageNum = 0;
    public int thisWorldNum = 0;
    public int thisStageNum = 0;

    //�������݂̃��[�h
    private bool isAppend = false;
    //�f�[�^�i�[�p�X ���g���q�͑��݂��Ȃ����̂ł�OK
    //���_���̎擾�󋵃f�[�^
    private string getPath = Application.persistentDataPath + "/medalGetData.mds";
    //���_���̗݌v�l���󋵃f�[�^
    private string countPath = Application.persistentDataPath + "/medalCountData.mds";
    void Start()
    {
        //������
        InitializeData();
        //�f�[�^�̗L���̃`�F�b�N
        CheckGetData(getPath);
        CheckCountData(countPath);
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
                    getMedal[i, j, k] = false;
                }
            }
            //���[���h���݌v�擾��
            getMedalWorld[i] = 0;
        }
    }
    //�f�[�^�̗L���m�F�A�Ȃ��ꍇ�͐V�K�쐬����
    private void CheckGetData(string getPath)
    {
        if (System.IO.Directory.Exists(Application.persistentDataPath))
        {
            if (!System.IO.File.Exists(getPath))
            {
                using (var fs = new StreamWriter(getPath, isAppend, System.Text.Encoding.GetEncoding("UTRF-8")))
                {
                    for (int i = 0; i < maxWorld; i++)
                    {
                        for (int j = 0; j < maxStage; j++)
                        {
                            for (int k = 0; k < maxMedalInStage; k++)
                            {
                                fs.Write(getMedal[i, j, k]);
                                if (k >= maxMedalInStage)
                                {
                                    fs.Write("\n");
                                }
                            }
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
                            getMedal[roopWorldNum, roopStageNum, i] = bool.Parse(data[i].ToString());
                        }
                        if (roopWorldNum < maxWorld)
                        {
                            roopWorldNum++;
                        }
                        if (roopStageNum < maxStage)
                        {
                            roopStageNum++;
                        }
                    }
                }
            }
        }
        //���ł��g�p���邽�߃��Z�b�g
        roopWorldNum = 0;
        roopStageNum = 0;
    }
    private void CheckCountData(string countPath)
    {
        if (System.IO.Directory.Exists(Application.persistentDataPath))
        {
            if (!System.IO.File.Exists(countPath))
            {
                using (var fs = new StreamWriter(countPath, isAppend, System.Text.Encoding.GetEncoding("UTRF-8")))
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
                        getMedalWorld[roopWorldNum] = int.Parse(data[0].ToString()) * 10;
                        getMedalWorld[roopWorldNum] += int.Parse(data[1].ToString());
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
    }
    //�f�[�^�ۑ�
    public void Save()
    {
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
                        if (k >= maxMedalInStage)
                        {
                            fs.Write("\n");
                        }
                    }
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
    public void GetMedal(int medalNum)
    {
        getMedal[thisWorldNum, thisStageNum, medalNum] = true;
    }
    //�f�[�^�擾
    public bool GetMedalData(int medalNum)
    {
        if (getMedal[thisWorldNum, thisStageNum, medalNum] == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
