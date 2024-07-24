using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] public FlockManager flockManager;
    private float speed = 5.0f;
    private float originalSpeed;
    private bool turning = false;

    //���F����
    private float neighbourDistance = 5.0f;
    //�������
    private float avoidDistance = 7.0f;
    //��]���x
    private float rotationSpeed = 5.0f;

    //��Q���������
    private float obstacleAvoidanceDistance = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //�Q��̊����͈�
        Bounds bounds = new Bounds(flockManager.transform.position, flockManager.flyLimits * 2);

        //�Փ˂����o
        RaycastHit hit;

        //����
        Vector3 direction = Vector3.zero;

        //�����Q��͈̔͊O�ɏo��, �܂��͑O���ɏ�Q��������
        if (!bounds.Contains(transform.position))
        {
            turning = true;
            direction = (flockManager.transform.position - transform.position).normalized;

            //�X�s�[�h�A�b�v
            speed = originalSpeed * 1.5f;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward, out hit, obstacleAvoidanceDistance))
        {
            // ��Q��������̃^�O�������Ă��邩���m�F
            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                turning = true;
                direction = Vector3.Reflect(this.transform.forward, hit.normal);

                //�X�s�[�h�A�b�v
                speed = originalSpeed * 1.5f;
            }
        }
        else
        {
            turning = false;
            //�X�s�[�h�����ɖ߂�
            speed = originalSpeed;
        }

        //�����ύX�̕K�v��
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
        //���I�u�W�F�N�g�擾
        GameObject[] gos = flockManager.allbird;

        //���S�ʒu
        Vector3 vcentre = Vector3.zero;

        //�Փ�
        Vector3 vavoid = Vector3.zero;

        //���ϑ��x
        float gSpeed = 0.01f;

        //�ڕW�ʒu
        Vector3 goalPos = flockManager.goalPos;

        float dist;

        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                //�����v�Z
                dist = Vector3.Distance(go.transform.position, this.transform.position);

                if (dist <= neighbourDistance)
                {
                    //���F�������̒����W�v
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < avoidDistance)
                    {
                        //����������̒��������
                        vavoid = vavoid + (this.transform.position - go.transform.position).normalized * (avoidDistance - dist);
                    }

                    //���ϑ��x�̌v�Z
                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            //�ڕW�ʒu�Ɍ�����
            vcentre = vcentre / groupSize + (goalPos - this.transform.position).normalized * 2.0f;

            Vector3 direction = (vcentre + vavoid * 2.0f) - transform.position;

            if (direction != Vector3.zero)
            {
                //�����ύX���X���[�Y�ɂ���
                direction = direction.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }
}