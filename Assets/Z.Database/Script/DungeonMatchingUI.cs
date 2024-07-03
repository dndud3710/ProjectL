using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonMatchingUI : MonoBehaviourPunCallbacks , IMakeUI
{

    string prevRoomName;
    ///1. 던전 이미지
    ///2. 던전 이름
    ///3. 던전 적정인원
    ///4. 매칭 시작 버튼

    [SerializeField] Image dungeonimage;
    [SerializeField] TextMeshProUGUI dungeonName;
    [SerializeField] TextMeshProUGUI dungeonPlayerNum;
    [SerializeField] TextMeshProUGUI Matchingtext;
    [SerializeField] TextMeshProUGUI MatchCount;
    [SerializeField] Button MatchingStartButton;
    [SerializeField] Transform matchinguiTransform;
    GameObject matchingUI;
    //1. 오크의 숲
    Dungeon dungeon;

    StringBuilder str;
    bool matchStart;
    bool Matching;

    int curPlayerNum;
    private void Awake()
    {

        str = new StringBuilder();
        matchingUI= Resources.Load<GameObject>("UI/MatchingCompletedUI");
      
    }
    public override void OnEnable()
    {
        base.OnEnable();
        curPlayerNum = 0;
        prevRoomName = PhotonNetwork.CurrentRoom.Name;


    }
    public void Init(Dungeon dungeon)
    {
        this.dungeon = dungeon;
        dungeonimage.sprite = dungeon.DungeonImage;

        MatchingStartButton.onClick.AddListener(() => MatchingStart(dungeon.DungeonName));
    }
    string dungeonname;
    string roomName;
    public void MatchingStart(string dungeonname)
    {
        ExitGames.Client.Photon.Hashtable readyProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"DungeonName",dungeonname },
            {"DungeonReady",true },
            {"MaxHP",GameManager.Instance.playerStatus.MaxHP},
            {"Class",GameManager.Instance.playerStatus.playerClass},
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(readyProperties);
        matchStart = true;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (Matching == true) return;
        string dName_ = string.Empty;

        foreach(Player g in PhotonNetwork.PlayerList)
        {
            if (g.CustomProperties["DungeonName"] == null) return;
            if (g.CustomProperties["DungeonReady"] == null) return;
            string curDName_ = (string)g.CustomProperties["DungeonName"];
            if (dName_ != string.Empty)
            {
                if (dName_ == curDName_)
                {
                    dName_ = curDName_;
                }
                else return;
            }
            curPlayerNum++;
            MatchCount.text = $"{curPlayerNum} / {PhotonNetwork.PlayerList.Length}";
        }
        Matching = true;
        MakeMachingUI();

    }
    public void MakeMachingUI()
    {
        GameObject g= Instantiate(matchingUI, matchinguiTransform);
        MatchingUI mui =g.GetComponent<MatchingUI>();
        mui.Init(PhotonNetwork.PlayerList.Length,dungeon.DungeonSceneName);
        mui.villageName = prevRoomName;
        mui.ExitHandler += (() => matchStart = false);
        mui.ExitHandler += (() => Matching = false);
        mui.ExitHandler += (() => {
            curPlayerNum = 0;
            Matchingtext.text = "";
            Matchingtext.text = "";
        });
    }
    string dot = "."; 
    float T;
    int c;
    private void Update()
    {
        if (matchStart)
        {
            T = T + Time.deltaTime;
            if (T > 0.2f)
            {
                str.Append("대기중");
                for (int i = 0; i < c; i++)
                    str.Append(dot);
                Matchingtext.text = str.ToString();
                str.Clear();
                if (c++ > 4)
                    c = 1;
                T = 0;
            }
           
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }

    public void ObjectDestroy()
    {
        OBPool.Instance.Despawn(this.gameObject);
    }
}
