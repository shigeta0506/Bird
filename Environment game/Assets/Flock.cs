using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] public FlockManager flockManager;
    private float speed = 5.0f;
    private float originalSpeed;
    private bool turning = false;

    //視認距離
    private float neighbourDistance = 5.0f;
    //回避距離
    private float avoidDistance = 7.0f;
    //回転速度
    private float rotationSpeed = 5.0f;

    //障害物回避距離
    private float obstacleAvoidanceDistance = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //群れの活動範囲
        Bounds bounds = new Bounds(flockManager.transform.position, flockManager.flyLimits * 2);

        //衝突を検出
        RaycastHit hit;

        //方向
        Vector3 direction = Vector3.zero;

        //鳥が群れの範囲外に出た, または前方に障害物がある
        if (!bounds.Contains(transform.position))
        {
            turning = true;
            direction = (flockManager.transform.position - transform.position).normalized;

            //スピードアップ
            speed = originalSpeed * 1.5f;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward, out hit, obstacleAvoidanceDistance))
        {
            // 障害物が特定のタグを持っているかを確認
            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                turning = true;
                direction = Vector3.Reflect(this.transform.forward, hit.normal);

                //スピードアップ
                speed = originalSpeed * 1.5f;
            }
        }
        else
        {
            turning = false;
            //スピードを元に戻す
            speed = originalSpeed;
        }

        //方向変更の必要無
        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * 2.0f * Time.deltaTime);
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
        //鳥オブジェクト取得
        GameObject[] gos = flockManager.allbird;

        //中心位置
        Vector3 vcentre = Vector3.zero;

        //衝突
        Vector3 vavoid = Vector3.zero;

        //平均速度
        float gSpeed = 0.01f;

        //目標位置
        Vector3 goalPos = flockManager.goalPos;

        float dist;

        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                //距離計算
                dist = Vector3.Distance(go.transform.position, this.transform.position);

                if (dist <= neighbourDistance)
                {
                    //視認距離内の鳥を集計
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < avoidDistance)
                    {
                        //回避距離内の鳥を避ける
                        vavoid = vavoid + (this.transform.position - go.transform.position).normalized * (avoidDistance - dist);
                    }

                    //平均速度の計算
                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            //目標位置に向かう
            vcentre = vcentre / groupSize + (goalPos - this.transform.position).normalized * 2.0f;

            Vector3 direction = (vcentre + vavoid * 2.0f) - transform.position;

            if (direction != Vector3.zero)
            {
                //方向変更をスムーズにする
                direction = direction.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }
}