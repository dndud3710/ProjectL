using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
public class SelectButtonFrameEnable : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    Button mybutton;
    public Action<Transform> frameInit; //버튼에 마우스가 올라갔을때 프레임을 활성화 시키기위한 이벤트
    public Action frameoff; //버튼에 마우스가 떨어졌을때 프레임을 비활성화 시키기위한 이벤트
    private void Awake()
    {
        mybutton = GetComponent<Button>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        frameInit?.Invoke(transform);
    }
    private void OnDisable()
    {
        mybutton.onClick.RemoveAllListeners();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        frameoff?.Invoke();
    }
}
