using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eulerTest : MonoBehaviour, IPooledObject
{
    public float angleVirage = 20;
    Transform t;
    Rigidbody rb;

    public void OnObjectSpawn()
    {
        if(rb)
            rb.velocity = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        t.Translate(t.forward * 2f * Time.deltaTime);
        t.Rotate(new Vector3(0, angleVirage, 0) * Time.deltaTime);
    }
}
