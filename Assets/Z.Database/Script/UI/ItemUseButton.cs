using DG.Tweening.Plugins;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUseButton : MonoBehaviour
{
    enum ItemUseType
    {
        equip,
        use,
        etc,
        equipment
    }
    [SerializeField] ItemUseType itemUseType;
    /// <summary>
    /// ���� 100 text�ϳ��� 35�̱⶧���� 35�ǹ���� �÷�����
    /// </summary>
    public Action EquipUseHandler;//��� ��� �ڵ鷯
    public Action UseHandler;//�Һ�ǰ ��� �ڵ鷯
    public Action DropHandler;//������ ������ �ڵ鷯

    GameObject UseItemSelectPanel; //������ ��� ����â ������
    GameObject UseItemSelectFrame;

    Dictionary<string ,Button> ButtonList;

    string[] equipstr = { "�����ϱ�", "������" };
    string[] equipmentstr = { "�����ϱ�" };
    string[] usestr = { "����ϱ�", "������" };
    string[] etcstr = { "������" };
    Transform frame;
    private void Awake()
    {
        ButtonList = new Dictionary<string, Button>();
        UseItemSelectFrame = Resources.Load<GameObject>("UI/Button/UseItemSelectFrame");

        string[] str = null;

        for(int i = 0; i < transform.childCount; i++)
        {
            Transform go = transform.GetChild(i);
            //go.AddComponent<SelectButtonFrameEnable>();
            switch (itemUseType)
            {
                case ItemUseType.equip:
                    str = equipstr;
                    go.GetChild(0).GetComponent<TextMeshProUGUI>().text = str[i];// �����ϱ� ������
                    
                    go.GetComponent<SelectButtonFrameEnable>().frameInit += FrameOn;
                    ButtonList.Add(str[i], go.gameObject.GetComponent<Button>());
                    break;
                case ItemUseType.use:
                    str = usestr;
                    go.GetChild(0).GetComponent<TextMeshProUGUI>().text = str[i];// ����ϱ� ������
                    go.GetComponent<SelectButtonFrameEnable>().frameInit += FrameOn;
                    ButtonList.Add(str[i], go.gameObject.GetComponent<Button>());
                    break;
                case ItemUseType.etc:
                    str = etcstr;
                    go.GetChild(0).GetComponent<TextMeshProUGUI>().text = str[i];// ������
                    go.GetComponent<SelectButtonFrameEnable>().frameInit += FrameOn;
                    ButtonList.Add(str[i], go.gameObject.GetComponent<Button>());
                    break;
                    case ItemUseType.equipment:
                    str = equipmentstr;
                    go.GetChild(0).GetComponent<TextMeshProUGUI>().text = str[i];// �����ϱ�
                    go.GetComponent<SelectButtonFrameEnable>().frameInit += FrameOn;
                    ButtonList.Add(str[i], go.gameObject.GetComponent<Button>());
                    break;
            }
            go.GetComponent<SelectButtonFrameEnable>().frameoff += PlayerUIManager.Instance.inventoryUI.DespawnItemSelectPanel;
        }
        frame = Instantiate(UseItemSelectFrame, ButtonList[str[0]].gameObject.transform).transform;

    }
    public void Init(GameItem item,int slotnum,Action CallBack=null)
    {

        switch (itemUseType)
        {
            case ItemUseType.equip:
                GameEquipItem eq = item as GameEquipItem;
                ButtonList[equipstr[0]].onClick.AddListener(() => eq.Use(slotnum));
                ButtonList[equipstr[0]].onClick.AddListener( PlayerUIManager.Instance.inventoryUI.DespawnItemSelectPanel);
                break;
            case ItemUseType.use:
                GameUseItem use = item as GameUseItem;
                ButtonList[usestr[0]].onClick.AddListener(() => use.Use());
                ButtonList[usestr[0]].onClick.AddListener(PlayerUIManager.Instance.inventoryUI.DespawnItemSelectPanel);
                break;
            case ItemUseType.etc:
                break;
            case ItemUseType.equipment:
                GameEquipItem eqm = item as GameEquipItem;
                Debug.Log("��");
                ButtonList[equipmentstr[0]].onClick.AddListener(() => Inventory.Instance.EquiItemUnEquip(eqm));
                ButtonList[equipmentstr[0]].onClick.AddListener(() => Debug.Log("����"));
                ButtonList[equipmentstr[0]].onClick.AddListener(PlayerUIManager.Instance.equipmentUI.DespawnItemSelectPanel);
                break;
        }
        CallBack?.Invoke();
    }
    private void OnClickUse()
    {

    }
    private void FrameOn(Transform tr)
    {
        if (!frame.gameObject.activeSelf)
            frame.gameObject.SetActive(true);
        frame.transform.SetParent(tr);
        frame.transform.localPosition = Vector3.zero;
    }
    private void FrameOff()
    {
        if (frame.gameObject.activeSelf)
            frame.gameObject.SetActive(false);
    }
    private void OnDisable()
    {

    }
}
