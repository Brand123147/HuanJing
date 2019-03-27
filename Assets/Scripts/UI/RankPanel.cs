using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RankPanel : MonoBehaviour {

    private Button btn_Close;
    private Text score1_Text;
    private Text score2_Text;
    private Text score3_Text;
    private Text[] text_Score;
    private RectTransform ScoreList;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowRankPanel, Show);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowRankPanel, Show);
    }
    private void Start()
    {
        Init();
        gameObject.SetActive(false);
    }
    private void Init()
    {
        btn_Close = transform.Find("btn_Close").GetComponent<Button>();
        btn_Close.onClick.AddListener(OnClickClose);
        score1_Text = transform.Find("ScoreList/Score_1/Text").GetComponent<Text>();
        score2_Text = transform.Find("ScoreList/Score_2/Text").GetComponent<Text>();
        score3_Text = transform.Find("ScoreList/Score_3/Text").GetComponent<Text>();
        ScoreList = transform.Find("ScoreList").GetComponent<RectTransform>();

        ScoreList.localScale = Vector2.zero;

        text_Score =new Text[] { score1_Text , score2_Text , score3_Text };
    }

    private void Show()
    {
        gameObject.SetActive(true);
        ScoreList.DOScale(Vector2.one, 0.5f);
        int[] arr = GameManager.Instance.GetScoreArr();
        for (int i = 0; i < text_Score.Length; i++)
        {
            text_Score[i].text = arr[i].ToString();
        }
    }
    private void OnClickClose()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        ScoreList.DOScale(Vector2.zero, 0.5f).OnComplete(()=> {
            gameObject.SetActive(false);
        });
    }
}
