using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] GameObject differentSpace;

    [SerializeField] float speed = 1;

    float angleDirY;
    float angleDirX;

    void Start()
    {
        // 自身の向きベクトル取得

        angleDirY = transform.eulerAngles.y * (Mathf.PI / 180.0f);
        angleDirX = transform.eulerAngles.x * (Mathf.PI / 180.0f);
    }

    void Update()
    {
        Vector3 dir = new Vector3(-Mathf.Sin(angleDirY), Mathf.Sin(angleDirX), Mathf.Cos(angleDirY) * -Mathf.Cos(angleDirX));

        // 自身の向きに移動
        transform.position += dir * speed * Time.deltaTime;
    }

    private void Explosion()
    {
        Instantiate(differentSpace, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Debug.Log("異空間を生成");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DifferentSpace")
        {
            DifferentSpace ds;
            ds = other.GetComponent<DifferentSpace>();
            ds.Growth();

            Destroy(gameObject);
            return;
        }
        Explosion();
    }
}
