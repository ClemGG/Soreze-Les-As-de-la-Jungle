using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        print(transform.eulerAngles);
        transform.LookAt(target);
        print(transform.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
