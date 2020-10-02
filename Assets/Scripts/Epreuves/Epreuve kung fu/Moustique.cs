using System.Collections;
using UnityEngine;

public class Moustique : Enemy, IPooledObject
{

    #region Variables


    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    SpriteRenderer rend;
    Animator anim;

    Transform camT, rendT;
    SpawnerMoustique sm;
    Vector3 spawnerPos;




    [Space(10)]
    [Header("Movement : ")]
    [Space(10)]


    [SerializeField] float normalSpeed = 3f, rotSpeed = 2f;
    [SerializeField] int rotIntervalleDirection = 45;
    [SerializeField] AnimationCurve rotCurve;
    Quaternion startRot;

    [Space(10)]

    public Vector2 delaysBetweenRotations = new Vector3(.5f, 1f);
    float randomDelay, timer;


    public LayerMask wallMask;
    public float dstBeforeRespawn = 12f;







    #endregion



    #region Mono


    protected override void Start()
    {
        base.Start();

        rend = t.GetComponentInChildren<SpriteRenderer>();
        anim = rend.GetComponent<Animator>();
        rendT = rend.transform;
        camT = Camera.main.transform;

        spawnerPos = sm.transform.position;

    }




    private void Update()
    {
        //pour garder le moustique face au joueur
        if (rendT && camT) rendT.LookAt(camT.position);

        //On arrête l'animation du moustique si on ne le voit pas
        anim.enabled = isVisible = rend.isVisible;


        //Déplacement et changement de direction
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



        //Pour replacer le moustique s'il s'éloigne trop
        float dstToCenter = (t.position - spawnerPos).sqrMagnitude;
        if (dstToCenter > dstBeforeRespawn * dstBeforeRespawn && !isRespawning && !isGrowing && !isTargeted)
        {
            StartCoroutine(RespawnThisEnemy());
        }
    }



    #endregion



    #region Enemy





    public override void DestroyThisEnemy()
    {
        EpreuveKungfuStatic.nbMoustiquesInScene--;

        base.DestroyThisEnemy();
        epreuve.CheckVictory();


    }





    protected override IEnumerator ChangeDirection()
    {
        float rotTimer = 0f;

        float x = Random.Range(-rotIntervalleDirection, rotIntervalleDirection);
        float y = Random.Range(-rotIntervalleDirection, rotIntervalleDirection);


        while (rotTimer < 1f)
        {
            rotTimer += Time.deltaTime * rotSpeed;
            t.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(new Vector3(x, t.localEulerAngles.y + y, 0)), rotCurve.Evaluate(rotTimer));
            yield return null;
        }

        startRot = t.rotation;

    }




    public void OnObjectSpawn()
    {
        base.Start();
        sm = FindObjectOfType<SpawnerMoustique>();


        randomDelay = Random.Range(delaysBetweenRotations.x, delaysBetweenRotations.y);

        if (!isTargeted)
            StartCoroutine(GrowScale());
        else
            t.localScale = Vector3.one;
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
            sm.Respawn(t);

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.zero, scale, timer);
            yield return null;
        }

        isRespawning = false;

    }


    #endregion
}
