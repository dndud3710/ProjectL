using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory  
{
    public Action getGoldEventHandler;//��尡 ���Ҷ� ȣ��Ǵ� �̺�Ʈ

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

    private GameItem[] itemlist_;//�κ��丮 ����Ʈ
    private GameItem[] QuickslotList_;//������ ����Ʈ
    private GameItem[] EquipSlotList_; //��񽽷� ����Ʈ
    //0 ���� 1 ���� 2 ���� 3 �尩 4 �Ź�
    public GameItem[] ItemList { get { return itemlist_; } }
    public GameItem[] QuickslotList { get { return QuickslotList_; } }
    public GameItem[] EquipSlotList { get { return EquipSlotList_; } }


    private int itemCount;//�κ��丮 ���� ����
    private int quickslotCount;//������ ����
    private int equipslotCount;//��񽽷� ����

    Dictionary<string, int> ItemCheck; //�������� �ִ��� üũ�ϴ� ��ųʸ�
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
    //2�ʸ��� �ڷ�ƾ�Լ�
  







    //�߰�, ����, ����, �ű��, 
    //������ ���� ���ѽý��ۺ���

    public int getItem(GameItem item_,int count) //������ ��� ��ũ��Ʈ
    {
        if (ItemCheck.ContainsKey(item_.ItemInfo.itemname)) //�̹� �������� �������
        {
             
            int num = ItemCheck[item_.ItemInfo.itemname];
            int amount_ = itemlist_[num].amount;
            amount_ += count;
            itemlist_[num].setAmount(amount_);
            return num;
        }
        //1. ���������� �迭�� �˻��Ͽ� ���ڸ� ������� ȹ��
        for (int i = 0; i < itemlist_.Length; i++)
            {
                if (itemlist_[i] == null)
                {
                    itemlist_[i] = item_;
                    item_.setAmount(count);
                    
                if(item_.type != Itemtype.equip) //��� �ƴҰ�� 
                    ItemCheck.Add(item_.ItemInfo.itemname, i);
                return i;
                }
            }
        //2. ���ڸ��� ���� ��� ����ó��
        return -1;
    }
    //��� ȹ�� �Լ�
    public void getGold(int gold)
    {
        Gold += gold;
        if(Gold < 0)
        {
            Gold = 0;
        }
        getGoldEventHandler?.Invoke(); //��尡 ���Ҷ� ȣ��Ǵ� �̺�Ʈ
    }
    public bool giveGold(int gold)
    {
        if (Gold - gold < 0) return false;
        Gold -= gold;
        getGoldEventHandler?.Invoke(); //��尡 ���Ҷ� ȣ��Ǵ� �̺�Ʈ
        return true;
    }
    /// <summary>
    /// �κ��丮���� Slot�� type�� üũ�Ͽ� �ش� ������ num��° �������� ��ȯ
    /// </summary>
    /// <param name="num">���° ����</param>
    /// <param name="type_">������ Ÿ��</param>
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
    //������ ���� (���� ������Ʈ)
    public void DeletItem(int num)
    {
        itemlist_[num] = null;
    }
    /// <summary>
    /// �κ��丮���� Equip�������� �����Ҷ� ȣ��
    /// </summary>
    /// <param name="item_"> ������ ������</param>
    /// <param name="num"> ������ �������� �κ��丮 ��ȣ</param>
    public void EquipItemEquip(GameEquipItem item_,int num)
    {
        if (item_.type != Itemtype.equip) return; //��񰡾ƴϸ� return
        if(item_.EquipItemInfo.Player_Class != (int)GameManager.Instance.playerStatus.playerClass) return; //�÷��̾� Ŭ������ �ٸ��� return
        int equipnum = (int)item_.equiptype;
        if (EquipSlotList_[equipnum] == null)// �������� ��� ������
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
    //EquipMent���� �����ϱ� Ŭ���� ȣ��
    public void EquiItemUnEquip(GameEquipItem item_) 
    {
        if (item_.type != Itemtype.equip) return; //��񰡾ƴϸ� return

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
        //1. ������ ���� ��ȯ
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
        //1. ó�� ������ �������� ������
        if (arr1[num1] == null)
        {
            return;
        }
        //2. ���ʸ� �������� ������
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
                

                if (type1 == ItemSlot.SlotType.Item) //������â���� ���â����
                {
                    if ((int)GameManager.Instance.playerStatus.playerClass != eq.EquipItemInfo.Player_Class)
                        return; //���� Ŭ�����ƴҽ� return
                    if ((int)eq.equiptype != num2) return;
                    
                    EquipSlotList_[(int)eq.equiptype] = arr1[num1];
                    arr1[num1] = null;
                    GameManager.Instance.playerStatus.EquipItem(eq.EquipItemInfo.status);
                }
                else if(type1 == ItemSlot.SlotType.Equip) //���â���� ������â����
                {
                    arr2[num2] = EquipSlotList_[(int)eq.equiptype];
                    EquipSlotList_[(int)eq.equiptype] = null;
                    GameManager.Instance.playerStatus.UnEquipItem(eq.EquipItemInfo.status);
                }
                PlayerUIManager.Instance.equipmentUI.InitStatinfo();
            }
        }
        //3. �Ѵ� �������� ������
        else 
        {
            
            if (false == equip)//��� �ƴҶ�
            {
                GameItem temp = arr1[num1];
                arr1[num1] = arr2[num2];
                arr2[num2] = temp;
            }
            else //����϶�
            {
                GameEquipItem eq = arr1[num1] as GameEquipItem;
               
                if (type1 == ItemSlot.SlotType.Item) //������â���� ���â����
                {
                    if ((int)GameManager.Instance.playerStatus.playerClass != eq.EquipItemInfo.Player_Class)
                        return; //���� Ŭ�����ƴҽ� return
                    if ((int)eq.equiptype != num2) return;
                    GameItem temp = EquipSlotList_[(int)eq.equiptype];
                    GameEquipItem eqitem = temp as GameEquipItem;
                    GameManager.Instance.playerStatus.UnEquipItem(eqitem.EquipItemInfo.status);
                    EquipSlotList_[(int)eq.equiptype] = arr1[num1];
                    GameManager.Instance.playerStatus.EquipItem(eq.EquipItemInfo.status);
                    arr1[num1] = temp;

                }
                else if (type1 == ItemSlot.SlotType.Equip) //���â���� ������â����
                {
                    GameItem temp = arr2[num2];
                    GameEquipItem eqitem = temp as GameEquipItem;
                    if ((int)GameManager.Instance.playerStatus.playerClass != eqitem.EquipItemInfo.Player_Class)
                        return; //���� Ŭ�����ƴҽ� return
                    GameManager.Instance.playerStatus.UnEquipItem(eqitem.EquipItemInfo.status);
                    arr2[num2] = EquipSlotList_[(int)eq.equiptype];
                    GameManager.Instance.playerStatus.EquipItem(eq.EquipItemInfo.status);
                    EquipSlotList_[(int)eq.equiptype] = temp;
                }
                PlayerUIManager.Instance.equipmentUI.InitStatinfo();
            }


        }
    }
    //���â�� ��� �����Ǿ����� 
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

