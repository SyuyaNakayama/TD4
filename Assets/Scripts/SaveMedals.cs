using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class SaveMedals : MonoBehaviour
{
    //ワールドごとのメダル最大値
    private const int maxMedal = 20;
    //ワールド数
    private const int maxWorld = 10;
    //ステージ数
    private const int maxStage = 5;
    //ステージごとのメダル
    private const int maxMedalInStage = 10;
    //メダルの取得状況
    private bool[,,] getMedal = new bool[11, 6, 11];
    //ワールドごとのメダル取得状況
    private int[] getMedalWorld = new int[11];
    //ループ用のステージ番号とシーン内のステージ番号
    private int roopWorldNum = 0;
    private int roopStageNum = 0;
    public int thisWorldNum = 0;
    public int thisStageNum = 0;

    //書き込みのモード
    private bool isAppend = false;
    //データ格納パス ※拡張子は存在しないものでもOK
    //メダルの取得状況データ
    private string getPath = Application.persistentDataPath + "/medalGetData.mds";
    //メダルの累計獲得状況データ
    private string countPath = Application.persistentDataPath + "/medalCountData.mds";
    void Start()
    {
        //初期化
        InitializeData();
        //データの有無のチェック
        CheckGetData(getPath);
        CheckCountData(countPath);
    }
    //データ初期化
    private void InitializeData()
    {
        for (int i = 0; i < maxWorld; i++)
        {
            //ステージごとの取得状況
            for (int j = 0; j < maxStage; j++)
            {
                for (int k = 0; k < maxMedalInStage; k++)
                {
                    getMedal[i, j, k] = false;
                }
            }
            //ワールド内累計取得状況
            getMedalWorld[i] = 0;
        }
    }
    //データの有無確認、ない場合は新規作成する
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
                //データがあるならそれを読み込む
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
        //他でも使用するためリセット
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
                //データがあるならそれを読み込む
                using (var fs = new StreamReader(countPath, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    while (fs.Peek() != -1)
                    {
                        string data = fs.ReadLine();
                        //十の位と一の位でそれぞれ足す
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
        //他でも使用するためリセット
        roopWorldNum = 0;
    }
    //データ保存
    public void Save()
    {
        //取得データ
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
        //カウントデータ
        using (var fs = new StreamWriter(countPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; i < maxWorld; i++)
            {
                //2ケタあるかどうかで処理を変える
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
    //データ削除
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
    //メダル取得判定
    public void GetMedal(int medalNum)
    {
        getMedal[thisWorldNum, thisStageNum, medalNum] = true;
    }
    //データ取得
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
