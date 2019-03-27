
[System.Serializable]   // 加上这个标签才可序列化
public class GameData
{   
    public static bool IsAgainGame = false; // 是否再来一次
    private bool isFirstGame;   // 是否第一次打开游戏
    private bool isMusicOn;  // 是否打开音效
    private int[] bestScoreArr;  // 最佳成绩，前3个
    private int selectSkin;   // 选中皮肤
    private bool[] skinUnlocked; // 是否解锁皮肤
    private int diamondCount;  // 记录钻石数量
    public void SetIsFirstGame(bool isFirstGame)
    {
        this.isFirstGame = isFirstGame;
    }
    public void SetIsMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;
    }
    public void SetBestScoreArrr(int[] bestScoreArr)
    {
        this.bestScoreArr = bestScoreArr;
    }
    public void SetSelectSkin(int selectSkin)
    {
        this.selectSkin = selectSkin;
    }
    public void SetSkinUnlocked(bool[] skinUnlocked)
    {
        this.skinUnlocked = skinUnlocked;
    }
    public void SetDiamondCount(int diamondCount)
    {
        this.diamondCount = diamondCount;
    }
    public bool GetIsFirstGame()
    {
        return isFirstGame;
    }
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }
    public int[] GetBestScoreArrr()
    {
        return bestScoreArr;
    }
    public int GetSelectSkin()
    {
        return selectSkin;
    }
    public bool[] GetSkinUnlocked()
    {
        return skinUnlocked;
    }
    public int GetDiamondCount()
    {
        return diamondCount;
    }
}
