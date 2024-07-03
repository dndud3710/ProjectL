
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoSlot : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler
{
    GameItem item;


    Image curImage;
    public GameItem item_ => item;
    bool enter;

    private void Awake()
    {
       curImage = GetComponent<Image>();
    }
    private void OnEnable()
    {
         curImage.sprite = LoadManager.Instance.transparencySprite;
    }
    public void setImage(GameItem item_)
    {
        item = item_;
        curImage.sprite = item_.itemicon;
    }
    private void OnDisable()
    {
        curImage.sprite = null;
        item = null;
    }
    public void Clear()
    {
        item = null;
        curImage.sprite = LoadManager.Instance.transparencySprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && enter==false)
        {
            OBPool.Instance.GetUI(PlayerUIManager.Instance.ToolTip_Prefab, Vector2.zero, transform)
        .GetComponent<ItemToolTip>().setItem(item, ItemSlot.SlotType.info);
            enter = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (enter)
        {
            OBPool.Instance.UIdespawn(PlayerUIManager.Instance.ToolTip_Prefab);
            enter = false;
        }
    }
}
