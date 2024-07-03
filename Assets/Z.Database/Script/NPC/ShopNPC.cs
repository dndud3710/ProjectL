using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : GameNPC
{
    [SerializeField]
    private ShopItemsSO shopitem;

    public ShopItemsSO shopitem_ => shopitem;

    public override void MakeUI(GameObject g, Transform tr,out IMakeUI makeui,Action Callback=null)
    {
        ShopUI sh = Instantiate(g, tr).GetComponent<ShopUI>();
        sh.SetShopItem(shopitem);
        Callback?.Invoke();
        makeui = sh;
    }
}
