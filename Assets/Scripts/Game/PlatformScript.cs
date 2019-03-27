using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 将这个脚本放入平台预制体，调用这个平台脚本随机传入sprite类型
/// </summary>
public class PlatformScript : MonoBehaviour {

    public SpriteRenderer[] spriteRenderers;   // 随机平台颜色

    public GameObject obstacle;  // 随机障碍平台的左右

    private bool startTimer;  // 开启计时器

    private float fallTime;   // 掉落时间

    private Rigidbody2D my_Body;

    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
    }

    public void Init(Sprite sprite, float fallTime, int obstacleDir)
    {
        my_Body.bodyType = RigidbodyType2D.Static;
        this.fallTime = fallTime;
        startTimer = true;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;
        }
        if (obstacleDir == 0)  // 朝右边
        {
            if (obstacle != null)
            {
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y, obstacle.transform.localPosition.z);
            }
        }
    }
    private void Update()
    {
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.PlayerIsMove == false)
        {
            return;
        }
        if (startTimer)
        {
            fallTime -= Time.deltaTime;
            if (fallTime < 0)  // 倒计时结束，开始掉落
            {
                // 掉落
                startTimer = false;
                if (my_Body.bodyType != RigidbodyType2D.Dynamic)
                {
                    my_Body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine("DelayHide");
                }
            }
        }
        if (transform.position.y - Camera.main.transform.position.y < -6)
        {
            StartCoroutine("DelayHide");
        }
    }


    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
 