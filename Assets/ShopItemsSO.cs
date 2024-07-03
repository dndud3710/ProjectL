using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/ShopItem", order = 1)]
public class ShopItemsSO : ScriptableObject
{
    public string NPCName;
    [Header("npc가 판매할 아이템 아이디")]
    public int[] itemsID;
    public Itemtype type;
}
