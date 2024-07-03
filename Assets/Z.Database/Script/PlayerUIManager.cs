using DG.Tweening;
using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    private string ResourceUIPath = "UI";// 리소스 경로

    [Header("UI")]
    [SerializeField] Transform inventorytransform; // 인벤토리창 위치
    [SerializeField] Transform equipmenttransform; // 장비창 위치
    [SerializeField] Transform SkillSlotTransform;
    [SerializeField] Button EquipUIButton;
    [SerializeField] Button InventoryUIButton;
    [SerializeField] HpbarEffect hpeffect;
    [SerializeField] HpbarEffect mpeffect;
    [SerializeField] GameObject Interaction;
    [Space(20)]
    [Header("Player Status")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image mpBar;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Slider expSlider;
    [SerializeField] TextMeshProUGUI LevelText;

    [Space(20)]
    [Header("Dialog Settings")]
    [SerializeField] private Text dialogText;
    public float displayTime = 1.5f;
    public float fadeDuration = 0.5f;

    List<SkillSlot> SkillSlots_;
    public List<SkillSlot> SkillSlots => SkillSlots_;

    [Header("UI Instance Scripts")]
    InventoryUI inventoryui; // 인벤토리창
    EquipmentUI equipmentui; // 장비창
    [SerializeField] StateInfoUI stateinfoui; // 장비창

    public InventoryUI inventoryUI => inventoryui;
    public EquipmentUI equipmentUI => equipmentui;
    public StateInfoUI stateinfoUI => stateinfoui;


    [Header("UI Resrouces")]
    public Slider castingBar;
    public GameObject damageTextPrefab;
    GameObject Inventory_UI_Prefab;//인벤토리창 프리팹
    GameObject Equipment_UI_Prefab;//장비창 프리팹

    public GameObject ToolTip_Prefab { get; private set; }

    public GameObject UseItemSelectPanel { get; private set; }
    public static PlayerUIManager Instance
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            SkillSlots_ = new List<SkillSlot>();
            Inventory_UI_Prefab = Resources.Load<GameObject>(ResourceUIPath + "/InventoryUI");
            Equipment_UI_Prefab = Resources.Load<GameObject>(ResourceUIPath + "/Equipment");

            ToolTip_Prefab = Resources.Load<GameObject>("UI/ItemToolTip");

            hpeffect.InitAction += () => { return hpBar.fillAmount; };
            mpeffect.InitAction += () => { return mpBar.fillAmount; };
            foreach (SkillSlot go in SkillSlotTransform.GetComponentsInChildren<SkillSlot>())
            {
                SkillSlots_.Add(go);
            }
            EquipUIButton.onClick.AddListener(() =>equipmentui = OBPool.Instance.GetUI(Equipment_UI_Prefab, equipmenttransform.position, equipmenttransform).GetComponent<EquipmentUI>());
            InventoryUIButton.onClick.AddListener(() =>inventoryui= OBPool.Instance.GetUI(Inventory_UI_Prefab, inventorytransform.position,inventorytransform).GetComponent<InventoryUI>());
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void InteractionOn()
    {
        if (!Interaction.activeSelf)
            Interaction.SetActive(true);
    }
    public void InteractionOff()
    {
        if(Interaction.activeSelf)
        Interaction.SetActive(false);
    }
    IEnumerator Start()
    {
        //캐릭터 생성 기다리기
        yield return new WaitUntil(() => GameManager.Instance.playerStatus != null);
        GameManager.Instance.playerStatus.HpChangedAction += SetHpUI;
        GameManager.Instance.playerStatus.MpChangedAction += SetMpUI;
        GameManager.Instance.playerStatus.ExpChangedAction += SetExp;
        LevelText.text = "1";
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryui == null || !inventoryui.gameObject.activeSelf)
            {
                inventoryui=  OBPool.Instance.GetUI(Inventory_UI_Prefab, inventorytransform.position, inventorytransform).GetComponent<InventoryUI>();
            }

            else if (inventoryui.gameObject.activeSelf)
            {
                OBPool.Instance.UIdespawn(Inventory_UI_Prefab);
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (equipmentui == null || !equipmentui.gameObject.activeSelf)
            {
                equipmentui = OBPool.Instance.GetUI(Equipment_UI_Prefab, equipmenttransform.position, equipmenttransform).GetComponent<EquipmentUI>();
            }

            else if (equipmentui.gameObject.activeSelf)
            {
                OBPool.Instance.UIdespawn(Equipment_UI_Prefab);

            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.Instance.QuickslotItemUse(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Inventory.Instance.QuickslotItemUse(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Inventory.Instance.QuickslotItemUse(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Inventory.Instance.QuickslotItemUse(3);
        }
        

    }
    public void InitQuickslot(int num)
    {
        stateinfoui.quickslots_[num].Init();
    }
    public void UI_SetSkill(List<SkillInstance> skillInstances)
    {
        for (int i = 0; i < skillInstances.Count; i++)
        {
            SkillSlots[i].Init(skillInstances[i],i);
        }
    }

    public void UI_ShowDamage(float damage, Vector3 position)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
        OBPool.Instance.Get<DamageText>(damageTextPrefab, out DamageText damageText, transform);
        damageText.transform.position = screenPos;
        damageText.SetText(damage);
    }

    public void UI_ShowMessage(string message)
    {
        dialogText.gameObject.SetActive(true);
        dialogText.text = message;
        dialogText.DOFade(1, fadeDuration).OnComplete(() =>
        {
            // 일정 시간 후 텍스트 페이드 아웃
            DOVirtual.DelayedCall(displayTime, () =>
            {
                dialogText.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    dialogText.gameObject.SetActive(false);  // 페이드 아웃 후 비활성화
                });
            });
        });
    }

    public void SetHpUI(float curHp, float maxHp)
    {
        hpeffect.gameObject.SetActive(true);
        hpText.text = $"{curHp} / {maxHp}";
        hpBar.fillAmount = curHp / maxHp;
    }
    public void SetMpUI(float curMp, float maxMp)
    {
        mpeffect.gameObject.SetActive(true);
        mpText.text = $"{curMp} / {maxMp}";
        mpBar.fillAmount = curMp / maxMp;
    }

    public void SetExp(float ratio)
    {
        expSlider.value = ratio * 100;
    }
    public void setMaxExp(float ratio)
    {
        expSlider.maxValue = ratio;
    }
}
