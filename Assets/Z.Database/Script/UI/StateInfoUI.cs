using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StateInfoUI : MonoBehaviour
{
    [SerializeField] Image characterIcon;
    [SerializeField] private Transform SkillSlotParent;
    [SerializeField] private Transform QuickSlotParent;

    ItemSlot[] quickslots;
    SkillSlot[] skilltslots;

    public ItemSlot[] quickslots_ => quickslots;
    GameObject slot_Prefab;
    int quickslotcount;

    GameObject ToolTipObject;
    public void SpawnToolTip(SkillInfo skills_)
    {
        if (skills_ != null)
        {
            ToolTipObject = OBPool.Instance.GetUI(PlayerUIManager.Instance.ToolTip_Prefab,Vector2.zero,transform);
            ToolTipObject.GetComponent<ItemToolTip>().setSkill(skills_);
        }
    }
    public void despawnTooltip()
    {
        if (ToolTipObject != null && ToolTipObject.activeSelf == true)
        {
            OBPool.Instance.UIdespawn(ToolTipObject);
            ToolTipObject = null;
        }
    }
    private IEnumerator SetCharacterIcon()
    {
        yield return new WaitUntil(() => GameManager.Instance !=null);
        yield return new WaitUntil(() => GameManager.Instance.playerStatus);
            Debug.Log(GameManager.Instance.playerStatus);
        int cls = (int)GameManager.Instance.playerStatus.playerClass;
       if(cls == 0)
        {
            characterIcon.sprite = Resources.Load<Sprite>("WarriorIcon");
        }
       else if(cls == 1)
        {
            characterIcon.sprite = Resources.Load<Sprite>("GunnerIcon");
        }
        else if (cls == 2)
        {
            characterIcon.sprite = Resources.Load<Sprite>("MageIcon");
        }
    }
    private void Start  ()
    {
        quickslotcount = 4;
        quickslots = new ItemSlot[quickslotcount];
        slot_Prefab = Resources.Load<GameObject>("UI/Item_QuickSlot");
        for (int i = 0; i < quickslotcount; i++)
        {
            quickslots[i]=Instantiate(slot_Prefab, QuickSlotParent).GetComponent<ItemSlot>();
            quickslots[i].setItem(Inventory.Instance.QuickslotList[i]);
            quickslots[i].setId(i);
        }
        StartCoroutine( SetCharacterIcon());
    }

}
