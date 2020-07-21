using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Motif : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform photoRectTransform;
    public Transform motifsParent;

    GameObject selectedMotif;

    RectTransform rt;
    Sprite motifSprite;
    Transform t;
    Vector2 startPos;

    int siblingIndex;

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        startPos = t.localPosition;
        siblingIndex = t.GetSiblingIndex();

        rt = GetComponent<RectTransform>();
        motifSprite = GetComponent<Image>().sprite;

    }



    private void MoveMotif()
    {
        t.position = Input.mousePosition;
    }

    public void CreateMotif()
    {
        GameObject go = new GameObject($"motif_{motifSprite.name}", typeof(Image), typeof(MotifGameObject));

        go.GetComponent<Image>().sprite = motifSprite;
        go.GetComponent<RectTransform>().sizeDelta = rt.sizeDelta;
        go.transform.position = t.position;
        go.transform.SetParent(motifsParent);

        go.GetComponent<MotifGameObject>().photoRectTransform = photoRectTransform;

        PanelPhotoButtons.instance.AddMotifToList(go);

    }




    public void OnDrag(PointerEventData eventData)
    {
        if (DialogueEpreuveSystem.instance.isPlaying)
            return;


        //if(PanelPhotoButtons.instance.motifCount < PanelPhotoButtons.instance.maxMotifs)
        //{

            MoveMotif();
            t.SetAsLastSibling();
        //}
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (DialogueEpreuveSystem.instance.isPlaying)
            return;


        //t.SetAsLastSibling();

        //if (PanelPhotoButtons.instance.motifCount < PanelPhotoButtons.instance.maxMotifs)
        // {
        if (photoRectTransform.GetWorldSapceRect(rt.GetWorldSapceRect().size).Contains(rt.GetWorldSapceRect().center) || 
                PanelPhotoButtons.instance.MotifContains(rt.rect.center) &&
                !PanelPhotoButtons.instance.ContainingMotifIsOutOfRect(photoRectTransform.GetWorldSapceRect(rt.GetWorldSapceRect().size), rt.rect.center))
            {
                CreateMotif();
            }

            t.localPosition = startPos;
            t.SetSiblingIndex(siblingIndex);

        //}
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!PanelPhotoButtons.instance)
            return;

        if (!PanelPhotoButtons.instance.showGizmos)
            return;

        rt = GetComponent<RectTransform>();

        Gizmos.color = Color.red;
        Gizmos.DrawCube(photoRectTransform.GetWorldSapceRect(rt.GetWorldSapceRect().size).center, photoRectTransform.GetWorldSapceRect(rt.GetWorldSapceRect().size).size);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(rt.GetWorldSapceRect().center, rt.GetWorldSapceRect().size / 10f);
    }


#endif
}
