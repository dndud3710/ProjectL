using DG.Tweening;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{

    public enum SlotType
    {
        Equip,
        Item,
        Quickslot,
        info
    }   
    [SerializeField] SlotType slottpye;
    public int id {  get; private set; }
    public GameItem item { get; private set; }

    
    public Image Icon { get; private set; }
    private Transform IconPos;
    private Image AmountTexture;

    public TextMeshProUGUI amount { get; private set; }
    public TextMeshProUGUI CoolTime { get; private set; }

    //Drag 관련 기능
    bool ondrag;
    Vector2 InitPoint;
    Vector2 mousePos;
    Sprite transparencyIcon;
    private void Awake()
    {
        id = -1;
        Icon= transform.Find("Icon").GetComponent<Image>();
        AmountTexture = transform.Find("TextMaskImage").GetComponent<Image>();
        transparencyIcon = Icon.sprite;
        Icon.transform.GetComponent<RectTransform>().SetAsLastSibling();
        IconPos = transform.Find("Icon").transform;
        amount = transform.Find("Count").GetComponent<TextMeshProUGUI>();
        CoolTime = transform.Find("CoolTime").GetComponent<TextMeshProUGUI>();
        NoItemDisable();
        CoolTime.gameObject.SetActive(false);
    }

    
    private void OnEnable()
    {
        Init();

    }
    public void setItem(GameItem item_)
    {
        //1. Equip일경우 Equip만 할당가능
        //2. Item일경우 모든 item가능
        //3. Quickslot일때는 useitem만 가능
        if (item_ != null)
        {
            switch (slottpye)
            {
                case SlotType.Item:
                    break;
                case SlotType.Quickslot:
                    if (item_.type != Itemtype.use) 
                        return;
                        break;
                case SlotType.Equip:
                    if (item_.type != Itemtype.equip)
                        return;
                    break;
            }
            this.item = item_;
            Init();
        }
    }

    public void setId(int id)
    {
        this.id = id;
    }
    public SlotType getSlotType()
    {
        return slottpye;
    }
    //나중에 Inventory, equip 에서 자동으로가져오게끔 설계해야함
    public void Init()
    {
        infoSlotReturn();
        if (id != -1)
        {
            item = Inventory.Instance.getItem(id,slottpye);
        }
        if (item != null)
        {
            if (slottpye != SlotType.Equip)
            {
                AmountTexture.gameObject.SetActive(true);
                amount.text = item.amount.ToString();
            }
            Icon.sprite = item.itemicon;
        }
        else
        {
            NoItemDisable();
        }
    }
    void NoItemDisable()
    {
        Icon.sprite = transparencyIcon;
        amount.text = "";
        AmountTexture.gameObject.SetActive(false);

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        infoSlotReturn();

        InitPoint = IconPos.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        infoSlotReturn();

        if (IconPos != null)
             IconPos.position= Input.mousePosition;
        ondrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        infoSlotReturn();
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            for (int i = 0; i < results.Count; i++)
            {
            if (results[i].gameObject.layer == LayerMask.NameToLayer("UI") &&
                results[i].gameObject.name == "Background")
            {
                ItemSlot islot = results[i].gameObject.transform.parent.GetComponent<ItemSlot>();
               
                Inventory.Instance.UIswapItem(id, islot.id,slottpye,islot.getSlotType());
                islot.Init();
                Init();
            }
            }
            IconPos.position = InitPoint;
        ondrag = false;
    }
    private void Update()
    {
        infoSlotReturn();
    }
    /// <summary>
    /// GetUI로 바뀌면서 obpool에서 바로처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            if (PlayerUIManager.Instance.gameObject.activeSelf) 
                OBPool.Instance.GetUI(PlayerUIManager.Instance.ToolTip_Prefab, Vector2.zero,PlayerUIManager.Instance.transform).GetComponent<ItemToolTip>().setItem(item,slottpye);
            else if(SimulationUI.Instance.gameObject.activeSelf)
                OBPool.Instance.GetUI(PlayerUIManager.Instance.ToolTip_Prefab, Vector2.zero, SimulationUI.Instance.transform).GetComponent<ItemToolTip>().setItem(item, slottpye);
        }
        //if (slottpye == SlotType.Item)
        //{
        //    OBPool.Instance.GetUI(PlayerUIManager.Instance.ToolTip_Prefab, transform.parent).GetComponent<ItemToolTip>().setItem(item);

        //    PlayerUIManager.Instance.inventoryUI.SpawnToolTip(item);
        //}
        //else if (slottpye == SlotType.Equip)
        //{
        //    OBPool.Instance.GetUI(PlayerUIManager.Instance.ToolTip_Prefab, PlayerUIManager.Instance.transform).GetComponent<ItemToolTip>().setItem(item);
        //    PlayerUIManager.Instance.equipmentUI.SpawnToolTip(item);
        //}
    }

    //이런식으로 나누는것보단 handler로 퉁치는게 좋을듯
    public void OnPointerExit(PointerEventData eventData)
    {
        //if (slottpye == SlotType.Item)
        //{
        if(item!=null)
            OBPool.Instance.UIdespawn(PlayerUIManager.Instance.ToolTip_Prefab);
        //    PlayerUIManager.Instance.inventoryUI.despawnTooltip();
        //}
        //else if (slottpye == SlotType.Equip)
        //{
        //    PlayerUIManager.Instance.equipmentUI.despawnTooltip();
        //}
      
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        infoSlotReturn();
        if (item != null)
        {
            if (slottpye != SlotType.Quickslot )
            {
            if (eventData.button == PointerEventData.InputButton.Right)
                {
                    if(slottpye == SlotType.Equip )
                        PlayerUIManager.Instance.inventoryUI.SpawnItemSelectPanel(item, id,true);
                    else if(slottpye ==SlotType.Item )
                        PlayerUIManager.Instance.inventoryUI.SpawnItemSelectPanel(item, id);
                }
            }
        }
    }
    private void OnDisable()
    {
        infoSlotReturn();
        if (ondrag)
        {
            IconPos.position = InitPoint;
            ondrag = false;
        }
        OBPool.Instance.UIdespawn(PlayerUIManager.Instance.ToolTip_Prefab);
    }


    private void infoSlotReturn()
    {
        if (slottpye == SlotType.info) return;
    }
}
