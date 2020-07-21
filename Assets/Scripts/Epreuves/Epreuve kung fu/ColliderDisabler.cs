using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class ColliderDisabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("enemy"))
        {
            c.GetComponent<Enemy>().DestroyThisEnemy();
        }
    }
}
