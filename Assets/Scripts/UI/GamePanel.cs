using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour {
    private Text Text_DiamondCount;
    private Text Text_Score;
    private Button Button_Play;
    private Button Button_Pause;

    private void Awake()
    {
        // 添加监听GamePanel显隐
        EventCenter.AddListener(EventDefine.ShowGamePanel, Show);
        EventCenter.AddListener<int>(EventDefine.UpdateScoreText, UpdateScoreText);
        EventCenter.AddListener<int>(EventDefine.UpdateDiamondText, UpdateDiamondText);    
       
        Init();
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        // 移除游戏panel
        EventCenter.RemoveListener(EventDefine.ShowGamePanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.UpdateScoreText, UpdateScoreText);
        EventCenter.RemoveListener<int>(EventDefine.UpdateDiamondText, UpdateDiamondText);

    }
    /// <summary>
    /// 游戏界面显示方法
    /// </summary>
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Init()
    {
        Text_DiamondCount = transform.Find("Diamond/Text_DiamondCount").GetComponent<Text>();
        Text_Score = transform.Find("Text_Score").GetComponent<Text>();
        Button_Play = transform.Find("Button_Play").GetComponent<Button>();
        Button_Play.onClick.AddListener(OnClickPlay);
        Button_Play.gameObject.SetActive(false);
        Button_Pause = transform.Find("Button_Pause").GetComponent<Button>();
        Button_Pause.onClick.AddListener(OnClickPause);

    }
  

    /// <summary>
    /// 游戏开始按钮
    /// </summary>
    private void OnClickPlay()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        Button_Play.gameObject.SetActive(false);
        Button_Pause.gameObject.SetActive(true);
        // 开始游戏
        GameManager.Instance.IsGamePause = false;
        Time.timeScale = 1;
    }
    /// <summary>
    /// 暂停按钮
    /// </summary>
    private void OnClickPause()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        Button_Play.gameObject.SetActive(true);
        Button_Pause.gameObject.SetActive(false);
        // 游戏暂停
        GameManager.Instance.IsGamePause = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// 更新游戏成绩
    /// </summary>
    private void UpdateScoreText(int score)
    {
        Text_Score.text = score.ToString();
    }

    /// <summary>
    /// 更新游戏钻石
    /// </summary>
    private void UpdateDiamondText(int diamondNum)
    {
        Text_DiamondCount.text = diamondNum.ToString();
    }
}
