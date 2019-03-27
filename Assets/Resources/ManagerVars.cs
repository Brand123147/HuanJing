using System.Collections.Generic;
using UnityEngine;

//在Assets中创建这个类的菜单功能，创建完成后可注释掉
//[CreateAssetMenu(menuName = "CreateManagerVarsContainer")]
public class ManagerVars : ScriptableObject {
    public static ManagerVars GetManagerVars()
    {
        // 通过resource加载
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    public List<Sprite> bgThemeSpriteList = new List<Sprite>();   // 随机背景
    public List<Sprite> platfromSpriteList = new List<Sprite>();  // 平台主题类型
    public List<Sprite> skinSpritesList = new List<Sprite>();     // 皮肤购买
    public List<Sprite> characterSkinSpriteList = new List<Sprite>();  // 游戏中人物皮肤

    public List<GameObject> commonPlatformGroup = new List<GameObject>();  
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();

    public List<string> skinNameList = new List<string>();  // 皮肤名字
    public List<int> skinPriceList = new List<int>();  // 皮肤价格


    public GameObject normalPlatformPre; // 正常平台
    public GameObject characterPre;  // 人物预制体
    public GameObject spikePlatformRight;  // 右边钉子平台
    public GameObject spikePlatformLeft;  // 左边钉子平台
    public GameObject deathEffect;  // 死亡特效
    public GameObject diamondPre;
    public GameObject skinChooseItemPre; // 选中的皮肤


    //平台递增向左x-0.554f向右x+0.554f,y+0.645f
    public float nextXPos = 0.554f, nextYPos = 0.645f;
    public Vector3 defaultPos = new Vector3(0f, -2.5f, 0); // 人物的初始位置

    public AudioClip jumpClip, fallClip, hitClip, diamondClip, buttonClip;

    public Sprite musicOn, musicOff;
}
