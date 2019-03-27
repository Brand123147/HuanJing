using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //static GameManager()
    //{
    //    GameObject go = new GameObject("GameManager");
    //    Instance = go.AddComponent<GameManager>();
    //    DontDestroyOnLoad(go);
    //}
    public bool IsGameStarted { get; set; }  // 游戏开始
    public bool IsGameOver { get; set; }   // 游戏结束
    public bool IsGamePause { get; set; }   // 游戏暂停
    public bool PlayerIsMove { get; set; }   // 玩家是否开始移动

    private int gameScore;  // 游戏成绩 
    private int gameDiamond;  // 游戏钻石

    private GameData data;
    private ManagerVars vars;

    #region 因为这里要对这些数据进行设置所以要持有一下gamedata中的数据

    private bool isFirstGame;   // 是否第一次打开游戏

    private bool isMusicOn;  // 是否打开音效

    private int[] bestScoreArr;  // 最佳成绩，前3个

    private int selectSkin;   // 选中皮肤

    private bool[] skinUnlocked; // 是否解锁皮肤

    private int diamondCount;  // 记录钻石数量

    #endregion

    private void Awake()
    {
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        //  DontDestroyOnLoad(this);  // 没有多个场景不需要
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventDefine.AddDiamond, AddGameDiamond);
        InitGameData();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
        EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddGameDiamond);
    }

    private void Start()
    {

    }

    /// <summary>
    /// 游戏成绩++
    /// </summary>
    private void AddGameScore()
    {
        if (IsGameOver || IsGamePause || IsGameStarted == false)  // 安全校验，这些情况下不加分数
        {
            return;
        }
        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdateScoreText, gameScore);   // 相当于调用GamePanel中的更新成绩函数显示
    }

    /// <summary>
    /// 获取游戏成绩
    /// </summary>
    /// <returns></returns>
    public int GetGameScore
    {
        get
        {
            return gameScore;
        }
    }

    /// <summary>
    /// 获取排行榜前三
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr()
    {
        List<int> list = bestScoreArr.ToList();     // 将数组转换成list，需用用到Linq
        // 从大到小排序
        list.Sort((x, y) => (-x.CompareTo(y)));  // list.Sort((x, y) => (x.CompareTo(y)))     从小到大排序
        bestScoreArr = list.ToArray();
        return bestScoreArr;
    }

    /// <summary>
    /// 游戏钻石++
    /// </summary>
    private void AddGameDiamond()
    {
        gameDiamond++;
        EventCenter.Broadcast(EventDefine.UpdateDiamondText, gameDiamond);
    }

    /// <summary>
    /// 获取游戏钻石
    /// </summary>
    /// <returns></returns>
    public int GetGameDiamond
    {
        get
        {
            return gameDiamond;
        }
    }

    /// <summary>
    /// 获取所得所有钻石
    /// </summary>
    /// <returns></returns>
    public int GetAllDiamond()
    {
        return diamondCount;
    }

    /// <summary>
    /// 获取未解锁皮肤
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool GetSkinUnlocked(int index)
    {
        return skinUnlocked[index];
    }
    /// <summary>
    /// 设置当前皮肤解锁
    /// </summary>
    /// <param name="index"></param>
    public void SetSkinUnlocaked(int index)
    {
        skinUnlocked[index] = true;
        Save();
    }

    /// <summary>
    /// 玩家移动
    /// </summary>
    private void PlayerMove()
    {
        PlayerIsMove = true;
    }

    /// <summary>
    /// 初始化游戏数据，看是否是第一次玩，如果不是第一次玩则获取已经存储的数据
    /// </summary>
    private void InitGameData()
    {
        Read();
        if (data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else
        {
            isFirstGame = true;
        }
        // 如果第一次开始游戏,初始化数据存进来
        if (isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            skinUnlocked = new bool[vars.skinSpritesList.Count];
            skinUnlocked[0] = true;
            diamondCount = 800;    // 默认给几个钻石
            data = new GameData();  // new一个文件写入，否则找不到可写入
            Save();
        }
        else  // 不是第一次游戏，则读取出来数据
        {
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArrr();
            selectSkin = data.GetSelectSkin();
            skinUnlocked = data.GetSkinUnlocked();
            diamondCount = data.GetDiamondCount();
        }
    }

    /// <summary>
    /// 更新总钻石数量
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAllDiamond(int value)
    {
        diamondCount += value;
        Save();
    }

    /// <summary>
    /// 设置当前选择的皮肤下标
    /// </summary>
    /// <param name="index"></param>
    public void SetSelectedSkin(int index)
    {
        selectSkin = index;
        Save();
    }
    /// <summary>
    /// 获得当前选择的皮肤
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSelectedSkin()
    {
        return selectSkin;
    }

    /// <summary>
    /// 储存数据
    /// </summary>
    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter(); // 序列化
            /*
             *  FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data");   // 不使用using的写法
             *  fs.Close();    // 不使用using的写法
             */

            using (FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data"))     // 写入文件流using System.IO;    ，这里使用using可以自动释放，不使用using则不能
            {
                // 这设置好数据
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetBestScoreArrr(bestScoreArr);
                data.SetSelectSkin(selectSkin);
                data.SetSkinUnlocked(skinUnlocked);
                data.SetDiamondCount(diamondCount);
                // 序列化
                bf.Serialize(fs, data);
            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            /*
             * FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open)；           
             * fs.Close();
             */

            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open))  // 打开文件
            {
                data = (GameData)bf.Deserialize(fs);  // 返回一个gameobject再强转成GameData   ,  如果读取不到会得到一个null

            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        skinUnlocked = new bool[vars.skinSpritesList.Count];
        skinUnlocked[0] = true;
        diamondCount = 0;    // 默认给几个钻石
        Save();
    }

    /// <summary>
    /// 保存前三成绩
    /// </summary>
    public void SaveScore(int score)
    {
        List<int> list = bestScoreArr.ToList();     // 将数组转换成list，需用用到Linq
        // 从大到小排序
        list.Sort((x, y) => (-x.CompareTo(y)));  // list.Sort((x, y) => (x.CompareTo(y)))     从小到大排序
        bestScoreArr = list.ToArray();
        int index = -1;
        for (int i = 0; i < bestScoreArr.Length; i++)
        {
            if (score > bestScoreArr[i])
            {
                index = i;
            }
        }
        if (index == -1)
        {
            return;
        }
        for (int i = bestScoreArr.Length - 1; i > index; i--)
        {
            bestScoreArr[i] = bestScoreArr[i - 1];
        }
        bestScoreArr[index] = score;
        Save();
    }

    /// <summary>
    /// 获取最高分
    /// </summary>
    public int GetBestScore()
    {
        return bestScoreArr.Max();   // 获取数组中的最大值Linq
    }

    /// <summary>
    /// 设置音效开关
    /// </summary>
    /// <param name="value"></param>
    public void SetIsMusicOn(bool value)
    {
        isMusicOn = value;
        Save();
    }
    /// <summary>
    /// 获取音效是否开启
    /// </summary>
    /// <returns></returns>
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }
}
