using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BolDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Image ombre;
    [Space(10)]
    [Header("Color : ")]
    [Space(10)]

    public Vector2Int screenMargin = new Vector2Int(200, 150);
    public ColorID colorIndex;
    public AnimationCurve animCurve;
    public float animSpeed = 2f;
    bool startedDragging = false;
    [HideInInspector] public bool introDone = false;



    //Order in layer
    int orderBack;
    int orderFront;

    ParticleSystemRenderer psr;
    Canvas imgFront, imgBack;


    //Scripts & Components

    Camera cam;
    Transform t;
    Rigidbody2D rb;
    BoxCollider2D bc;
    EpreuveMixCouleursV2 e;
    RectTransform rt;


    [HideInInspector] public Vector3 startPoint;





    private void Start()
    {
        e = (EpreuveMixCouleursV2)Epreuve.instance;
        cam = Camera.main;
        t = transform;
        rt = GetComponent<RectTransform>();
        startPoint = rt.position;



        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        imgFront = GetComponent<Canvas>();
        psr = t.GetChild(0).GetComponent<ParticleSystemRenderer>();
        imgBack = t.GetChild(1).GetComponent<Canvas>();
        psr.GetComponent<ParticleSystem>().Stop();

        orderFront = imgFront.sortingOrder;
        orderBack = imgBack.sortingOrder;

    }



    //Change la couleur de l'ombre
    public void ChangeShadowColor()
    {
        if (startedDragging && !e.EpreuveFinished) 
        { 
            ombre.color = new Color(ombre.color.r, ombre.color.g, ombre.color.b, Mathf.Lerp(1f, 0f, Vector3.Distance(rt.position, startPoint) / 1f));
            ombre.transform.position = new Vector3(rt.position.x, ombre.transform.position.y, ombre.transform.position.z);
        }

    }



    //Si le bol touche la calebasse, jouer l'animation de remplissage
    private void OnTriggerEnter2D(Collider2D c)
    {
        if(c.CompareTag("mix couleurs/bol resultat") && !e.EpreuveFinished && !e.isAnimating && !e.bolResultat.isAnimating)
        {

            e.stopIntroAnim = true;   //Utilisé uniquement dans l'anim d'intro
            StartCoroutine(BolAnim());
        }
    }



    //initialisation des paramètres au début du drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!e.isAnimating && introDone && !e.EpreuveFinished && !e.bolResultat.isAnimating)
        {
            OnEndDrag(eventData);
            startedDragging = true;


            e.isDragging = true;
            //print($"begin drag : {name}");

            imgFront.sortingOrder = 21;
            psr.sortingOrder = 22;
            imgBack.sortingOrder = 23;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Placer le bol sur la position de la souris
        //(Converti en pixels car c'est un canvas) 
        if (e.isDragging && introDone && !e.EpreuveFinished && !e.isAnimating && !e.bolResultat.isAnimating)
        {
            //print($"drag : {name}");

            MoveBolAnim(eventData.position);

        }
    }


    //réinitialisation des paramètres à la fin du drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!e.isAnimating && !e.EpreuveFinished)
        {
            e.isDragging = false;
            rt.position = startPoint;

            ChangeShadowColor();

            //print($"end drag : {name}");

            imgFront.sortingOrder = orderFront;
            psr.sortingOrder = orderFront;
            imgBack.sortingOrder = orderBack;
        }
    }


    public void MoveBolAnim(Vector3 pos)
    {
        pos = new Vector3
                (
                    Mathf.Clamp(pos.x, screenMargin.x, Screen.width - screenMargin.x),
                    Mathf.Clamp(pos.y, screenMargin.y, Screen.height - screenMargin.y),
                    0f
                );


        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, pos, cam, out Vector2 localPosition);
        rt.position = rt.TransformPoint(localPosition);

        ChangeShadowColor();

    }

    private IEnumerator BolAnim()
    {
        e.validateBtn.interactable = false;
        e.clearButton.interactable = false;
        e.isAnimating = true;
        bc.enabled = false;

        float timer = 0f;
        Vector3 startPos = rt.position;
        Quaternion startRot = rt.rotation;


        //Si la calebasse n'est pas pleine, on joue l'animation de remplissage
        if (e.currentCombination.Count < 3)
        {

            //On déplace et tourne le bol pour qu'il soit au dessus de la calebasse
            while (timer < 1f)
            {
                timer += Time.deltaTime * animSpeed;
                rt.position = Vector3.Lerp(startPos, e.animEndPoint.position, animCurve.Evaluate(timer));
                rt.rotation = Quaternion.Lerp(startRot, e.animEndPoint.rotation, animCurve.Evaluate(timer));
                yield return null;
            }

            e.calebasseFront.sortingOrder = 24;


            //On joue l'effet de liquide
            AudioManager.instance.Play(e.verseClip);
            psr.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(1f);
            psr.GetComponent<ParticleSystem>().Stop();


            e.AddColorToMix(colorIndex);


            timer = 0f;

            //On ramène sa rotation à la normale
            while (timer < 1)
            {
                timer += Time.deltaTime * animSpeed * 3f;
                rt.rotation = Quaternion.Lerp(e.animEndPoint.rotation, Quaternion.identity, animCurve.Evaluate(timer));
                yield return null;
            }
        }
        timer = 0f;


        //On ramène ensuite le bol à son point de départ
        while (timer < 1f)
        {
            timer += Time.deltaTime * animSpeed;
            rt.position = Vector3.Lerp(e.animEndPoint.position, startPoint, animCurve.Evaluate(timer));
            yield return null;
        }

        bc.enabled = true;
        e.calebasseFront.sortingOrder = -1;
        e.isAnimating = false;
        e.validateBtn.interactable = e.currentCombination.Count > 1;
        e.clearButton.interactable = true;

        OnEndDrag(null);
    }

    public void ResetComponent()
    {

        bc.enabled = true;
    }

}
