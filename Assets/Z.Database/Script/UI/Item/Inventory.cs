using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory  
{
    public Action getGoldEventHandler;//골드가 변할때 호출되는 이벤트

    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Inventory();
            }
            return instance;
        }
    }

    public int Gold { get; private set; }

    private GameItem[] itemlist_;//인벤토리 리스트
    private GameItem[] QuickslotList_;//퀵슬롯 리스트
    private GameItem[] EquipSlotList_; //장비슬롯 리스트
    //0 무기 1 투구 2 갑옷 3 장갑 4 신발
    public GameItem[] ItemList { get { return itemlist_; } }
    public GameItem[] QuickslotList { get { return QuickslotList_; } }
    public GameItem[] EquipSlotList { get { return EquipSlotList_; } }


    private int itemCount;//인벤토리 슬롯 갯수
    private int quickslotCount;//퀵슬롯 갯수
    private int equipslotCount;//장비슬롯 갯수

    Dictionary<string, int> ItemCheck; //아이템이 있는지 체크하는 딕셔너리
    public void Init()
    {
        itemCount = 36;
        quickslotCount = 4;
        equipslotCount = 5;
        Gold = 0;
        itemlist_ = new GameItem[itemCount];
        QuickslotList_ = new GameItem[quickslotCount];
        EquipSlotList_ = new GameItem[equipslotCount];
        ItemCheck = new Dictionary<string, int>();
        //getItem(FactoryManager.Instance.GetItem<GameEquipItem>(0));
        getItem(GameManager.Instance.factory.GetItem(2,Itemtype.equip),1);
        getItem(GameManager.Instance.factory.GetItem(1,Itemtype.use),1);
        getItem(GameManager.Instance.factory.GetItem(0,Itemtype.use),2);
        getItem(GameManager.Instance.factory.GetItem(3,Itemtype.use),0);
        getItem(GameManager.Instance.factory.GetItem(2,Itemtype.use),3);
        getItem(GameManager.Instance.factory.GetItem(3,Itemtype.use),2);
        getGold(10000);

    }
    private void Start()
    {
      
    }
    //2초마다 코루틴함수
  







    //추가, 삭제, 변경, 옮기기, 
    //데이터 검증 권한시스템변경

    public int getItem(GameItem item_,int count) //아이템 얻는 스크립트
    {
        if (ItemCheck.ContainsKey(item_.ItemInfo.itemname)) //이미 아이템이 있을경우
        {
             
            int num = ItemCheck[item_.ItemInfo.itemname];
            int amount_ = itemlist_[num].amount;
            amount_ += count;
            itemlist_[num].setAmount(amount_);
            return num;
        }
        //1. 순차적으로 배열을 검색하여 빈자리 있을경우 획득
        for (int i = 0; i < itemlist_.Length; i++)
            {
                if (itemlist_[i] == null)
                {
                    itemlist_[i] = item_;
                    item_.setAmount(count);
                    
                if(item_.type != Itemtype.equip) //장비가 아닐경우 
                    ItemCheck.Add(item_.ItemInfo.itemname, i);
                return i;
                }
            }
        //2. 빈자리가 없을 경우 예외처리
        return -1;
    }
    //골드 획득 함수
    public void getGold(int gold)
    {
        Gold += gold;
        if(Gold < 0)
        {
            Gold = 0;
        }
        getGoldEventHandler?.Invoke(); //골드가 변할때 호출되는 이벤트
    }
    public bool giveGold(int gold)
    {
        if (Gold - gold < 0) return false;
        Gold -= gold;
        getGoldEventHandler?.Invoke(); //골드가 변할때 호출되는 이벤트
        return true;
    }
    /// <summary>
    /// 인벤토리에서 Slot의 type을 체크하여 해당 슬롯의 num번째 아이템을 반환
    /// </summary>
    /// <param name="num">몇번째 슬롯</param>
    /// <param name="type_">슬롯의 타입</param>
    /// <returns></returns>
    public GameItem getItem(int num,ItemSlot.SlotType type_)
    {
        if (type_ == ItemSlot.SlotType.Item)
        {
            if (itemlist_[num] == null) return null;
            else
            {
                if (itemlist_[num].amount == 0) return null;
                return itemlist_[num];
            }
        }
        else if (type_ == ItemSlot.SlotType.Quickslot)
        {
            if (QuickslotList_[num] == null) return null;
            else
            {
                if (QuickslotList_[num].amount == 0) return null;
                return QuickslotList_[num];
            }
        }
        else if (type_ == ItemSlot.SlotType.Equip)
        {
            if (EquipSlotList_[num] == null) return null;
            else
            {
                return EquipSlotList_[num];
            }
        }
        else return null;
    }
    public void QuickslotItemUse(int num)
    {
        GameUseItem use = QuickslotList[num] as GameUseItem;
        use.Use();
        int count_ = QuickslotList[num].amount;
        QuickslotList[num].setAmount(--count_);
        PlayerUIManager.Instance.InitQuickslot(num);
    }
    public void ItemUse(int num)
    {
        GameUseItem use = ItemList[num] as GameUseItem;
        use.Use();
        int count_ = ItemList[num].amount;
        ItemList[num].setAmount(--count_);
        PlayerUIManager.Instance.inventoryUI.SlotInit(num);
    }
    //아이템 삭제 (추후 업데이트)
    public void DeletItem(int num)
    {
        itemlist_[num] = null;
    }
    /// <summary>
    /// 인벤토리에서 Equip아이템을 장착할때 호출
    /// </summary>
    /// <param name="item_"> 장착할 아이템</param>
    /// <param name="num"> 장착할 아이템의 인벤토리 번호</param>
    public void EquipItemEquip(GameEquipItem item_,int num)
    {
        if (item_.type != Itemtype.equip) return; //장비가아니면 return
        if(item_.EquipItemInfo.Player_Class != (int)GameManager.Instance.playerStatus.playerClass) return; //플레이어 클래스가 다르면 return
        int equipnum = (int)item_.equiptype;
        if (EquipSlotList_[equipnum] == null)// 장착중인 장비가 없을때
        {
            EquipSlotList_[equipnum] = itemlist_[num];
            itemlist_[num] = null;
            GameManager.Instance.playerStatus.EquipItem(item_.EquipItemInfo.status);
        }
        else
        {
            GameItem temp = itemlist_[num];
            GameManager.Instance.playerStatus.EquipItem(item_.EquipItemInfo.status);
            itemlist_[num] = EquipSlotList_[equipnum];
            GameEquipItem eqitem = EquipSlotList_[equipnum] as GameEquipItem;
            GameManager.Instance.playerStatus.UnEquipItem(eqitem.EquipItemInfo.status);
            EquipSlotList_[equipnum] = temp;
        }
        EquipMentUiInit(equipnum, num);
    }
    //EquipMent에서 해제하기 클릭시 호출
    public void EquiItemUnEquip(GameEquipItem item_) 
    {
        if (item_.type != Itemtype.equip) return; //장비가아니면 return

        int equipnum = (int)item_.equiptype;
        int num = getItem(EquipSlotList_[equipnum],1);
        EquipSlotList_[equipnum] = null;


        EquipMentUiInit(equipnum, num);
    }

    private void EquipStat()
    {
        PlayerStatus ps= GameManager.Instance.playerStatus;
        foreach (GameEquipItem go in EquipSlotList_)
        {
            ps.EquipItem(go.EquipItemInfo.status);
        }

    }
    public void UIswapItem(int num1, int num2,ItemSlot.SlotType type1, ItemSlot.SlotType type2)
    {
        //1. 퀵슬롯 끼리 교환
        if (type1 == ItemSlot.SlotType.Quickslot && type2 == ItemSlot.SlotType.Quickslot)
        {
            changeLogic(ref QuickslotList_, ref QuickslotList_, num1, num2);
        }
        else if (type1 == ItemSlot.SlotType.Item && type2 == ItemSlot.SlotType.Quickslot)
        {
            if (itemlist_[num1].type != Itemtype.use) return;
            changeLogic(ref itemlist_, ref QuickslotList_, num1, num2);
        }
        else if (type1 == ItemSlot.SlotType.Quickslot && type2 == ItemSlot.SlotType.Item)
        {
            if (itemlist_[num2]!=null && itemlist_[num2].type != Itemtype.use) return;
            changeLogic(ref QuickslotList_, ref itemlist_, num1, num2);
        }
        else if (type1 == ItemSlot.SlotType.Item && type2 == ItemSlot.SlotType.Equip)
        {
            if (itemlist_[num1].type != Itemtype.equip) return;
            changeLogic(ref itemlist_, ref EquipSlotList_, num1, num2,true,type1,type2);
        }
        else if (type1 == ItemSlot.SlotType.Equip && type2 == ItemSlot.SlotType.Item)
        {
            if (itemlist_[num2] != null && itemlist_[num2].type != Itemtype.equip) return;
            changeLogic(ref EquipSlotList_, ref itemlist_, num1, num2, true, type1, type2);
        }
        else if (type1 == ItemSlot.SlotType.Item && type2 == ItemSlot.SlotType.Item)
        {
            changeLogic(ref itemlist_, ref itemlist_, num1, num2);
        }
        else { return; }
        
    }
    
    private void changeLogic(ref GameItem[] arr1 ,ref GameItem[] arr2,int num1, int num2,bool equip=false, ItemSlot.SlotType type1=ItemSlot.SlotType.Item,ItemSlot.SlotType type2= ItemSlot.SlotType.Equip)
    {
        //1. 처음 선택한 아이템이 없을때
        if (arr1[num1] == null)
        {
            return;
        }
        //2. 한쪽만 아이템이 있을때
        else if (arr2[num2] == null)
        {
            if (false == equip)
            {
                arr2[num2] = arr1[num1];
                arr1[num1] = null;
            }
            else
            {
                GameEquipItem eq = arr1[num1] as GameEquipItem;
                

                if (type1 == ItemSlot.SlotType.Item) //아이템창에서 장비창으로
                {
                    if ((int)GameManager.Instance.playerStatus.playerClass != eq.EquipItemInfo.Player_Class)
                        return; //같은 클래스아닐시 return
                    if ((int)eq.equiptype != num2) return;
                    
                    EquipSlotList_[(int)eq.equiptype] = arr1[num1];
                    arr1[num1] = null;
                    GameManager.Instance.playerStatus.EquipItem(eq.EquipItemInfo.status);
                }
                else if(type1 == ItemSlot.SlotType.Equip) //장비창에서 아이템창으로
                {
                    arr2[num2] = EquipSlotList_[(int)eq.equiptype];
                    EquipSlotList_[(int)eq.equiptype] = null;
                    GameManager.Instance.playerStatus.UnEquipItem(eq.EquipItemInfo.status);
                }
                PlayerUIManager.Instance.equipmentUI.InitStatinfo();
            }
        }
        //3. 둘다 아이템이 있을때
        else 
        {
            
            if (false == equip)//장비가 아닐때
            {
                GameItem temp = arr1[num1];
                arr1[num1] = arr2[num2];
                arr2[num2] = temp;
            }
            else //장비일때
            {
                GameEquipItem eq = arr1[num1] as GameEquipItem;
               
                if (type1 == ItemSlot.SlotType.Item) //아이템창에서 장비창으로
                {
                    if ((int)GameManager.Instance.playerStatus.playerClass != eq.EquipItemInfo.Player_Class)
                        return; //같은 클래스아닐시 return
                    if ((int)eq.equiptype != num2) return;
                    GameItem temp = EquipSlotList_[(int)eq.equiptype];
                    GameEquipItem eqitem = temp as GameEquipItem;
                    GameManager.Instance.playerStatus.UnEquipItem(eqitem.EquipItemInfo.status);
                    EquipSlotList_[(int)eq.equiptype] = arr1[num1];
                    GameManager.Instance.playerStatus.EquipItem(eq.EquipItemInfo.status);
                    arr1[num1] = temp;

                }
                else if (type1 == ItemSlot.SlotType.Equip) //장비창에서 아이템창으로
                {
                    GameItem temp = arr2[num2];
                    GameEquipItem eqitem = temp as GameEquipItem;
                    if ((int)GameManager.Instance.playerStatus.playerClass != eqitem.EquipItemInfo.Player_Class)
                        return; //같은 클래스아닐시 return
                    GameManager.Instance.playerStatus.UnEquipItem(eqitem.EquipItemInfo.status);
                    arr2[num2] = EquipSlotList_[(int)eq.equiptype];
                    GameManager.Instance.playerStatus.EquipItem(eq.EquipItemInfo.status);
                    EquipSlotList_[(int)eq.equiptype] = temp;
                }
                PlayerUIManager.Instance.equipmentUI.InitStatinfo();
            }


        }
    }
    //장비창에 장비가 장착되었을때 
    private void EquipMentUiInit(int equipnum, int num)
    {
        InventoryUI iui = PlayerUIManager.Instance.inventoryUI;
        EquipmentUI eui = PlayerUIManager.Instance.equipmentUI;
        if (iui != null)
            iui.SlotInit(num);
        if (eui != null)
            eui.SlotInit(equipnum);
        if (eui != null)
            eui.InitStatinfo();
    }
}

