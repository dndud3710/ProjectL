using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPhotonManager : MonoBehaviourPunCallbacks
{
    [Header("서버 로비 이름")]
    [SerializeField] string ServerLobbyName;
    [Header("서버 룸 이름")]
    [SerializeField] string ServerRoomName;
    [Header("다음으로 넘어갈 씬 이름")]
    [SerializeField] string LoadSceneName;

    public static LoginPhotonManager Instance
    {
        get; private set;
    }
    public Dictionary<string, RoomOptions> MakeRooms { get; private set; }
    TypedLobby typedlobby;
    private void Awake()
    {
        Instance = this;
        Initialize();

    }
    LoginUIManager uimanager;
    private void Start()
    {
        uimanager = GameObject.Find("UIManager").GetComponent<LoginUIManager>();
    }
    private void Initialize()
    {
        MakeRooms = new Dictionary<string, RoomOptions>();
        //원래는 json파일을 이용하여 자동화
        MakeNewRoom(ServerRoomName, 4, true, true);
        typedlobby = new TypedLobby(ServerLobbyName, LobbyType.Default);
    }
    private void MakeNewRoom(string name, int maxplayers, bool isopen, bool isvisible)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxplayers;
        roomOptions.IsOpen = isopen;
        roomOptions.IsVisible = isvisible;
        MakeRooms.Add(name, roomOptions);
    }



    #region login

    public void LogInButton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    #region PunCallback Life
    public override void OnConnectedToMaster()
    {
        Instantiate(Resources.Load<GameObject>("ResourceManager"));
        PlayerPrefs.SetInt("FirstLogin", 0);
        PhotonNetwork.JoinLobby(typedlobby);
        StartGameScene();
    }
    private void StartGameScene()
    {
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRoom(ServerRoomName);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(ServerRoomName, roomOptions: MakeRooms[ServerRoomName]);

    }
    public override void OnJoinedRoom()
    {
        Player localplayer = PhotonNetwork.LocalPlayer;


        ExitGames.Client.Photon.Hashtable ClassProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"Class",uimanager.characterselect.CurrentClass }

        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(ClassProperties);
        PhotonNetwork.LocalPlayer.NickName = uimanager.characterselect.GetNickName();
        LoadManager.Instance.LoadScene(LoadSceneName);
        //SceneManager.LoadScene("Loading");
    }

    public override void OnCreatedRoom()
    {
        //SceneManager.LoadScene("Loading");
    }
    #endregion
    #endregion
}
