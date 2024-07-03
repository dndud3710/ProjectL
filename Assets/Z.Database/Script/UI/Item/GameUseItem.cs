using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUseItem : GameItem
{
    public UseItemInfo useiteminfo { get; private set; }

    public void setItem(UseItemInfo useiteminfo,ItemInfo info, Itemtype type)
    {
        setBaseInfo(info, type);
        this.useiteminfo = useiteminfo;
    }
    public void Use()
    {
        if (GameManager.Instance != null)
        {

            GameManager.Instance.playerStatus.GainHP(useiteminfo.HpHeal);
            GameManager.Instance.playerStatus.GainMP(useiteminfo.MpHeal);
        }
    }
}

[Serializable]
public class UseItemInfo
{
    public ItemInfo iteminfo;
    public int HpHeal { get; private set; }
    public int MpHeal { get; private set; }
    public UseItemInfo(int id, string itemname, string iconame, string description,
        int hpheal,int mpheal,int sell,int purchase)
    {
        iteminfo = new ItemInfo(id, itemname, iconame, description, sell, purchase);
        this.HpHeal = hpheal;
        this.MpHeal = mpheal;
    }
}