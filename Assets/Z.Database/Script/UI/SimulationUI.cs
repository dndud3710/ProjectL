using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    GameNPC CurNPC;
    [SerializeField] Transform dungeonPanelPos;
    [SerializeField] Transform shoptransform;
    [SerializeField] Transform inventoryTransform;
    public static SimulationUI Instance { get; private set; }
    /// <summary>
    /// 퀘스트인지 상점인지, 그냥대화인지 여기서 판별후
    /// 해당하는 UI를 띄워준다.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public Action ExitHandler;
    private GameObject ChoiceShopPanel;
    private GameObject ChoiceTalkPanel;
    private GameObject ChoiceDungeonPanel;

    private GameObject DungeonMatchingPanel;

    private GameObject ShopUI;

    public InventoryUI inventoryui { get; private set; }

    npcType_ npctype;
    [SerializeField] GameDialogue talkmanager;
    Transform choiceInterfaceParent; //choicepanel transform

    Image npcClassImage; //NPC 직업 이미지

    List<GameObject> InitList;

    private void Load()
    {
        choiceInterfaceParent = transform.Find("ChoiceInterface");
        ChoiceShopPanel = Resources.Load<GameObject>("UI/SimulationUI/Button/NpcShopChoicePanel");
        ChoiceTalkPanel = Resources.Load<GameObject>("UI/SimulationUI/Button/NpcTalkChoicePanel");
        ChoiceDungeonPanel = Resources.Load<GameObject>("UI/SimulationUI/Button/NpcDungeonChoicePanel");
        DungeonMatchingPanel = Resources.Load<GameObject>("UI/DungeonMatchUI");
        ShopUI = Resources.Load<GameObject>("UI/ShopUI");
        InitList = new List<GameObject>();
    }
    private void OnEnable()
    {
    }
    private void Start()
    {
    }
    GameObject dungeonpanel;

    public void setNpcUI(TalkDialogue talk, GameNPC npc)
    {
        GameObject g = Instantiate(ChoiceTalkPanel, choiceInterfaceParent);
        //g.GetComponent<Button>().onClick.AddListener()
        InitList.Add(g);
        talkmanager.setDialogue(talk);
    }
    public void setShopNpcUI(TalkDialogue talk, GameNPC npc)
    {
        GameObject g = Instantiate(ChoiceShopPanel, choiceInterfaceParent);
        g.GetComponent<Button>().onClick.AddListener(() =>
        {
            g.GetComponent<Button>().interactable = false;
            IMakeUI sho;
            npc.MakeUI(ShopUI, shoptransform,out sho,()=>
            inventoryui = OBPool.Instance.GetUI(Resources.Load<GameObject>("UI/InventoryUI"), inventoryTransform.position, inventoryTransform).GetComponent<InventoryUI>());

            ExitHandler +=  sho.ObjectDestroy;
            ExitHandler += () => OBPool.Instance.UIdespawn(Resources.Load<GameObject>("UI/InventoryUI"));
        });
        InitList.Add(g);
        talkmanager.setDialogue(talk);
    }
    public void setDungeonNpcUI(TalkDialogue talk, GameNPC npc)
    {
        GameObject g = Instantiate(ChoiceDungeonPanel, choiceInterfaceParent);
        Button btn = g.GetComponent<Button>();
        DungeonNPC dnpc = npc as DungeonNPC;
        btn.onClick.AddListener(() =>
        {
            IMakeUI dongeon;

            g.GetComponent<Button>().interactable = false;
            npc.MakeUI(DungeonMatchingPanel, dungeonPanelPos, out dongeon);
               
            DungeonMatchingUI mu_ = dongeon as DungeonMatchingUI;
            mu_.Init(dnpc.getDungeon());
            ExitHandler += () => dongeon.ObjectDestroy();
        });
        InitList.Add(g);
        talkmanager.setDialogue(talk);
    }
    public void setPortalNpcUI(TalkDialogue talk, GameNPC npc)
    {
        GameObject g = Instantiate(Resources.Load<GameObject>("UI/SimulationUI/Button/NpcPortalChoicePanel"), choiceInterfaceParent);
        g.GetComponent<Button>().onClick.AddListener(() =>
        {
            LoadManager.Instance.LoadScene("Map");
        });
        InitList.Add(g);


        talkmanager.setDialogue(talk);
    }

    //public void SetUi(TalkDialogue talk, npcType_ type, GameNPC npc)
    //{
    //    CurNPC = npc;
    //    npctype = type;
    //    if (type == npcType_.talk) //npc
    //    {
    //        GameObject g = Instantiate(ChoiceTalkPanel, choiceInterfaceParent);
    //        //g.GetComponent<Button>().onClick.AddListener()
    //        InitList.Add(g);
    //    }
    //    else if (type == npcType_.shop) // 상점 npc
    //    {
    //        GameObject g = Instantiate(ChoiceShopPanel, choiceInterfaceParent);
    //        ShopNPC snpc = npc as ShopNPC;
    //        g.GetComponent<Button>().onClick.AddListener(() =>
    //        {
    //            g.GetComponent<Button>().interactable = false;
    //            ShopUI sh = Instantiate(ShopUI, shoptransform).GetComponent<ShopUI>();
    //            inventoryui = OBPool.Instance.GetUI(Resources.Load<GameObject>("UI/InventoryUI"), inventoryTransform.position, inventoryTransform).GetComponent<InventoryUI>();
    //            sh.SetShopItem(snpc.shopitem_);
    //            ExitHandler += () => Destroy(sh.gameObject);
    //            ExitHandler += () => OBPool.Instance.UIdespawn(Resources.Load<GameObject>("UI/InventoryUI"));
    //        });
    //        InitList.Add(g);
    //    }
    //    else if (type == npcType_.Dungeon) //던전 npc
    //    {
    //        GameObject g = Instantiate(ChoiceDungeonPanel, choiceInterfaceParent);
    //        Button btn = g.GetComponent<Button>();
    //        DungeonNPC dnpc = npc as DungeonNPC;
    //        btn.onClick.AddListener(() =>
    //        {
    //            g.GetComponent<Button>().interactable = false;
    //            dungeonpanel = OBPool.Instance.Get(DungeonMatchingPanel, dungeonPanelPos.position, Quaternion.identity, transform);
    //            dungeonpanel.GetComponent<DungeonMatchingUI>().Init(dnpc.getDungeon());
    //            ExitHandler += () => OBPool.Instance.Despawn(dungeonpanel);
    //        });
    //        InitList.Add(g);
    //    }
    //    else if (type == npcType_.Portal)
    //    {
    //        GameObject g = Instantiate(Resources.Load<GameObject>("UI/SimulationUI/Button/NpcPortalChoicePanel"), choiceInterfaceParent);
    //        g.GetComponent<Button>().onClick.AddListener(() =>
    //        {
    //            LoadManager.Instance.LoadScene("Map");
    //        });
    //        InitList.Add(g);
    //    }
    //    talkmanager.setDialogue(talk);
    //}



    private void OnDisable()
    {
        foreach (GameObject go in InitList)
        {
            Destroy(go);
        }
        InitList.Clear();
        CurNPC = null;
        inventoryui = null;
        ExitHandler?.Invoke();
        ExitHandler = null;
        if (GameManager.Instance.playerStatus != null)
            GameManager.Instance.playerStatus.NpcTalking = false;
    }
}
