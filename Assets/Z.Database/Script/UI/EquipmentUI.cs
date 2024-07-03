using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IPointerToolTip
{
    public void SpawnToolTip(GameItem item_);
    public void despawnTooltip();
    //ondisable¿¡¼­µµ despawn
}

public class EquipmentUI : WindowUI
{
    [SerializeField] Transform SlotsParent;
    [SerializeField] Transform CharacterStatinfo;

    ItemSlot[] equipSlots;

    GameObject slot_Prefab;
    int quickslotcount;
    GameObject EquipMentUseItemPanel;

    Dictionary<string, TextMeshProUGUI> statinfoText;
    private void Awake()
    {
        EquipMentUseItemPanel = Resources.Load<GameObject>("UI/Button/EquipMentUseItemPanel");
        statinfoText = new Dictionary<string, TextMeshProUGUI>();
        for(int i = 0; i < CharacterStatinfo.childCount; i++)
        {
            Transform go = CharacterStatinfo.GetChild(i);
            statinfoText.Add(go.name, go.GetComponent<TextMeshProUGUI>());
        }

    }
    private void Start()
    {
        quickslotcount = 5;
        equipSlots = new ItemSlot[quickslotcount];


        slot_Prefab = Resources.Load<GameObject>("UI/Item_EquipSlot");
        GameItem[] equiplist = Inventory.Instance.EquipSlotList;
        for (int i = 0; i < quickslotcount; i++)
        {
            equipSlots[i] = Instantiate(slot_Prefab, SlotsParent).GetComponent<ItemSlot>();
            equipSlots[i].setItem(equiplist[i]);
            equipSlots[i].setId(i);
        }
        InitStatinfo();
    }
    private void OnEnable()
    {
        InitStatinfo();
    }
    public void InitStatinfo()
    {
        Status ps = GameManager.Instance.playerStatus.playerStat;
        statinfoText["STR"].text = $"STR : {ps.STR}";
        statinfoText["DEX"].text = $"DEX : {ps.DEX}";
        statinfoText["INT"].text = $"INT : {ps.INT}";
        statinfoText["DEF"].text = $"DEF : {ps.DEF}";
    }
    public void SlotInit(int num)
    {
        equipSlots[num].Init();
    }
    GameObject ToolTipObject;
    GameObject ItemUseButtonObj;

    public void SpawnToolTip(GameItem item_)
    {
        if (item_ != null)
        {
            ToolTipObject = OBPool.Instance.Get<ItemToolTip>(PlayerUIManager.Instance.ToolTip_Prefab, transform);
            ToolTipObject.GetComponent<ItemToolTip>().setItem(item_, ItemSlot.SlotType.Equip);
        }
    }
    public void despawnTooltip()
    {
        if (ToolTipObject != null && ToolTipObject.activeSelf == true)
        {
            OBPool.Instance.Despawn(ToolTipObject);
            ToolTipObject = null;
        }
    }
    public void SpawnItemSelectPanel(GameItem item, int slotnum)
    {
        if (ItemUseButtonObj == null)
        {
            ItemUseButtonObj = OBPool.Instance.Get(EquipMentUseItemPanel, Input.mousePosition, Quaternion.identity, transform);
            ItemUseButtonObj.GetComponent<ItemUseButton>().Init(item, slotnum);
        }
        else
        {
            DespawnItemSelectPanel();
        }

    }
    public void DespawnItemSelectPanel()
    {
        if (ItemUseButtonObj != null && ItemUseButtonObj.activeSelf)
        {
            OBPool.Instance.Despawn(ItemUseButtonObj);
        }
        ItemUseButtonObj = null;
    }
    private void OnDisable()
    {
        if (ItemUseButtonObj != null)
        {
            OBPool.Instance.Despawn(ItemUseButtonObj);
            ItemUseButtonObj = null;
        }
        if (ToolTipObject != null)
        {
            OBPool.Instance.Despawn(ToolTipObject);
            ToolTipObject = null;
        }
    }
}
