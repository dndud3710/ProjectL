using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FactoryManager
{
    private Dictionary<int, EquipItemInfo> EquipItemDic;
    private Dictionary<int, UseItemInfo> UseItemDic;
    private Dictionary<int, EtcItemInfo> EtcItemDic;
    public FactoryManager()
    {
        LoadData();
    }
    private void LoadData()
    {
        EquipItemDic =new Dictionary<int, EquipItemInfo>();
        UseItemDic = new Dictionary<int, UseItemInfo>();
        EtcItemDic = new Dictionary<int, EtcItemInfo>();
        List<EquipItemInfo> equipinfoList = ConvertJson<List<EquipItemInfo>>.ReadJsonFile(EFolderName.Item, "EquipItemsInfo");
        List<UseItemInfo> useinfoList = ConvertJson<List<UseItemInfo>>.ReadJsonFile(EFolderName.Item, "UseItemsInfo");
        List<EtcItemInfo> etcinfoList = ConvertJson<List<EtcItemInfo>>.ReadJsonFile(EFolderName.Item, "EtcItemsInfo");
        foreach (EquipItemInfo item in equipinfoList)
        {
            EquipItemDic.Add(item.iteminfo.id, item);
        }
        foreach (UseItemInfo item in useinfoList)
        {
            UseItemDic.Add(item.iteminfo.id, item);
        }
        foreach (EtcItemInfo item in etcinfoList)
        {
            EtcItemDic.Add(item.iteminfo.id, item);
        }
    }
    public GameItem GetItem(int id,Itemtype type)
    {
        GameItem gameitem = null;
        if (type == Itemtype.equip)
        {
            GameEquipItem equipitem = new GameEquipItem();
            equipitem.setItem(EquipItemDic[id], EquipItemDic[id].equiptype, EquipItemDic[id].iteminfo, Itemtype.equip);
            gameitem = equipitem;
        }
        else if (type == Itemtype.use)
        {
            GameUseItem useitem = new GameUseItem();
            useitem.setItem(UseItemDic[id], UseItemDic[id].iteminfo, Itemtype.use);
            gameitem = useitem;
        }
        else if (type == Itemtype.etc)
        {
            GameEtcItem etcitem = new GameEtcItem();
            etcitem.setItem(EtcItemDic[id], EtcItemDic[id].iteminfo, Itemtype.etc);
            gameitem = etcitem;
        }
        return gameitem;
    }
    public TestMob GetMonster()
    {
        TestMob mob = new TestMob();
        return mob;
    }
    public TestMob[] GetMonster(int num)
    {
        TestMob[] mob = new TestMob[num];
        return mob;
    }
}
