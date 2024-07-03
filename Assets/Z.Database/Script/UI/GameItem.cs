using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum Itemtype
{
    equip,
    use,
    etc
}
public class GameItem
{

    public Itemtype type {  get; private set; }
    public ItemInfo ItemInfo { get; private set; }
    public int amount { get; private set; }
    public Sprite itemicon { get; private set; }

    protected void setBaseInfo(ItemInfo itemInfo,Itemtype type)
    {
        ItemInfo = itemInfo;
        itemicon = Resources.Load<Sprite>("Data/Image/Item/" + itemInfo.iconName);
        this.type = type;
    }
    public void setAmount(int num)
    {
        amount = num;
    }

}

[Serializable]
public class ItemInfo
{
    public int id;
    public string itemname;
    public string iconName;
    public string Description;
    public int sellPrice;
    public int purchase;
    public ItemInfo(int id,string itemname,string iconame,string description,int sell,int purchase)
    {
        this.id = id;
        this.itemname = itemname;
        this.iconName = iconame;
        this.Description = description;
        this.sellPrice = sell;
        this.purchase = purchase;
    }
}
