using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Clement.Utilities.Maths;


public class InfiniteRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    float yaw, pitch;
    bool shouldCompute = false, isOverWheel = false;
    Transform t;
    Coroutine co;
    Camera cam;

    [Range(1,20)] public int nbSections = 8;
    public float mouseSensitivity = 5f;
    public AnimationCurve lerpCurve;
    public float lerpSpeed = 2f;



    //Déclarées ici car elles seront aussi réassignées à yaw et pitch juste après StopCoroutine
    float startRot, destRot;
    int curSection, curSectionSound, lastSection;
    bool hasPlayedRouletteSound = false;

    EpreuveNumeroArt e;
    Touch touchOnWheel;


    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        e = (EpreuveNumeroArt)Epreuve.instance;
        cam = Camera.main;

        touchOnWheel = new Touch();
        touchOnWheel.fingerId = -1;
    }

    private void Update()
    {

        if (e.EpreuveFinished)
            return;

        if(Input.touchCount > 0)
        touchOnWheel = Input.GetTouch(0);

        //print(touchOnWheel.deltaPosition);

        if (shouldCompute)
        {
#if UNITY_EDITOR || UNITY_STANDALONE

            yaw += IsUnderWheel(Input.GetAxis("Mouse X") * mouseSensitivity, true);
            pitch += IsUnderWheel(Input.GetAxis("Mouse Y") * mouseSensitivity, false);
#else
            if (Input.touchCount > 0/* && touchOnWheel.fingerId == -1*/)
            {
                Vector2 drag = touchOnWheel.deltaPosition;
                drag.x = Mathf.Clamp(drag.x, -1f, 1f);
                drag.y = Mathf.Clamp(drag.y, -1f, 1f);
                yaw += IsUnderWheel(drag.x * mouseSensitivity, true);
                pitch += IsUnderWheel(drag.y * mouseSensitivity, false);
            }
#endif




            ClampAngle(yaw, 0f, 360f);
            ClampAngle(pitch, 0f, 360f);
            Vector3 targetRot = new Vector3(t.eulerAngles.x, t.eulerAngles.y, yaw + pitch);
            t.eulerAngles = targetRot;
            ClampAngle(t.eulerAngles.z, 0f, 360f);
        }


        if (!isOverWheel && Input.GetMouseButtonUp(0))
        {
            OnPointerUp(null);

        }
        curSection = GetCurrentSection();
        if(lastSection != curSection && isOverWheel)
        {
            AudioManager.instance.Play(e.rouletteClip);
            lastSection = curSection;
        }

    }

    private IEnumerator RoundRotationNearestSection()
    {
        float timer = 0f;
        while(timer < 1f)
        {
            timer += Time.deltaTime * lerpSpeed;
            t.eulerAngles = new Vector3(t.eulerAngles.x, t.eulerAngles.y, Mathf.Lerp(startRot, destRot, lerpCurve.Evaluate(timer)));
            yield return null;
        }
        timer = 0f;
        AudioManager.instance.Play(e.rouletteClip);

    }




    public int GetCurrentSection()
    {
        ComputeDst();
        return curSection;
    }

    private void ComputeDst()
    {
        startRot = t.eulerAngles.z;
        curSection = Mathf.RoundToInt(ClampAngle(startRot, 0f, 360f) * nbSections / 360f);
        destRot = 360f / nbSections * curSection;
    }

    private float IsUnderWheel(float val, bool horizontal)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Vector2 viewportMousePos = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector2 viewportSpritePos = cam.WorldToViewportPoint(t.position);
#elif UNITY_IOS
        Vector2 viewportMousePos = cam.ScreenToViewportPoint(touchOnWheel.position);
        Vector2 viewportSpritePos = cam.WorldToViewportPoint(t.position);
#endif

        return horizontal 
            ? viewportMousePos.y < viewportSpritePos.y ? val : -val
            : viewportMousePos.x < viewportSpritePos.x ? -val : val;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }




    public void OnPointerDown(PointerEventData eventData)
    {
        if (e.EpreuveFinished)
            return;


        if (isOverWheel)
            shouldCompute = true; 
        
        if (co != null)
        {
            StopCoroutine(co);

            ComputeDst();
            yaw += destRot - startRot;
            pitch += destRot - startRot;
        }


        

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (e.EpreuveFinished)
            return;


        shouldCompute = false;

        ComputeDst();
        if(isOverWheel)
        co = StartCoroutine(RoundRotationNearestSection());

        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOverWheel = true;

        //if (Input.touchCount > 0)
        //{
        //    foreach (Touch touch in Input.touches)
        //    {
        //        int id = touch.fingerId;
        //        if (EventSystem.current.IsPointerOverGameObject(id) && touchOnWheel.fingerId == -1 && isOverWheel)
        //        {
        //            touchOnWheel = touch;
        //        }
        //    }
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOverWheel = false;

        //if (Input.touchCount > 0)
        //{
        //    touchOnWheel = new Touch();
        //    touchOnWheel.fingerId = -1;
        //}
    }
}
