using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRotEulerAngles : MonoBehaviour
{
    public Transform prefab;
    public float spawnDelay = 1f;
    float timer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < spawnDelay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            ObjectPooler.instance.SpawnFromPool(prefab.name, transform.position, transform.rotation, GameObject.Find("GameObject").transform);
        }
    }
}
