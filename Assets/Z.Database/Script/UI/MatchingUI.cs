using Newtonsoft.Json.Serialization;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingUI : MonoBehaviourPunCallbacks
{
    private Action ReadyHandler;
    private Action CancelHandler;
    public Action ExitHandler;
    public string villageName;

    ///1. ��Ī �ִ� �ο�
    ///2. �غ� ��� ��ư
    ///3. �غ� �ּҿ� ���� Image��ȭ
    ///3.1 readyImage ���׶��
    ///3.2 cancelImage Xǥ��

    GameObject MatchPlayerIcon;


    [SerializeField] Transform PlayerCountTransform; //�÷��̾� ������ parent
    List<GameObject> PlayerImage;
    [SerializeField] Button ready;
    [SerializeField] Button Cancel;

    private string scenename;
    private void Awake()
    {
        PlayerImage = new List<GameObject>();
        MatchPlayerIcon = Resources.Load<GameObject>("UI/MatchPlayerIcon");
        ready.onClick.AddListener(() => ReadyHandler?.Invoke());
        Cancel.onClick.AddListener(() => CancelHandler?.Invoke());
        ready.onClick.AddListener(() => { ready.interactable = false; Cancel.interactable = false; });
        Cancel.onClick.AddListener(() => { ready.interactable = false; Cancel.interactable = false; }   );

    }

    public void Init(int playercount,string scenename)
    {
        for (int i = 0; i < playercount; i++)
        {
            PlayerImage.Add(Instantiate(MatchPlayerIcon, PlayerCountTransform));
        }
        this.scenename = scenename;
        //�غ�Ϸ�, ��ǥ�Ϸ�, image�Ϸ�
        ReadyHandler += (ReadyProperties);
        ReadyHandler += (() => PlayerImage[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.Find("ReadyImage").gameObject.SetActive(true));

        //�غ����, ��ǥ�Ϸ�, image�Ϸ�
        CancelHandler += (CancelProperties);
        CancelHandler += (() => PlayerImage[PhotonNetwork.LocalPlayer.ActorNumber-1].transform.Find("CancelImage").gameObject.SetActive(true));

    }
    private void ReadyProperties()
    {
        ExitGames.Client.Photon.Hashtable readyProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Ready", true },
            { "Confirm",true}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(readyProperties);
    }
    private void CancelProperties()
    {
        ExitGames.Client.Photon.Hashtable readyProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Ready", false },
            { "Confirm",true}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(readyProperties);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if ((bool)targetPlayer.CustomProperties["Ready"])
        {
            PlayerImage[targetPlayer.ActorNumber - 1].transform.Find("ReadyImage").gameObject.SetActive(true);
        }
        else
        {
            PlayerImage[targetPlayer.ActorNumber - 1].transform.Find("CancelImage").gameObject.SetActive(true);
        }
        foreach( Player g in PhotonNetwork.PlayerList)
        {
            if (g.CustomProperties["Confirm"] == null) return;
        }

        foreach (Player g in PhotonNetwork.PlayerList)
        {
            if ((bool)g.CustomProperties["Ready"] == false)
            {
                ReturnToserver();
                return;
            }
        }
        StartCoroutine(StartDungeon());
    }
    private void ReturnToserver()
    {
        ExitHandler?.Invoke();
        Destroy(gameObject);
    }
     
    private IEnumerator StartDungeon()
    {
        yield return new WaitForSeconds(1f);
        LoadManager.Instance.LoadScene(scenename);
    }
}
