using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] public FlockManager flockManager;
    private float speed = 5.0f;
    private float originalSpeed;
    private bool turning = false;

    private float neighbourDistance = 5.0f; // 視認距離を広げる
    private float avoidDistance = 5.0f; // 回避距離を広げる
    private float rotationSpeed = 5.0f; // 回転速度を低くしてスムーズにする

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        // 群れの活動範囲
        Bounds bounds = new Bounds(flockManager.transform.position, flockManager.flyLimits * 2);

        // 衝突を検出
        RaycastHit hit = new RaycastHit();

        // 方向
        Vector3 direction = Vector3.zero;

        // 鳥が群れの範囲外に出た, または前方に障害物がある
        if (!bounds.Contains(transform.position))
        {
            turning = true;
            direction = (flockManager.transform.position - transform.position).normalized;

            // スピードアップ
            speed = originalSpeed * 1.5f;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);

            // スピードアップ
            speed = originalSpeed * 1.5f;
        }
        else
        {
            turning = false;
            // スピードを元に戻す
            speed = originalSpeed;
        }

        // 方向変更の必要無
        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void ApplyRules()
    {
        // 鳥オブジェクト取得
        GameObject[] gos = flockManager.allbird;

        // 中心位置
        Vector3 vcentre = Vector3.zero;

        // 衝突
        Vector3 vavoid = Vector3.zero;

        // 平均速度
        float gSpeed = 0.01f;

        // 目標位置
        Vector3 goalPos = flockManager.goalPos;

        float dist;

        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);

                if (dist <= neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < avoidDistance)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position).normalized * (avoidDistance - dist);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.position).normalized * 2.0f; // 目標位置に向かう力を強化

            Vector3 direction = (vcentre + vavoid * 2.0f) - transform.position;

            if (direction != Vector3.zero)
            {
                // 方向変更をスムーズにする
                direction = direction.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }
}
