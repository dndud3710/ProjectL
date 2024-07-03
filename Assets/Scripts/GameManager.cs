using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public Transform playerSpawnPoint;

    [Header("Player Settings")]
    public Class @class;
    public PlayerStatus playerPrefab;
    [HideInInspector] public PlayerAttack playerAttack;
    [HideInInspector] public PlayerStatus playerStatus;
    [HideInInspector] private PlayerCam playerCam;
    private Dictionary<string, SkillInfo> skillInfoDictionary;
    private Dictionary<EffectType, StatusEffectData> effectDataDictionary;
    [Space(20)]

    //UI Manager Field
    [Header("UIManager")]
    public DamageText damageTextPrefab;
    private Canvas mainCanvas;
    public int selectedSkillIndex = 1;

    private FactoryManager factorymanager_;
    public FactoryManager factory => factorymanager_;

    private void Awake()
    {
        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);
            if(mainCanvas == null)
                mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            SceneManager.sceneLoaded += SceneLoadedAction;
            factorymanager_ = new FactoryManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SceneLoadedAction(Scene scene, LoadSceneMode sceneMode)
    {
        if(mainCanvas == null)
            mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (scene.name != "Map")
            return;
        if (playerStatus != null)
        {
            var spawnPoint = GameObject.Find("CharacterSpawnObject").transform.position;
            playerStatus.transform.position = spawnPoint;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoadedAction;
    }
    IEnumerator Start()
    {
        LoadData();

        yield return new WaitForSeconds(1);
        
        InstantiatePlayer();

        yield return new WaitUntil(() => playerStatus.TryGetComponent(out playerAttack));

    }

    private void InstantiatePlayer()
    {
        var playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        var localPlayerClass = (Class)playerProperties["Class"];

        var player = PhotonNetwork.Instantiate($"Prefabs/Player{(int)localPlayerClass}", playerSpawnPoint.position, Quaternion.identity);
        playerStatus = player.GetComponent<PlayerStatus>();

        playerStatus.playerClass = localPlayerClass;

        playerStatus.PlayerSet = true;
    }

    // 스크립터블 오브젝트 로드
    private void LoadData()
    {
        skillInfoDictionary = new Dictionary<string, SkillInfo>();
        effectDataDictionary = new Dictionary<EffectType, StatusEffectData>();

        SkillInfo[] skillInfos = Resources.LoadAll<SkillInfo>("SO/Skills");
        StatusEffectData[] effectDatas = Resources.LoadAll<StatusEffectData>("SO/Rune");

        foreach (var skillInfo in skillInfos)
        {
            skillInfoDictionary.Add(skillInfo.name, skillInfo);
        }

        foreach (var effectData in effectDatas)
        {
            effectDataDictionary.Add(effectData.EffectType, effectData);
        }

    }

    public SkillInfo GetSkillInfo(string name)
    {
        if (skillInfoDictionary.TryGetValue(name, out SkillInfo value))
        {
            return value;
        }
        Debug.Log("Skill Scriptable Objcet 이름 확인" + name + "Doesn't exist");
        return null;
    }

    public StatusEffectData GetEffectData(EffectType effectType)
    {
        if (effectDataDictionary.TryGetValue(effectType, out StatusEffectData value))
        {
            return value;
        }

        Debug.Log("Effect Scriptable Objcet 이름 확인" + effectType + "Doesn't exist");
        return null;
    }

    public PhotonView GetPlayerPhotonView(int viewId)
    {
        var targetPlayer = PhotonView.Find(viewId);

        if (targetPlayer != null)
        {
            return targetPlayer;
        }
        return null;
    }

    ////UI Manager
    public void UI_ShowDamage(float damage, Vector3 position)
    {
        Vector2 mousePosition = Camera.main.WorldToScreenPoint(position);
        Vector3 screenPosition = mousePosition + Vector2.up * 2;
        var damageText = Instantiate(damageTextPrefab, mainCanvas.transform);
        damageText.transform.position = screenPosition;
        damageText.SetText(damage);
    }

    public void SetSelectedSkillIndex(int index)
    {
        if (index < 1 || index > 4)
            return;
        selectedSkillIndex = index;
    }
}
