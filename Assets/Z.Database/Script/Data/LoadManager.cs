
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public enum UILoadType
    {
        playerui,
        simulationui
    }
    /// <summary>
    /// addressable 에서 Material은 M_, Texture는 T_, 붙이기 Label
    /// </summary>
    public static LoadManager Instance { get; private set; }

    public Action<bool> UiSetActive;

    GameObject playerui_Prefab;
    GameObject simulationui_Prefab;

    GameObject playerui;
    GameObject simulationui;

    private Dictionary<string, GameObject> UiOnOff;
    private List<string> keys;

    private PhotonNetWorkPool photonpool;
    public PhotonNetWorkPool photonObpool => photonpool;
    public Sprite transparencySprite {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitList();
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
       
    }
    private void InitList()
    {
        playerui_Prefab = Resources.Load<GameObject>("UI/PlayerUI");
        simulationui_Prefab = Resources.Load<GameObject>("UI/SimulationUI");
        transparencySprite = Resources.Load<Sprite>("Data/Image/transparency");
        UiOnOff = new Dictionary<string, GameObject>();
        keys = new List<string>();
        photonpool = new PhotonNetWorkPool();
        PhotonNetwork.PrefabPool = photonpool;
    }

    public string SceneName_;
    public void LoadScene(string sceneName)
    {
        SceneName_ = sceneName;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        PhotonNetwork.LoadLevel("Loading");
    }

    #region UILOad 관련
    /// <summary>
    /// 게임 처음 시작시 로드할 UI
    /// </summary>
    public void GmaeStartLoadUI()
    {
        NPCManager.Instance.Init();
        Inventory.Instance.Init();
       

    }

    public void LoadUI()
    {
        playerui = OBPool.Instance.Get<PlayerUIManager>(playerui_Prefab);
        simulationui = OBPool.Instance.Get<SimulationUI>(simulationui_Prefab);
        keys.Clear();
        UiOnOff.Clear();
        keys.Add(playerui.name);
        keys.Add(simulationui.name);

        UiOnOff.Add(keys[0], playerui);
        UiOnOff.Add(keys[1], simulationui);

        changeUI(UILoadType.playerui);
    }

    public void changeUI(UILoadType uitype)
    {
        string str = keys[(int)uitype];



        foreach (var item in UiOnOff.Keys)
        {
            if (item==str)
            {
                OBPool.Instance.Get(UiOnOff[item]);
            }
            else
            {
               OBPool.Instance.Despawn(UiOnOff[item]);
            }
        }
    }


    #endregion

}
