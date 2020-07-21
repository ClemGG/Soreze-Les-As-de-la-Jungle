using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fourmi : Enemy
{
    [Space(10)]
    [Header("IA : ")]
    [Space(10)]

    [SerializeField] int moveRotIntervalle = 20;
    [SerializeField] float delayBetweenMoveRot = 2f;
    float rotMoveTimer;



    public override void DestroyThisEnemy()
    {
        base.DestroyThisEnemy();
        EpreuveKungfuStatic.nbFourmisInScene--;

    }





    protected override void MoveTowardsWeakPoint()
    {

        if (target != null)
        {

            if (target.destroyed)
            {
                DestroyThisEnemy();
                return;
            }
        }

        t.Translate(t.forward * speedToUse * Time.deltaTime, Space.World);

        if(rotMoveTimer < delayBetweenMoveRot)
        {
            rotMoveTimer += Time.deltaTime;
        }
        else
        {
            rotMoveTimer = 0f;
            //StartCoroutine(Rot());
        }


    }






    //private IEnumerator Rot()
    //{
    //    float rotTimer = 0f;

    //    float alea = Random.Range(-moveRotIntervalle, moveRotIntervalle);
    //    while (rotTimer < 1f)
    //    {
    //        rotTimer += Time.deltaTime * rotSpeed;
    //        t.localEulerAngles = Vector3.Lerp(startRot, new Vector3(startRot.x, startRot.y + alea, startRot.z), rotCurve.Evaluate(rotTimer));
    //        yield return null;
    //    }

    //    startRot = t.localEulerAngles;
    //}





    protected override IEnumerator ChangeDirection()
    {

        yield return null;
        //isRotating = true;
        //float rotTimer = 0f;


        //float alea = Random.Range(-rotIntervalleDirection, rotIntervalleDirection);

        //while (rotTimer < 1f)
        //{
        //    rotTimer += Time.deltaTime * rotSpeed;
        //    t.localEulerAngles = Vector3.Lerp(startRot, new Vector3(startRot.x, 180 + alea, startRot.z), rotCurve.Evaluate(rotTimer));
        //    yield return null;
        //}

        //startRot = t.localEulerAngles;
        //isRotating = true;
    }
}
