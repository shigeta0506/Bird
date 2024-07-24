using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private float speed = 5f;
    private float detectionDistance = 7f;

    void Update()
    {
        // �O���Ɍ������Ĉړ�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Raycast�ŏ�Q�������o
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance))
        {
            // ��Q�������o���ꂽ�ꍇ�A�E�����ɒ��p�ɉ��
            if (hit.collider.CompareTag("Obstacle"))
            {
                float turnAngle = Random.Range(0, 2) == 0 ? 90f : -90f;
                // Y����]�݂̂��s��
                transform.Rotate(0, turnAngle, 0);
            }
        }
    }
}
