using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileJunior : MonoBehaviour, IPooledObject
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    Transform t;
    EpreuveComparaison e;
    Rigidbody rb;

    [Space(10)]
    [Header("Projectile : ")]
    [Space(10)]

    [HideInInspector] public int ID;
    public float speed = 10f;
    public Vector3 target;


    // Update is called once per frame
    void FixedUpdate()
    {
        t.position = Vector3.MoveTowards(t.position, target, Time.deltaTime * speed);
        t.LookAt(target);
    }


    public void SetProjectile(int index, Vector3 newTarget)
    {
        ID = index;
        target = newTarget;
    }


    private void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag("comparaison/painting"))
        {
            ComparaisonSprite cs = c.collider.GetComponent<ComparaisonSprite>();
            e.HighlightDifferences(cs.ID == ID ? ID : -1);

            if(cs.ID == ID)
                e.CheckVictory(ID);

        }
        else
        {
            e.HighlightDifferences(-1);
        }

        Animator splashAnim = ObjectPooler.instance.SpawnFromPool("splash junior", t.position, Quaternion.identity).GetComponent<Animator>();
        splashAnim.Play("correct");

        AudioManager.instance.Play(e.impactClip);
        AudioManager.instance.Play(e.posePapierClip);

        e.isShooting = false;
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        if (!t) t = transform;
        if (!e) e = (EpreuveComparaison)Epreuve.instance;
        if (!rb) rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;

    }
}
