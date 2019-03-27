using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ShopPanel : MonoBehaviour {

    private ManagerVars vars;
    private Transform parent;
    private Text text_Name;
    private Text text_DiamondCount;
    private Button btn_Back;
    private Button btn_Buy;
    private Button btn_Select;
    private int selectIndex;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
        vars = ManagerVars.GetManagerVars();
    }

    private void Start()
    {      
        Init();
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }
    void Show()
    {
        gameObject.SetActive(true);
    }
    private void Init()
    {
        parent = transform.Find("ScrollRect/Parent");
        text_Name = transform.Find("text_Name").GetComponent<Text>();
        text_DiamondCount = transform.Find("Diamond/text_DiamondCount").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnClickBack);
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Select.onClick.AddListener(OnClickSelectSkin);
        btn_Buy = transform.Find("btn_Buy").GetComponent<Button>();
        btn_Buy.onClick.AddListener(OnClickBuy);

        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((2 + vars.skinSpritesList.Count) *160, 274f);

        for (int i = 0; i < vars.skinSpritesList.Count; i++)
        {
            GameObject go = Instantiate(vars.skinChooseItemPre, parent);
            // go.GetComponentInChildren<Image>().sprite = vars.skinSpritesList[i];
            if (GameManager.Instance.GetSkinUnlocked(i) == false)
            {
                go.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            }
            else
            {
                go.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }
            go.transform.GetChild(0).GetComponent<Image>().sprite = vars.skinSpritesList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);    // 确定选中人物的位置
        }
        // 打开页面显示当前选中的是哪个皮肤
        parent.transform.localPosition = new Vector3(GameManager.Instance.GetCurrentSelectedSkin() * -160, 0);
    }

    private void Update()
    {
        // parent是960，每一个是160，拆分成6个
        selectIndex = Mathf.RoundToInt(parent.transform.localPosition.x / -160.0f);
        if (Input.GetMouseButtonUp(0))
        {
            // parent.GetComponent<RectTransform>().DOAnchorPos(new Vector3(currentIndex * -160f, 0), 1f);
            // parent.transform.localPosition = new Vector3(currentIndex * -160f, 0);
            parent.DOLocalMoveX(selectIndex * -160f, 0.2f);
        }
        SetItemSize(selectIndex);
        RefreshUI(selectIndex);
    }
    private void SetItemSize(int index)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (index == i)
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
            }
            else
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
        }
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnClickBack()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 点击购买皮肤
    /// </summary>
    private void OnClickBuy()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        int price = int.Parse(btn_Buy.GetComponentInChildren<Text>().text);
        if (price > GameManager.Instance.GetAllDiamond())
        {
            EventCenter.Broadcast<string>(EventDefine.Hint, "钻石不足!!!");
            return;
        }
        GameManager.Instance.UpdateAllDiamond(-price);
        parent.GetChild(selectIndex).GetChild(0).GetComponent<Image>().color = Color.white;
        GameManager.Instance.SetSkinUnlocaked(selectIndex);
    }

    /// <summary>
    /// 点击皮肤同步到main界面
    /// </summary>
    private void OnClickSelectSkin()
    {
        EventCenter.Broadcast(EventDefine.PlayButtonAudio);

        EventCenter.Broadcast(EventDefine.ChangeSkin, selectIndex);
        GameManager.Instance.SetSelectedSkin(selectIndex);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新存储数据
    /// </summary>
    /// <param name="selectIndex"></param>
    private void RefreshUI(int selectIndex)
    {
        text_Name.text = vars.skinNameList[selectIndex];     // 选中人物的名字刷新
        text_DiamondCount.text = GameManager.Instance.GetAllDiamond().ToString();

        // 未解锁
        if (GameManager.Instance.GetSkinUnlocked(selectIndex) == false)
        {
            btn_Select.gameObject.SetActive(false);
            btn_Buy.gameObject.SetActive(true);
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPriceList[selectIndex].ToString();
        }
        else
        {
            btn_Select.gameObject.SetActive(true);
            btn_Buy.gameObject.SetActive(false);
        }
    }
}
