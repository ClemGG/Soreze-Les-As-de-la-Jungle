using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moustique : Enemy, IPooledObject
{


    [Space(10)]
    [Header("IA : ")]
    [Space(10)]

    public LayerMask wallMask;
    public float dstBeforeRespawn = 12f;
    SpawnerMoustique sm;
    bool isRespawning, isGrowing;

    [Space(10)]

    public Vector2 delaysBetweenRotations = new Vector3(.5f, 1f);
    float randomDelay, timer;


    protected bool notCaughtYet = true;

    public override void DestroyThisEnemy()
    {
        EpreuveKungfuStatic.nbMoustiquesInScene--;

        epreuve.CheckVictory();

        base.DestroyThisEnemy();

    }

    protected override void MoveTowardsWeakPoint()
    {

        if (target != null)
        {
            //if (target.destroyed)
            //{
            //    DestroyThisEnemy();
            //    return;
            //}


            //t.Translate((target.transform.position - t.position).normalized * speedToUse * Time.deltaTime, Space.World);
            //t.LookAt(target.transform);

        }
        //t.Translate(t.forward * speedToUse * Time.deltaTime, Space.World);


    }

    private void Update()
    {
        if (isCaught && notCaughtYet)
        {
            ObjectPooler.instance.SpawnFromPool("hit", t.position, Quaternion.identity);
            notCaughtYet = false;

        }


        if (isCaught)
        {
            StopAllCoroutines();
            return;
        }


        if (timer < randomDelay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            randomDelay = Random.Range(delaysBetweenRotations.x, delaysBetweenRotations.y);
            StartCoroutine(ChangeDirection());
        }
        t.Translate(t.forward * normalSpeed * Time.deltaTime, Space.World);

        Vector3 spawnerPos = sm.transform.position;
        float dstToCenter = (t.position - spawnerPos).sqrMagnitude;

        if (dstToCenter > dstBeforeRespawn * dstBeforeRespawn && !isRespawning && !isGrowing)
        {
            StartCoroutine(RespawnThisEnemy());
        }
    }

    private void FixedUpdate()
    {
        if (isCaught)
            return;

        Ray r = new Ray(t.position, t.forward);
        bool restore = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        if (!isRotating && Physics.Raycast(r, out RaycastHit hit, 3f, wallMask, QueryTriggerInteraction.Collide))
        {
            //print(hit.collider.name);
            timer = 0f;
            randomDelay = Random.Range(delaysBetweenRotations.x, delaysBetweenRotations.y);

            t.eulerAngles.Scale(-Vector3.one);
            //StartCoroutine(ReverseDirection());
        }
        Physics.queriesHitBackfaces = restore;
    }




    protected override IEnumerator ChangeDirection()
    {
        isRotating = true;
        float rotTimer = 0f;

        float x = Random.Range(-rotIntervalleDirection, rotIntervalleDirection);
        float y = Random.Range(-rotIntervalleDirection, rotIntervalleDirection);
        //float z = Random.Range(-rotIntervalleDirection, rotIntervalleDirection);


        while (rotTimer < 1f)
        {
            rotTimer += Time.deltaTime * rotSpeed;
            //t.rotation = Quaternion.Euler(Vector3.Lerp(startRot.eulerAngles, new Vector3(x, t.localEulerAngles.y + y, 0), rotCurve.Evaluate(rotTimer)));
            t.rotation = Quaternion.Lerp(startRot, Quaternion. Euler(new Vector3(x, t.localEulerAngles.y + y, 0)), rotCurve.Evaluate(rotTimer));
            yield return null;
        }

        //startRot = t.localEulerAngles;
        startRot = t.rotation;

        isRotating = false;
    }






    public void OnObjectSpawn()
    {
        base.Start();
        isRotating = false;
        isOnWeakPoint = false;
        sm = FindObjectOfType<SpawnerMoustique>();


        randomDelay = Random.Range(delaysBetweenRotations.x, delaysBetweenRotations.y);

        //List<WeakPoint> ts = new List<WeakPoint>();

        //for (int i = 0; i < epreuve.weakPoints.Length; i++)
        //{
        //    if (!epreuve.weakPoints[i].destroyed)
        //        ts.Add(epreuve.weakPoints[i]);
        //}

        //target = ts[Random.Range(0, ts.Count)];

        StartCoroutine(GrowScale());
    }

    private IEnumerator GrowScale()
    {
        isGrowing = true;
        float timer = 0f;
        Vector3 scale = t.localScale;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.zero, scale, timer);
            yield return null;
        }

        isGrowing = false;
    }


    private IEnumerator RespawnThisEnemy()
    {
        isRespawning = true;
        float timer = 1f;
        Vector3 scale = t.localScale;


        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.zero, scale, timer);
            yield return null;
        }

        if(!isCaught)
            yield return StartCoroutine(sm.Respawn(t));

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.zero, scale, timer);
            yield return null;
        }

        isRespawning = false;

    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {

//        col = GetComponent<BoxCollider>();

//        Gizmos.color = new Color(1, 0, 0, .3f);
//        Gizmos.DrawCube(t.position, col.size);
//    }

//#endif
}
