using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] public FlockManager flockManager;
    private float speed = 5.0f;
    private bool turning = false;

    private float neighbourDistance = 3.0f;
    private float rotationSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //�Q��̊����͈�
        Bounds bounds = new Bounds(flockManager.transform.position, flockManager.flyLimits * 2);

        //�Փ˂����o
        RaycastHit hit = new RaycastHit();

        //����
        Vector3 direction = Vector3.zero;

        //�����Q��͈̔͊O�ɏo��,�܂��͑O���ɏ�Q��������
        if (!bounds.Contains(transform.position))
        {
            turning = true;
            direction = flockManager.transform.position - transform.position;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }
        else
        {
            turning = false;
        }

        //�����ύX�̕K�v��
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
        //���I�u�W�F�N�g�擾
        GameObject[] gos;
        gos = flockManager.allbird;

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
                dist = Vector3.Distance(go.transform.position, this.transform.position);

                if (dist <= neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.position);

            Vector3 direction = (vcentre + vavoid) - transform.position;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),rotationSpeed * Time.deltaTime);
        }
    }
}
