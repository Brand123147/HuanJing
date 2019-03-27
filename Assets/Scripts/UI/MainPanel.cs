using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{

    private Button Button_Start;
    private Button Button_Shop;
    private Button Button_Rank;
    private Button Button_Sound;
    private Button Button_Reset;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowMainPanel, Show);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        Init();

    }
  
    private void Start()
    {

        if (GameData.IsAgainGame)
        {
            // OnClickStart();
             Invoke("OnClickStart", 0.5f);
            //EventCenter.Broadcast(EventDefine.ShowGamePanel);
            //EventCenter.Broadcast(EventDefine.PlayButtonAudio);
            //GameManager.Instance.IsGameStarted = true;
            //gameObject.SetActive(false);
        }
        Sound();
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowMainPanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }
    void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 皮肤更换，这里更换UI皮肤图片
    /// </summary>
    /// <param name="skinIndex"></param>
    void ChangeSkin(int skinIndex)
    {
        transform.Find("Buttons/Button_Shop/Image").GetComponent<Image>().sprite = 
            ManagerVars.GetManagerVars().skinSpritesList[skinIndex];
    }

    /// <summary>
    /// 按钮初始化
    /// </summary>
    void Init()
    {
        Button_Start = transform.Find("Button_Start").GetComponent<Button>();
        Button_Start.onClick.AddListener(OnClickStart);

        Button_Shop = transform.Find("Buttons/Button_Shop").GetComponent<Button>();
        Button_Shop.onClick.AddListener(OnClickShop);

        Button_Rank = transform.Find("Buttons/Button_Rank").GetComponent<Button>();
        Button_Rank.onClick.AddListener(OnClickRank);

        Button_Sound = transform.Find("Buttons/Button_Sound").GetComponent<Button>();
        Button_Sound.onClick.AddListener(OnClickSound);

        Button_Reset = transform.Find("Buttons/Button_Reset").GetComponent<Button>();
        Button_Reset.onClick.AddListener(OnClickReset);
    }
    /// <summary>
    /// 点击开始按钮
    /// </summary>
    void OnClickStart()
    {
        EventCenter.Broadcast(EventDefine.ShowGamePanel);
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);   
        GameManager.Instance.IsGameStarted = true;
        gameObject.SetActive(false);

    }
    /// <summary>
    /// 点击商店按钮
    /// </summary>
    void OnClickShop()
    {
        EventCenter.Broadcast(EventDefine.ShowShopPanel);
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        gameObject.SetActive(false);

    }
    /// <summary>
    /// 点击排行榜按钮
    /// </summary>
    void OnClickRank()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 点击音效按钮
    /// </summary>
    void OnClickSound()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);
        GameManager.Instance.SetIsMusicOn(!GameManager.Instance.GetIsMusicOn());
        Sound();
    }
    private void Sound()
    {
        if (GameManager.Instance.GetIsMusicOn())  // 如果音效开启
        {
            Button_Sound.transform.GetChild(0).GetComponent<Image>().sprite = ManagerVars.GetManagerVars().musicOn;
        }
        else
        {
            Button_Sound.transform.GetChild(0).GetComponent<Image>().sprite = ManagerVars.GetManagerVars().musicOff;

        }
        EventCenter.Broadcast(EventDefine.IsMusicOn, GameManager.Instance.GetIsMusicOn());
    }
    /// <summary>
    /// 点击重置按钮
    /// </summary>
    void OnClickReset()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        EventCenter.Broadcast(EventDefine.ShowResetPanel);
    }

}
