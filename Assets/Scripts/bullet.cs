using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] float speed = 1;

    void Start()
    {
        
    }

    void Update()
    {

        // 自身の向きベクトル取得
        float angleDirY = transform.eulerAngles.y * (Mathf.PI / 180.0f);
        float angleDirX = transform.eulerAngles.x * (Mathf.PI / 180.0f);


        Vector3 dir = new Vector3(-Mathf.Sin(angleDirY), Mathf.Sin(angleDirX), Mathf.Cos(angleDirY) * -Mathf.Cos(angleDirX));

        // 自身の向きに移動
        transform.position += dir * speed * Time.deltaTime;

    }
}
