using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ResetPanel : MonoBehaviour
{
    private Image Bg;
    private RectTransform Bg2;
    private Button btn_No;
    private Button btn_Yes;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowResetPanel, Show);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowResetPanel, Show);
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        btn_No = transform.Find("Bg2/btn_No").GetComponent<Button>();
        btn_No.onClick.AddListener(OnClickNo);
        btn_Yes = transform.Find("Bg2/btn_Yes").GetComponent<Button>();
        btn_Yes.onClick.AddListener(OnClickYes);
        Bg = transform.Find("Bg").GetComponent<Image>();
        Bg2 = transform.Find("Bg2").GetComponent<RectTransform>();
        Bg.color = new Color(Bg.color.r, Bg.color.g, Bg.color.b, 0);
        Bg2.localScale = new Vector2(0, 0);
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        Bg.DOColor(new Color(Bg.color.r, Bg.color.g, Bg.color.b, 0.4f), .5f);
        Bg2.DOScale(new Vector2(1, 1), .5f);
    }
    private void OnClickYes()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        GameManager.Instance.ResetData();
        // 重置游戏之后重新加载场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnClickNo()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        Bg.DOColor(new Color(Bg.color.r, Bg.color.g, Bg.color.b, 0), .3f);
        Bg2.DOScale(Vector2.zero, .3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

    }
}
