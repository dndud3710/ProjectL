using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Trash_ItemListCheck : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> list;

    public void Init()
    {
        GameItem[] itemlist = Inventory.Instance.EquipSlotList;
        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i] != null)    
            list[i].text = $"item{i} : {itemlist[i].ItemInfo.itemname} count : {itemlist[i].amount}";
            else
            {
                list[i].text = $"item{i} : ";
            }
        }
    }
    
}