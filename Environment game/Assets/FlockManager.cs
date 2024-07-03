using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class FlockManager : MonoBehaviour
{
    [SerializeField] private GameObject birdPrefab;
    private int numbird = 10;
    public GameObject[] allbird;
    public Vector3 flyLimits = new Vector3(5, 5, 5);

    public Vector3 goalPos;



    // Use this for initialization
    void Start()
    {
        allbird = new GameObject[numbird];
        for (int i = 0; i < numbird; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-flyLimits.x, flyLimits.x),
                                                                Random.Range(-flyLimits.y, flyLimits.y),
                                                                Random.Range(-flyLimits.z, flyLimits.z));
            allbird[i] = (GameObject)Instantiate(birdPrefab, pos, Quaternion.identity);

            allbird[i].GetComponent<Flock>().flockManager = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        goalPos = this.transform.position + new Vector3(Random.Range(-flyLimits.x, flyLimits.x),
                                                       Random.Range(-flyLimits.y, flyLimits.y),
                                                       Random.Range(-flyLimits.z, flyLimits.z));
    }

}
