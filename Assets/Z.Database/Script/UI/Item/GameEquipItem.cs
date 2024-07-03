using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EquipType
{
    Weapon,
    Helmet,
    Armor,
    Gloves,
    Boots
}
public class GameEquipItem : GameItem
{

  
    public EquipType equiptype { get;private set; }
    public EquipItemInfo EquipItemInfo { get; private set; }

    public void setItem(EquipItemInfo item,int equiptype, ItemInfo info, Itemtype type)
    {
        setBaseInfo(info, type);
        EquipItemInfo = item;
        this.equiptype = (EquipType)equiptype;
    }

    public void Use(int num)
    {
        Inventory.Instance.EquipItemEquip(this,num);
    }
}

[Serializable]
public class EquipItemInfo
{
    public ItemInfo iteminfo;
    public Status status;
    public int equiptype;
    public int Player_Class;
    public string GamePrefabName;
    public EquipItemInfo(int id, string itemname, string iconame, string description,
        int str,int dex, int INT,int DEF,int equiptype,int Player_Class,string GamePrefabName,int sell,int purchase)
    {
        this.equiptype =equiptype;
        iteminfo = new ItemInfo(id, itemname, iconame, description,sell,purchase);
        status = new Status(str,dex,INT,DEF);
        this.Player_Class = Player_Class;
        this.GamePrefabName = GamePrefabName;
    }
}
