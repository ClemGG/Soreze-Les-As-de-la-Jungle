using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [SerializeField] LayerMask weakPointMask;
    [SerializeField] Transform raycastStartPoint;
    [SerializeField] float raycastDst = .1f;
    protected Transform t;
    protected Rigidbody rb;
    protected BoxCollider col;
    protected ObjectPooler objectPooler;
    protected EpreuveKungfu epreuve;

    protected WeakPoint target;


    [Space(10)]
    [Header("Enemy : ")]
    [Space(10)]

    [SerializeField] protected float normalSpeed = 3f, slowSpeed = .5f, rotSpeed = 2f;
    protected float speedToUse;
    [SerializeField] protected int rotIntervalleDirection = 45;
    [SerializeField] protected AnimationCurve rotCurve;
    //protected Vector3 startRot;
    protected Quaternion startRot;
    protected bool isOnWeakPoint = false, isRotating = false;
    public bool isCaught = false, isTargeted = false;


    [Space(10)]

    [SerializeField] Transform[] prefabsToSpawnOnDeath;


    // Start is called before the first frame update
    protected void Start()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        objectPooler = ObjectPooler.instance;
        epreuve = (EpreuveKungfu)Epreuve.instance;

        col.isTrigger = true;
        startRot = t.rotation;
    }

    // Update is called once per frame
    void Update()
    {


        if (epreuve.EpreuveFinished || isCaught)
            return;

       

        MoveTowardsWeakPoint();

        if (!isOnWeakPoint)
        {
            speedToUse = normalSpeed;
        }
        else
        {
            speedToUse = slowSpeed;
            MoveInsideWeakPoint();
        }
    }

    protected virtual void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("weak point"))
        {
            isOnWeakPoint = true;
            target = c.GetComponent<WeakPoint>();
            target.LooseLife();
            //startRot = t.localEulerAngles;
            startRot = t.rotation;
        }
    }
    protected void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("weak point"))
        {
            isOnWeakPoint = false;
            c.GetComponent<WeakPoint>().RegainLife();
            target = null;
        }
    }

    //protected void OnMouseDown()
    //{
    //    DestroyThisEnemy();
    //}



    #region Abstract Methods


    protected void MoveInsideWeakPoint()
    {
        if (Physics.Raycast(raycastStartPoint.position, raycastStartPoint.forward * raycastDst, weakPointMask) && !isRotating)
        {
            StartCoroutine(ChangeDirection());
        }
    }

    protected abstract IEnumerator ChangeDirection();

    protected abstract void MoveTowardsWeakPoint();

    public virtual void DestroyThisEnemy()
    {
        for (int i = 0; i < prefabsToSpawnOnDeath.Length; i++)
        {
            objectPooler.SpawnFromPool(prefabsToSpawnOnDeath[i].name, t.position, prefabsToSpawnOnDeath[i].rotation, epreuve.enemiesParent);
        }

        isOnWeakPoint = false;

        if (target)
        {
            target.RegainLife();
            target = null;
        }

        gameObject.SetActive(false);
    }


    #endregion
}
