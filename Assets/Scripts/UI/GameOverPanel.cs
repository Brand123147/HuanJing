using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    private Text text_Score, text_BestScore, text_AddDiamondCount;
    private Button Button_Rank, Button_Home, Button_Restart;
    private Image img_New;

    private void Awake()
    {
        text_Score = transform.Find("text_Score").GetComponent<Text>();
        text_BestScore = transform.Find("text_BestScore").GetComponent<Text>();
        text_AddDiamondCount = transform.Find("Diamond/text_AddDiamondCount").GetComponent<Text>();
        Button_Rank = transform.Find("Button_Rank").GetComponent<Button>();
        Button_Rank.onClick.AddListener(OnClickRank);
        Button_Home = transform.Find("Button_Home").GetComponent<Button>();
        Button_Home.onClick.AddListener(OnClickHome);
        Button_Restart = transform.Find("Button_Restart").GetComponent<Button>();
        Button_Restart.onClick.AddListener(OnClickRestart);
        img_New = transform.Find("Img_New").GetComponent<Image>();
        EventCenter.AddListener(EventDefine.ShowGameOverPanel, Show);

    }
    private void Start()
    {
        img_New.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGameOverPanel, Show);
    }
    private void Show()
    {
        if (GameManager.Instance.GetGameScore > GameManager.Instance.GetBestScore())
        {
            text_BestScore.text = "最高分" + GameManager.Instance.GetGameScore;
            img_New.gameObject.SetActive(true);
            img_New.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 5), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            text_BestScore.text = "最高分" + GameManager.Instance.GetBestScore();
            img_New.gameObject.SetActive(false);
        }
        GameManager.Instance.SaveScore(GameManager.Instance.GetGameScore);
        text_Score.text = GameManager.Instance.GetGameScore.ToString();
        text_AddDiamondCount.text = "+" + GameManager.Instance.GetGameDiamond.ToString();
        // 更新钻石数量
        GameManager.Instance.UpdateAllDiamond(GameManager.Instance.GetGameDiamond);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 点击排行榜
    /// </summary>
    private void OnClickRank()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 点击Home
    /// </summary>
    private void OnClickHome()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);
        GameData.IsAgainGame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    /// <summary>
    /// 点击再来一局
    /// </summary>
    AsyncOperation _asyncOperation;
    private void OnClickRestart()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);
        // 获取当前激活的场景再加载一下就是重新加载场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = true;
        
    }
  
}
