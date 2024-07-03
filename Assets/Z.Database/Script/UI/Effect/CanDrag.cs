using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    RectTransform tr;
    private void Awake()
    {
        tr=transform.parent.GetComponent<RectTransform>();
    }

    bool ondrag;
    Vector2 mousePos;
    Vector2 res;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (tr != null)
            mousePos = tr.position;
        ondrag = true;
        Vector2 difpos = Input.mousePosition;
        res = difpos - new Vector2(tr.position.x, tr.position.y);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (tr != null)
            tr.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ondrag = false;
    }
    
    private void Update()
    {
        if (ondrag)
        {
            Vector2 mpos = Input.mousePosition;
            mousePos= mpos - res;
        }
    }

}
