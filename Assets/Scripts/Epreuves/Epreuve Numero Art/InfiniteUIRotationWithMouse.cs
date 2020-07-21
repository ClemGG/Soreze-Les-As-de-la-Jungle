using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfiniteUIRotationWithMouse : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    float yaw, pitch;
    bool shouldCompute = false, isOverWheel = false;
    Transform t;
    Camera cam;
    Coroutine co;

    public int nbSections = 8;
    public float mouseSensitivity = 5f;
    public AnimationCurve lerpCurve;
    public float lerpSpeed = 2f;

    //Déclarées ici car elles seront aussi réassignées à yaw et pitch juste après StopCoroutine
    float startRot, destRot;
    int curSection;

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        cam = Camera.main;
    }

    private void Update()
    {
        if (shouldCompute)
        {
            yaw += IsUnderWheel(Input.GetAxis("Mouse X") * mouseSensitivity, true);
            pitch += IsUnderWheel(Input.GetAxis("Mouse Y") * mouseSensitivity, false);

            ClampAngle(yaw, 0f, 360f);
            ClampAngle(pitch, 0f, 360f);
            Vector3 targetRot = new Vector3(0f, 0f, yaw + pitch);
            t.eulerAngles = targetRot; 
            ClampAngle(t.eulerAngles.z, 0f, 360f);
        }

        if (!isOverWheel && Input.GetMouseButtonUp(0))
        {
            OnPointerUp(null);

        }
    }

    private IEnumerator RoundRotationNearestSection()
    {
        float timer = 0f;
        while(timer < 1f)
        {
            timer += Time.deltaTime * lerpSpeed;
            t.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(startRot, destRot, lerpCurve.Evaluate(timer)));
            yield return null;
        }
        timer = 0f;

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

        Vector2 viewportMousePos = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector2 viewportpritePos = cam.WorldToViewportPoint(t.position);

        return horizontal
            ? viewportMousePos.y < viewportpritePos.y ? val : -val
            : viewportMousePos.x < viewportpritePos.x ? -val : val;
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
        shouldCompute = false;

        ComputeDst();
        co = StartCoroutine(RoundRotationNearestSection());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOverWheel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOverWheel = false;
    }
}
