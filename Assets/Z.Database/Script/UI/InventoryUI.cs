using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryUI : WindowUI
{ 
    //UI의 핵심은 상호작용이 일어났을시 동기화를 해주는 부분을 명확히해야한다는것
    public Action GoldInitAction;
    Vector2 setPos;

    [SerializeField] TextMeshProUGUI MoneyText;
    Transform slots_Parent;

    ItemSlot[] slots;
    GameObject slot_Prefab;


    GameObject EquipUseItemSelectPanel;
    GameObject UseUseItemSelectPanel;
    GameObject EtcUseItemSelectPanel;


   
    int slot_num;
    private void Awake()
    {
        slot_num = 36;
        slots= new ItemSlot[slot_num];
        setPos = transform.position;
        slots_Parent = transform.Find("SlotsPanel");


        
        slot_Prefab = Resources.Load<GameObject>("UI/Slot");
        for (int i = 0; i < slot_num; i++)
        {
            slots[i] = Instantiate(slot_Prefab, slots_Parent).GetComponent<ItemSlot>();
            slots[i].setItem(Inventory.Instance.ItemList[i]);
            slots[i].setId(i);
        }



        //아이템 사용 선택창 프레임
        EquipUseItemSelectPanel = Resources.Load<GameObject>("UI/Button/EquipUseItemPanel");
        UseUseItemSelectPanel = Resources.Load<GameObject>("UI/Button/UseUseItemPanel");
        EtcUseItemSelectPanel = Resources.Load<GameObject>("UI/Button/EtcUseItemPanel");
      

    }

    public void Start()
    {
        GoldInitAction += ()=> MoneyText.text = Inventory.Instance.Gold.ToString();
    }
    GameObject ItemUseButtonObj;
    GameObject ToolTipObject;
    private void OnEnable()
    {
        MoneyText.text = Inventory.Instance.Gold.ToString();
    }


    public void SpawnItemSelectPanel(GameItem item,int slotnum,bool equipment=false)
    {
        if (ItemUseButtonObj == null)
        {
            if (equipment) //착용중인 장비를 선택하였을 때
            {
                PlayerUIManager.Instance.equipmentUI.SpawnItemSelectPanel(item,slotnum);
            }
            else
            {
                switch (item.type)
                {
                    case Itemtype.equip:
                        ItemUseButtonObj = OBPool.Instance.Get(EquipUseItemSelectPanel, Input.mousePosition, Quaternion.identity, transform);

                        break;
                    case Itemtype.use:
                        ItemUseButtonObj = OBPool.Instance.Get(UseUseItemSelectPanel, Input.mousePosition, Quaternion.identity, transform);
                        break;
                    case Itemtype.etc:
                        ItemUseButtonObj = OBPool.Instance.Get(EtcUseItemSelectPanel, Input.mousePosition, Quaternion.identity, transform);
                        break;
                }

                ItemUseButtonObj.GetComponent<ItemUseButton>().Init(item, slotnum);
            }
        }
        else
        {
            DespawnItemSelectPanel();
        }
    }
    public void DespawnItemSelectPanel()
    {
        if (ItemUseButtonObj!=null && ItemUseButtonObj.activeSelf)
        {
            OBPool.Instance.Despawn(ItemUseButtonObj);
        }
        ItemUseButtonObj = null;
    }
        

    public void SlotInit(int slotnum)
    {
        slots[slotnum].Init();
    }
    private void OnDisable()
    {
        if(ItemUseButtonObj != null)
        {
            OBPool.Instance.Despawn(ItemUseButtonObj);
            ItemUseButtonObj = null;
        }
        if (ToolTipObject != null)
        {
            OBPool.Instance.Despawn(ToolTipObject);
            ToolTipObject = null;
        }
    }
    //void Init()
    //{
    //    GameItem[] itemlist = Inventory.Instance.ItemList;
    //    for (int i = 0; i < slot_num; i++)
    //    {
    //        slots[i].setItem(itemlist[i]);
    //    }
    //}
}
