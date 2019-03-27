using UnityEngine;

public class Game_BGTheme : MonoBehaviour {
    private SpriteRenderer m_spriteRenderer;
    private ManagerVars vars;
    private void Awake()
    {
        // 从管理器中随机获取需要的sprite
        vars = ManagerVars.GetManagerVars();
        int ranValue = Random.Range(0, vars.bgThemeSpriteList.Count);

        // 给当前的背景随机切换
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = vars.bgThemeSpriteList[ranValue];
    }
}
