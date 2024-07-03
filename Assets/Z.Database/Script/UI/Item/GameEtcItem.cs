using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEtcItem : GameItem
{
    public EtcItemInfo EtcItemInfo { get; private set; }
    public void setItem(EtcItemInfo etciteminfo, ItemInfo info, Itemtype type)
    {
        setBaseInfo(info, type);
        this.EtcItemInfo = etciteminfo;
    }

}

[Serializable]
public class EtcItemInfo
{
    public ItemInfo iteminfo;
    public EtcItemInfo(int id, string itemname, string iconame, string description,int sell,int purchase)
    
    {
        iteminfo = new ItemInfo(id, itemname, iconame, description, sell, purchase);
    }
}