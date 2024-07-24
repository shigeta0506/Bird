using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private float speed = 5f;
    private float detectionDistance = 7f;

    void Update()
    {
        // 前方に向かって移動
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Raycastで障害物を検出
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance))
        {
            // 障害物が検出された場合、右か左に直角に回避
            if (hit.collider.CompareTag("Obstacle"))
            {
                float turnAngle = Random.Range(0, 2) == 0 ? 90f : -90f;
                // Y軸回転のみを行う
                transform.Rotate(0, turnAngle, 0);
            }
        }
    }
}
