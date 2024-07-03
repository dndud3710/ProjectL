using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    GameObject HpBarObject;

    public GameObject portalPrefab;

    [Header("몬스터 스폰위치")]
    [SerializeField] Transform PlayerSpawnPoint;
    [SerializeField] Transform BossSpawnPoint;
    [SerializeField] Transform[] monsterSpawnPoints;
    [Header("UI 스폰 위치")]
    [SerializeField] Transform TeamHpPoint;
    [SerializeField] Transform BossHpPoint;
    [Header("Step")]
    [SerializeField] private int curStep = 0;
    [SerializeField] private int maxStep;
    [SerializeField] List<GameObject> Walls;
    [SerializeField] List<int> MonsterCount;
    [SerializeField] List<int> MonsterArea;

    private List<GameObject> monsterList;
    private DeathKnight boss;

    public List<PlayerStatus> playerList;

    string mobName;
    public Hpbar bossHpbar { get; private set; }
    float bossMaxHP;
    string bossName;
    public Dictionary<int, TeamHpUI> teamhpList { get; private set; }
    int[] PlayerHp;

    public static DungeonManager Instance { get; private set; }
    string bossname;
    private void Awake()
    {
        Instance = this;
        HpBarObject = Resources.Load<GameObject>("UI/TeamHPBar");
        teamhpList = new Dictionary<int, TeamHpUI>();
        monsterList = new();
        if (PhotonNetwork.IsMasterClient)
        {
            playerList = new();
        }
        GameObject g = Resources.Load<GameObject>("Prefabs/Enemy/Mob/TestRagDollMob2");
        mobName = g.name;
        LoadManager.Instance.photonObpool.AddResource(g,20);
    }
    private void Start()
    {

        GameManager.Instance.playerStatus.transform.position = PlayerSpawnPoint.position;
        LoadManager.Instance.LoadUI();

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            int num = PhotonNetwork.PlayerList[i].ActorNumber;
            int classAvatar = -1;
            float Maxhp = 0;
            string nickname = "";
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == num)
                {
                    classAvatar = (int)p.CustomProperties["Class"];
                    Maxhp = (float)p.CustomProperties["MaxHP"];
                    nickname = p.NickName;
                }
            }

            teamhpList.Add(num, Instantiate(HpBarObject, TeamHpPoint).GetComponent<TeamHpUI>());

            Sprite sprite = null;
            if (classAvatar == 0) //warrior
                sprite = Resources.Load<Sprite>("WarriorIcon");
            else if (classAvatar == 1)
                sprite = Resources.Load<Sprite>("GunnerIcon");
            else if (classAvatar == 2)
                sprite = Resources.Load<Sprite>("MageIcon");
            else Debug.LogError("classAvatar 미설정");
            teamhpList[num].Init(Maxhp, GameManager.Instance.playerStatus.CurHP, nickname, sprite);

            if (PhotonNetwork.IsMasterClient)
            {
                var go = GetGameObjectByActorNumber(PhotonNetwork.PlayerList[i].ActorNumber);
                playerList.Add(go.GetComponent<PlayerStatus>());
            }
        }
        PlayerHp = new int[PhotonNetwork.CurrentRoom.PlayerCount];

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnBoss());
        }
    }

    private GameObject GetGameObjectByActorNumber(int actorNumber)
    {
        foreach (PhotonView photonView in FindObjectsOfType<PhotonView>())
        {
            if (photonView.Owner != null && photonView.Owner.ActorNumber == actorNumber)
            {
                return photonView.gameObject;
            }
        }
        return null;
    }

    IEnumerator SpawnBoss()
    {

        GameObject bossObj = PhotonNetwork.Instantiate("Prefabs/Enemy/Boss", BossSpawnPoint.position, Quaternion.identity);
        yield return null;
        if (bossObj != null)
        {
            boss = bossObj.GetComponent<DeathKnight>();
        }
        ExitGames.Client.Photon.Hashtable readyProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"bossMaxHp",boss.MaxHP },
            {"bossName",boss.BossName }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(readyProperties);
        yield return new WaitForSeconds(1f);
    }
    IEnumerator SpawnMonsterCoroutine(int num)
    {
        if (PhotonNetwork.IsMasterClient == false)
            yield break;

        monsterList.Clear();

        for (int i = 0; i < MonsterCount[num]; i++)
        {
            Vector3 pos = Random.insideUnitSphere * MonsterArea[num] + monsterSpawnPoints[num].position;
            pos.y = monsterSpawnPoints[num].position.y;

            GameObject go = PhotonNetwork.Instantiate(mobName, pos, Quaternion.identity);
            monsterList.Add(go);
            yield return null;
        }
    }
    public void SpawnMonster(int num)
    {
        curStep = num;
        switch (num)
        {
            case 0:
                StartCoroutine(SpawnMonsterCoroutine(num));
                break;
            case 1:
                StartCoroutine(SpawnMonsterCoroutine(num));
                break;
            case 2:
                bossMaxHP = (float)PhotonNetwork.CurrentRoom.CustomProperties["bossMaxHp"];
                bossname = (string)PhotonNetwork.CurrentRoom.CustomProperties["bossName"];
                bossHpbar = Instantiate(Resources.Load<GameObject>("UI/BossHpBar"), GameObject.Find("Canvas").transform).GetComponent<Hpbar>();
                bossHpbar.Init(bossMaxHP, bossMaxHP, bossname);
                if (boss != null)
                {
                    boss.StartPattern();
                }
                else
                    Debug.Log("Boss is Null");
                Debug.Log(boss);

                break;
        }
    }

    public void RemoveMonster(GameObject go)
    {
        monsterList.Remove(go);
        if (monsterList.Count <= 0)
        {
            Walls[curStep]?.SetActive(false);
        }
    }

    public void DungeonClear()
    {
        Instantiate(portalPrefab, BossSpawnPoint.position, Quaternion.Euler(-90, 0, 0));

        GameManager.Instance.playerStatus.Revival();
    }
}
