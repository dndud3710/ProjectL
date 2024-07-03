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
    public Action<Transform> frameInit; //��ư�� ���콺�� �ö����� �������� Ȱ��ȭ ��Ű������ �̺�Ʈ
    public Action frameoff; //��ư�� ���콺�� ���������� �������� ��Ȱ��ȭ ��Ű������ �̺�Ʈ
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
