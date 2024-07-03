using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviourPun, IEffectable, IPunObservable
{
    // 직업, 체력, 장비 같은 CustomProperties로 세팅할 것들을
    #region Status
    [Header("플레이어 정보")]
    public Class playerClass;
    private int level = 1;
    public Status playerStat;
    private PlayerAttack playerAttack;
    private PlayerMove playerMove;
    private float curHp;
    private float maxHp;
    private float curMp;
    private float maxMp;
    private float curExp;
    private float maxExp;
    private bool alive = true;
    [HideInInspector] public bool PlayerSet = false;

    private bool canMove = true;
    public bool CanMove { get => canMove; }
    public bool Alive { get => alive; }
    public float CurHP { get => curHp; set => curHp = value; }
    public float MaxHP { get => maxHp; set => maxHp = value; }
    public float CurMP { get => curMp; set => curMp = value; }
    public float MaxMP { get => maxMp; set => maxMp = value; }

    public event Action<float, float> HpChangedAction;
    public event Action<float, float> MpChangedAction;
    public event Action<float> ExpChangedAction;

    public event Action DieAction;

    public bool NpcTalking { get; set; }
    public bool Parrying { get; set; }

    #region Components
    public PlayerCam playerCam;
    private Animator animator;
    #endregion

    public void GainHP(float hp)
    {
        CurHP += hp;
        if (CurHP >= MaxHP)
        {
            CurHP = MaxHP;
        }
        SetHpUI();
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.teamhpList[PhotonNetwork.LocalPlayer.ActorNumber].setHp(CurHP);
    }
    public void GainMP(float mp)
    {
        CurMP += mp;
        if(CurMP >= MaxMP)
        {
            CurMP = MaxMP;
        }
        SetMpUI();
    }
    public void GainExp(float exp)
    {
        curExp += exp;
        if (curExp >= maxExp)
        {
            curExp = 0;
            LevelUp();
        }
        
        ExpChangedAction?.Invoke(curExp / maxExp);
    }
    private void LevelUp()
    {
        level++;
        maxExp += 100;
        MaxHP += 1000;
        MaxMP += 1000;
        CurHP = MaxHP;
        CurMP = MaxMP;
        playerStat.LevelUp();
        SetHpUI();
        SetMpUI();
    }
    private void SetHpUI()
    {
        HpChangedAction?.Invoke(curHp, maxHp);
    }
    private void SetMpUI()
    {
        MpChangedAction?.Invoke(curMp, maxMp);
    }

    public void UseMana(float mana)
    {
        CurMP -= mana;
        if (CurMP <= 0)
            CurMP = 0;
        SetMpUI();
    }
    private void Die()
    {
        playerAttack.HandleAnimatorTrigger("Die", true);
        playerAttack.HandleAnimatorTrigger("Alive", false);
        alive = false;
        playerMove.ResetPath();
        DieAction?.Invoke();
        playerCam.SetPriority(5);
    }

    public void Revival()
    {
        CurHP = MaxHP;
        CurMP = MaxMP;
        alive = true;
        playerAttack.HandleAnimatorTrigger("Alive", true);
        playerCam.SetPriority(20);
        SetHpUI();
        SetMpUI();
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.teamhpList[PhotonNetwork.LocalPlayer.ActorNumber].setHp(CurHP);
    }
    #endregion

    #region Initailize
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        DontDestroyOnLoad(gameObject);
    }
    
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == "Loading")
            return;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    IEnumerator Start()
    {
        MaxHP = 5000;
        CurHP = MaxHP;

        MaxMP = 1500;
        CurMP = MaxMP;


        maxExp = 100;
        curExp = 0;
        yield return new WaitUntil(() => PlayerSet == true);
        photonView.RPC("SetPlayerAttack", RpcTarget.AllBuffered, playerClass);
        if(photonView.IsMine)   
            playerCam.SetFollower(transform);
        SetHpUI();
        SetMpUI();
        PlayerUIManager.Instance.setMaxExp(maxExp);
    }


    [PunRPC]
    public void SetPlayerAttack(Class playerClass)
    {
        playerStat = new Status(); // STR = DEX = INT = DEF = 4;
        switch (playerClass)
        {
            case Class.Warrior:
                playerAttack = gameObject.AddComponent<WarriorClass>();
                break;
            case Class.Gunner:
                playerAttack = gameObject.AddComponent<GunnerClass>();
                break;
            case Class.Mage:
                playerAttack = gameObject.AddComponent<MageClass>();
                break;
        }

        canMove = photonView.IsMine;
    }

    public void PlayerStop()
    {
        canMove = false;
    }
    public void PlayerResume()
    {
        canMove = photonView.IsMine;
    }
    #endregion

    #region Item
    [Header("플레이어 아이템")]
    [SerializeField] private GameObject[] HandItems;
    public void EquipItem(Status itemStat)
    {
        playerStat = playerStat + itemStat;
    }

    public void UnEquipItem(Status itemStat)
    {
        playerStat = playerStat - itemStat;
    }
    #endregion

    #region IEffectable
    [PunRPC]
    public void TakeDamage(float damage, Vector3 attackerPos = default, int viewId = 0)
    {
        if (Parrying)
            return;

        if (alive == false)
            return;

        if (photonView.IsMine == false)
            return;

        CurHP -= damage;
        if (CurHP <= 0)
        {
            Die();
            CurHP = 0;
        }
        SetHpUI(); 
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.teamhpList[PhotonNetwork.LocalPlayer.ActorNumber].setHp(CurHP);
    }
    public void TakeDamageRPC(float damage, Vector3 attackerPos, int viewId)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage, attackerPos, viewId);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(1000);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Revival();
        }
    }
    Coroutine stunCoroutine;
    public void Stun(float stunTime)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunCoroutine(stunTime));
    }

    IEnumerator StunCoroutine(float stunTime)
    {
        PlayerStop();
        playerAttack.HandleAnimatorTrigger("Stun", true);
        yield return new WaitForSeconds(stunTime);
        stunCoroutine = null;
        PlayerResume();
        playerAttack.HandleAnimatorTrigger("StunEnd", true);
    }

    private Dictionary<EffectType, Coroutine> buffCoroutine = new Dictionary<EffectType, Coroutine>();

    public void ApplyEffect(StatusEffectData effectData, int viewId)
    {
        StartCoroutine(EffectCoroutine(effectData, viewId));
    }

    public IEnumerator EffectCoroutine(StatusEffectData data, int viewId)
    {
        yield return null;
    }

    #endregion

    #region Photon
    private float prevhp=-1;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CurHP);
            
        }
        else
        {
            DungeonManager dun = DungeonManager.Instance;
            float curhp = (float)stream.ReceiveNext();
            
            if(dun != null)
            {
                if(prevhp!=-1 && prevhp != curhp)
                    dun.teamhpList[info.Sender.ActorNumber].setHp(curhp);
            }
            prevhp = curhp;
        }
    }

    public void ShakeCamera(float shakeIntensity, float shakeTime)
    {
        if(photonView.IsMine)
            playerCam.ShakeCamera(shakeIntensity, shakeTime);
    }
    #endregion
}
