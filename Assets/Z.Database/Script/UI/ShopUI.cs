using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour , IMakeUI
{
    [SerializeField] TextMeshProUGUI buyitemprice;
    [SerializeField] Button BuyItemButton;
    int buyMoney;
    [SerializeField]
    List<InfoSlot> buyItemImages;
    Itemtype type;
    ///���� ������Ʈ
    ///1. ����, �Ǹ� Ŭ���� ������Ʈ�� �� ����
    ///2. �������� �Ǹ��ϴ� �����۵�
    ///2.1 ������ ������ image
    ///2.2 ������ �̸� textugui
    ///2.3 ������ ���� textugui
    ///3. �����Ϸ��� �������� ��Ƴ��� ����
    ///4. ������ ����/�Ǹ� �ϴ� ��ư

    List<GameObject> ButtonList; //���� �Ǹ� ��ư
    List<ShopItemInfo> shopiteminfo;
    List<GameItem> sellItems;

    GameObject ToolTipObject;
    [Serializable]
    class ShopItemInfo
    {
        public InfoSlot iconslot;

        public TextMeshProUGUI itemname;
        public TextMeshProUGUI itemprice;


    }


    private void Awake()
    {
        buyMoney = 0;
        sellItems = new List<GameItem>();
        ButtonList = new List<GameObject>();
        shopiteminfo = new List<ShopItemInfo>();

        Transform slotsTr = transform.Find("Top");

        foreach(InfoSlot info_ in buyItemImages)
        {
            info_.transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameItem item__ = info_.item_;
                if (item__ != null)
                {
                    info_.Clear();

                }
                buyMoney -= item__.ItemInfo.purchase;
                buyitemprice.text = buyMoney.ToString();
            });
        }

        for (int i = 0; i < slotsTr.childCount; i++)
        {
            ShopItemInfo temp = new ShopItemInfo();

            InfoSlot info = slotsTr.GetChild(i).transform.Find("ImgIcon").GetComponent<InfoSlot>();
                slotsTr.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (info.item_ != null)
                        buyItemList(info.item_);
                });
            temp.iconslot = info;
            temp.itemname = slotsTr.GetChild(i).transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            temp.itemprice = slotsTr.GetChild(i).transform.Find("Image").transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
            shopiteminfo.Add(temp);
            BuyItemButton.onClick.AddListener(BuyItems);
        }
    }
    /// <summary>
    /// ������ �̸��� �޾Ƽ� ����
    /// </summary>
    /// <param name="ItemListName"></param>
    public void SetShopItem(ShopItemsSO shopso)
    {
        int count = 0;
        switch (shopso.type)
        {
            case Itemtype.equip:
                foreach (int item in shopso.itemsID)
                {
                    GameItem temp = GameManager.Instance.factory.GetItem(item,Itemtype.equip);
                    shopiteminfo[count].iconslot.setImage(temp);
                    shopiteminfo[count].itemname.text = temp.ItemInfo.itemname;
                    shopiteminfo[count].itemprice.text = temp.ItemInfo.purchase.ToString();
                    count++;
                }
                break;

            case Itemtype.use:
                foreach (int item in shopso.itemsID)
                {
                    GameItem temp = GameManager.Instance.factory.GetItem(item, Itemtype.use);
                    shopiteminfo[count].iconslot.setImage(temp);
                    shopiteminfo[count].itemname.text = temp.ItemInfo.itemname;
                    shopiteminfo[count].itemprice.text = temp.ItemInfo.purchase.ToString();
                    count++;
                }
                break;
        }

        
    }

    private void buyItemList(GameItem item)
    {
        foreach(InfoSlot info_ in buyItemImages)
        {
            if(info_.item_ == null)
            {
                info_.setImage(item);
                buyMoney += item.ItemInfo.purchase;
                buyitemprice.text = buyMoney.ToString();
                return;
            }
        }
    }
    private void BuyItems()
    {
        if(Inventory.Instance.giveGold(buyMoney) == true)
        {
            foreach (InfoSlot info_ in buyItemImages)
            {
                if (info_.item_ != null)
                {
                    SimulationUI.Instance.inventoryui.SlotInit(Inventory.Instance.getItem(info_.item_, 1));
                    SimulationUI.Instance.inventoryui.GoldInitAction?.Invoke();
                }
            }
            foreach (InfoSlot info_ in buyItemImages)
            {
                info_.Clear();
            }
            buyMoney = 0;
            buyitemprice.text = "0";
        }
    }
    private void OnDisable()
    {
        foreach(InfoSlot info_ in buyItemImages)
        {
            info_.Clear();
        }
        buyMoney = 0;
        buyitemprice.text = "0";
        if(ToolTipObject !=null && ToolTipObject.activeSelf) OBPool.Instance.Despawn(ToolTipObject);
    }

    public void ObjectDestroy()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// ������ �� �ΰ���
    /// 1. �巡�� ���ñ���� �ְ� �ʱ�ȭ�ϴ� ��ɵ��� �ִ� ��������� prfab variant�� ����� slotprefab
    /// 2. ������ img�� tooltip�� �����ִ� ����
    /// </summary>
}
