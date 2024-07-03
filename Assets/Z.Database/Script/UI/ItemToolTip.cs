using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI Manager에서 GameItem을 다운캐스팅하여 아이템 종류가 무엇인지 파악한 뒤 툴팁 호출
/// </summary>
public class ItemToolTip : MonoBehaviour
{
    RectTransform rectTr;
    [Header("Item")]    
    [SerializeField] private Image ItemIcon;
    [SerializeField] private TextMeshProUGUI ItemName;
    [SerializeField] private TextMeshProUGUI ItemDescript;
    [Header("Equip")]
    [SerializeField] private TextMeshProUGUI STR;
    [SerializeField] private TextMeshProUGUI DEX;
    [SerializeField] private TextMeshProUGUI INT;
    [SerializeField] private TextMeshProUGUI DEF;

    [Header("Skill")]
    [SerializeField] private TextMeshProUGUI Damage;
    [SerializeField] private TextMeshProUGUI CoolTime;

    [SerializeField] GameObject EquipVersion;
    [SerializeField] GameObject SkillVersion;
    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }
    public void setItem(GameItem item_,ItemSlot.SlotType slottype)
    {
        rectTr.pivot = new Vector2(1, 1);
        ItemIcon.sprite = item_.itemicon;
        ItemName.text = item_.ItemInfo.itemname;
        ItemDescript.text = item_.ItemInfo.Description;
        if (item_.type == Itemtype.equip)
        {   
            EquipVersion.SetActive(true);
            SkillVersion.SetActive(false);
            
            rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, 600);
            
            GameEquipItem eq = item_ as GameEquipItem;
            Status stat = eq.EquipItemInfo.status;
            STR.text = $"STR : {stat.STR.ToString()}";
            DEX.text = $"DEX : {stat.DEX.ToString()}";
            INT.text = $"INT : {stat.INT.ToString()}";
            DEF.text = $"DEF : {stat.DEF.ToString()}";
        }
        else
        {
            EquipVersion.SetActive(false);
            SkillVersion.SetActive(false);
            rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, 250);
        }
        if (slottype == ItemSlot.SlotType.Quickslot)
        {
            rectTr.pivot = new Vector2(0, 0);
        }
    }
   
    public void setSkill(SkillInfo skillinfo)
    {
        rectTr.pivot = new Vector2(0, 0);
        ItemIcon.sprite = skillinfo.icon;
        ItemName.text = skillinfo.skillName;
        ItemDescript.text = skillinfo.description;
        Damage.text = $"{GameManager.Instance.playerAttack.damage.minDamage} ~ {GameManager.Instance.playerAttack.damage.maxDamage}";
        CoolTime.text = $"{skillinfo.baseCooldown}초";
        EquipVersion.SetActive(false);
        SkillVersion.SetActive(true);
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, 265);
    }
    private void Update()
    {
        transform.position = Input.mousePosition;
        //if(type_ == type.item)
        //else if (type_ == type.skill)
    }
}
