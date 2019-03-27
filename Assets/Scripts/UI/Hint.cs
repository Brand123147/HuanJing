using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Hint : MonoBehaviour {
    private Image img_Bg;
    private Text text_Hint;
    // Use this for initialization
    private void Awake()
    {
        EventCenter.AddListener<string>(EventDefine.Hint, Show);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.Hint, Show);
    }
    void Start () {
        Init(); 

    }
	void Init()
    {
        img_Bg = GetComponent<Image>();
        text_Hint = GetComponentInChildren<Text>();
        img_Bg.color = new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0);
        text_Hint.color = new Color(text_Hint.color.r, text_Hint.color.g, text_Hint.color.b, 0);
    }
    private void Show(string text)
    {
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f);
        StopCoroutine("Delay");
        text_Hint.text = text;   
        transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear).OnComplete(()=> {
            StartCoroutine("Delay");
        });
        img_Bg.DOColor( new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0.6f), 0.5f);
        text_Hint.DOColor(new Color(text_Hint.color.r, text_Hint.color.g, text_Hint.color.b, 1), 0.5f);
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        transform.DOLocalMoveY(50, 0.5f).SetEase(Ease.Linear);
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0f), 0.3f);
        text_Hint.DOColor(new Color(text_Hint.color.r, text_Hint.color.g, text_Hint.color.b, 0f), 0.3f);
    }
}
