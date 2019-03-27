using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    public Transform rayDown, rayLeft, rayRight;
    public LayerMask platformLayer, obstacleLayer;  // 检测是哪一层
    private bool isMoveLeft = false;  // 是否向左
    private bool isJumping = false;  //是否在跳跃
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_Body;  // 检测y轴速度
    private SpriteRenderer spriteRenderer;
    private GameObject lastHitGo = null;   // 检测是不是最后遇到的平台
    private bool isMove = false;
    private AudioSource m_AudioSource;

    private void Awake()
    {    
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        my_Body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_AudioSource = GetComponent<AudioSource>();
    }
   
    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());     
    }
  
    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }
    /// <summary>
    /// 音效是否开启
    /// </summary>
    /// <param name="value"></param>
    private void IsMusicOn(bool value)
    {
        m_AudioSource.mute = !value;
    }
    /// <summary>
    /// 改变皮肤
    /// </summary>
    /// <param name="skin"></param>
    private void ChangeSkin(int skinIndex)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[skinIndex];
    }

    /// <summary>
    /// 自己写判断点击事件
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        // 创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        // 向点击位置发射一条射线，检测是否点击到UI
        EventSystem.current.RaycastAll(eventData, raycastResults);   // 发射一条射线，如果检测到UI则返回true
        return raycastResults.Count > 0;
    }
    private void Update()
    {
        // 是否点击到gameobject，如果点击到则return掉，诺物体取消射线接收则该函数失效
        // 点击屏幕左边的暂停按钮不让人物往左跳跃

        /*  用这两个if情况测会有bug，就是死了之后再点一下才会判断死亡，人物还会再跳一下叫一下
         *     if (Application.platform == RuntimePlatform.Android ||
              Application.platform == RuntimePlatform.IPhonePlayer)
          {
              int fingerId = Input.GetTouch(0).fingerId;
              if (EventSystem.current.IsPointerOverGameObject(fingerId))
              {
                  return;
              }
          }        
        if (Application.isMobilePlatform)
        {
            int fingerId = Input.GetTouch(0).fingerId;
            if (EventSystem.current.IsPointerOverGameObject(fingerId))
            {
                return;
            }
        }
        else
          {
              if (EventSystem.current.IsPointerOverGameObject())
              {
                  return;
              }
          }
           */
        if (IsPointerOverGameObject(Input.mousePosition))
        {
            return;
        }
        // 如果游戏结束或者或者游戏暂停或者游戏未开始则不能动
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePause || GameManager.Instance.IsGameStarted == false)
        {
            return;
        }
        if (Input.GetMouseButton(0) && isJumping == false && nextPlatformLeft != Vector3.zero)
        {
            if (isMove == false)
            {
                EventCenter.Broadcast(EventDefine.PlayerMove);
                isMove = true;
            }
            EventCenter.Broadcast(EventDefine.DecidePath);  // 每点击一下生成路径1个
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;  // 获取点击位置
            if (mousePos.x < Screen.width / 2)  // 点击屏幕左边
            {
                isMoveLeft = true;
            }
            if (mousePos.x > Screen.width / 2)  // 点击屏幕右边
            {
                isMoveLeft = false;
            }
            Jump();
        }

        // 游戏结束1
        if (my_Body.velocity.y < -4f && IsRayPlatform() == false &&
            GameManager.Instance.IsGameOver == false)
        {
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            m_AudioSource.PlayOneShot(vars.fallClip);  // 播放掉落声音
            // 调用结束面板
            StartCoroutine("DelayShowGameOverPanel");
        }

        // 游戏结束2
        if (isJumping && IsRayObstacle() && GameManager.Instance.IsGameOver == false)
        {
            // 播放死亡特效
            GameObject go = ObjectPool.Instance.GetDeathEffect();
            go.transform.position = this.transform.position;
            go.SetActive(true);
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;
            m_AudioSource.PlayOneShot(vars.hitClip);  // 播放撞死声音
            // 调用结束面板
            StartCoroutine("DelayShowGameOverPanel");
        }
        if (transform.position.y - Camera.main.transform.position.y < -6 &&
            GameManager.Instance.IsGameOver == false)
        {
            GameManager.Instance.IsGameOver = true;
            m_AudioSource.PlayOneShot(vars.fallClip);  // 播放掉落声音
            StartCoroutine("DelayShowGameOverPanel");
        }
        //Debug.DrawRay(rayDown.position, Vector2.down * .5f, Color.red);
        //Debug.DrawRay(rayLeft.position, Vector2.left * .15f, Color.red);
        //Debug.DrawRay(rayRight.position, Vector2.right * .15f, Color.red);
        IsRayPlatform();
    }

    private IEnumerator DelayShowGameOverPanel()
    {
        yield return new WaitForSeconds(2f);
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
    }

    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, .5f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                if (lastHitGo != hit.collider.gameObject)    // 防止射线一直触发多次加分数
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 是否检测到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, .15f, obstacleLayer);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        m_AudioSource.PlayOneShot(vars.jumpClip);   // 跳跃声音
        if (isMoveLeft)   //向左跳跃
        {
            transform.localScale = new Vector3(-1, 1, 1);  //反转身躯
            //x,y分开写是因为y轴要高一点再往下落的感觉效果看起来像跳跃
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);
            transform.DOMoveY(nextPlatformLeft.y + 0.8f, 0.15f);
        }
        else   // 向右跳跃
        {
            transform.localScale = Vector3.one;  //反转身躯
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
        }
    }

    /// <summary>
    /// 通过碰撞拿到当前平台位置
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
        {
            isJumping = false;
            Vector3 currentPlatformPos = collision.gameObject.transform.position;  // 当前位置

            // 下一个左边平台位置
            nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos, 0);

            // 下一个右边平台位置
            nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos, 0);
        }
        if (collision.collider.tag == "Pickup")
        {
            EventCenter.Broadcast(EventDefine.AddDiamond);
            // 吃到钻石
            m_AudioSource.PlayOneShot(vars.diamondClip);  // 播放吃钻石声音
            collision.gameObject.SetActive(false);
        }
    }

}
