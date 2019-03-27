using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Transform target;
    private Vector3 offset; // 摄像机与目标的偏移量
    private Vector3 velocity; // 平滑跟随速度

    private void Update()
    {
        // target为空，而player不为空则将player的位置赋值给target
        if (target == null && GameObject.FindWithTag("Player") != null)
        {
            target = GameObject.FindWithTag("Player").transform;
            offset = target.position - transform.position;
        }
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            // 平滑插值，使摄像机看起来移动不会那么生硬直接跳过去，让它会有跟随的效果
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x - offset.x, ref velocity.x, .5f);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y - offset.y, ref velocity.y, .5f);
            if (posY > transform.position.y)
            {
                transform.position = new Vector3(posX, posY, transform.position.z);
            }
        }

        //if (target != null)
        //{
        //    float posX = target.position.x;
        //    float posY = target.position.y;
        //    if (posY > transform.position.y)
        //    {
        //        transform.position = new Vector3(posX, posY, transform.position.z);
        //    }
        //}
    }
}
