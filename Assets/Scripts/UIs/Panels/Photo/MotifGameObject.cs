using UnityEngine;
using UnityEngine.EventSystems;

public class MotifGameObject : MonoBehaviour, IPointerDownHandler, IDragHandler, IDropHandler
{
    Vector3 startPos;
    Transform t;
    RectTransform rt;

    public RectTransform photoRectTransform;


    [Space(10)]

    public float delayBetweenClicks = .5f;
    public float distanceToDrag = 5f;
    float timer;

    int nbClicks = 0;
    bool shouldCount = true;



    private void Start()
    {
        startPos = transform.position;
        t = transform;
        rt = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float drag = (eventData.position - (Vector2)t.position).sqrMagnitude;

        if (drag > distanceToDrag * distanceToDrag)
        {
            t.position = Input.mousePosition.ClampedInsideOfRect(photoRectTransform.GetWorldSapceRect(rt.GetWorldSapceRect().size));
            nbClicks = 0;
        }

        //t.parent.SetAsLastSibling();
        t.SetAsLastSibling();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!photoRectTransform.GetWorldSapceRect().Contains(rt.GetWorldSapceRect().center))
        {
            t.position = startPos;
        }

        startPos = transform.position;
        //t.parent.SetSiblingIndex(3);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (shouldCount)
        {
            nbClicks++;
        }
        else
        {
            nbClicks = 0;
        }

        if (nbClicks == 0 && !shouldCount)
        {
            nbClicks++;
            shouldCount = true;
        }


        if (nbClicks == 2)
            PanelPhotoButtons.instance.RemoveMotifFromList(gameObject);
    }


    private void Update()
    {
        if (shouldCount)
        {
            if(timer < delayBetweenClicks)
            {
                timer += Time.deltaTime;
            }
            else
            {
                shouldCount = false;
            }
        }
        else
        {
            timer = 0f;
        }
    }
}
