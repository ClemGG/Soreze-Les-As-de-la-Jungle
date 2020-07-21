using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BolDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    //public Transform limitMinX, limitMaxX;
    public Image ombre;
    [Space(10)]
    [Header("Color : ")]
    [Space(10)]

    public Vector2Int screenMargin = new Vector2Int(200, 150);
    public ColorID colorIndex;
    public AnimationCurve animCurve;
    public float animSpeed = 2f;
    bool isAnimating = false, isDragging = false, startedDragging = false;


    //Order in layer
    int orderBack;
    int orderParticle;
    int orderFront;

    ParticleSystemRenderer psr;
    Canvas imgFront, imgBack;


    //Scripts & Components

    Camera cam;
    Transform t;
    //Rigidbody rb;
    Rigidbody2D rb;
    BoxCollider2D bc;
    EpreuveMixCouleursV2 e;
    RectTransform rt;


    private void Start()
    {
        e = (EpreuveMixCouleursV2)Epreuve.instance;
        cam = Camera.main;
        t = transform;
        //startPoint = t.position;
        rt = GetComponent<RectTransform>();
        startPoint = rt.position;



        bc = GetComponent<BoxCollider2D>();
        //rb = GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        imgFront = GetComponent<Canvas>();
        psr = t.GetChild(0).GetComponent<ParticleSystemRenderer>();
        imgBack = t.GetChild(1).GetComponent<Canvas>();
        psr.GetComponent<ParticleSystem>().Stop();

        orderFront = imgFront.sortingOrder;
        orderParticle = psr.sortingOrder;
        orderBack = imgBack.sortingOrder;

    }



    Vector3 startPoint;
    //Vector3 screenPoint;
    //Vector3 offset;
    //public float dstFromCamera = 4f;


    //void OnMouseDown()
    //{
    //    e.isDragging = true;
    //    screenPoint = Camera.main.WorldToScreenPoint(t.position);
    //    offset = t.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    //}

    //void OnMouseDrag()
    //{
    //    if (e.isDragging && !e.EpreuveFinished)
    //    {
    //        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dstFromCamera);
    //        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    //        curPosition.x = Mathf.Clamp(curPosition.x, limitMinX.position.x, limitMaxX.position.x);

    //        //rb.MovePosition(t.position + curPosition * Time.deltaTime);
    //        t.position = curPosition;
    //    }
    //}

    //private void OnMouseUp()
    //{
    //    e.isDragging = false;
    //    t.position = startPoint;
    //}


    public void LateUpdate()
    {
        if (startedDragging && !e.EpreuveFinished) 
        { 
            ombre.color = new Color(ombre.color.r, ombre.color.g, ombre.color.b, Mathf.Lerp(1f, 0f, Vector3.Distance(rt.position, startPoint) / 1f));
            ombre.transform.position = new Vector3(rt.position.x, ombre.transform.position.y, ombre.transform.position.z);
        }

    }



    Collider2D col;
    private void OnTriggerEnter2D(Collider2D c)
    {
        if(c.CompareTag("mix couleurs/bol resultat") && !e.EpreuveFinished && !e.isAnimating && !e.bolResultat.isAnimating)
        {


            //OnMouseUp();
            StartCoroutine(BolAnim());
        }
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!e.isAnimating && !e.EpreuveFinished && !e.bolResultat.isAnimating)
        {
            OnEndDrag(eventData);
            startedDragging = true;


            e.isDragging = true;


            imgFront.sortingOrder = 21;
            psr.sortingOrder = 22;
            imgBack.sortingOrder = 23;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (e.isDragging && !e.EpreuveFinished && !e.isAnimating && !e.bolResultat.isAnimating)
        {
            Vector2 localPosition = Vector2.zero;


            eventData.position = new Vector3
                (
                    //Mathf.Clamp(eventData.position.x, 200, 1848),
                    //Mathf.Clamp(eventData.position.y, 150, 1386),
                    Mathf.Clamp(eventData.position.x, screenMargin.x, Screen.width - screenMargin.x),
                    Mathf.Clamp(eventData.position.y, screenMargin.y, Screen.height - screenMargin.y),
                    0f
                );


            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, cam, out localPosition);


            rt.position = rt.TransformPoint(localPosition);


        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!e.isAnimating && !e.EpreuveFinished)
        {
            e.isDragging = false;
            rt.position = startPoint;



            imgFront.sortingOrder = orderFront;
            psr.sortingOrder = orderFront;
            imgBack.sortingOrder = orderBack;
        }
    }


    private IEnumerator BolAnim()
    {
        e.validateBtn.interactable = false;
        e.isAnimating = true;
        bc.enabled = false;

        float timer = 0f;
        Vector3 startPos = rt.position;
        Quaternion startRot = rt.rotation;

        if (e.currentCombination.Count < 3)
        {

            while (timer < 1f)
            {
                timer += Time.deltaTime * animSpeed;
                rt.position = Vector3.Lerp(startPos, e.animEndPoint.position, animCurve.Evaluate(timer));
                rt.rotation = Quaternion.Lerp(startRot, e.animEndPoint.rotation, animCurve.Evaluate(timer));
                yield return null;
            }

            e.calebasseFront.sortingOrder = 24;

            AudioManager.instance.Play(e.verseClip);
            psr.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(1f);
            psr.GetComponent<ParticleSystem>().Stop();


            e.AddColorToMix(colorIndex);


            timer = 0f;


            while (timer < 1)
            {
                timer += Time.deltaTime * animSpeed * 3f;
                rt.rotation = Quaternion.Lerp(e.animEndPoint.rotation, Quaternion.identity, animCurve.Evaluate(timer));
                yield return null;
            }
        }
        timer = 0f;


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

        OnEndDrag(null);
    }

    public void ResetComponent()
    {

        bc.enabled = true;
    }

}
