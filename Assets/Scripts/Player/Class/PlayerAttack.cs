using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviourPun
{
    #region Fields
    protected float lastRollTime;
    protected float rollInterval = 3f;
    public List<SkillInstance> skillList; // Space = 0, Q = 1, W = 2, E = 3, R = 4
    protected int enemyLayer;
    protected float timer = 0;
    public Damage damage;
    [Header("Charge")]
    protected Slider chargeBar;
    protected bool isCharging = false;
    protected float chargeStartTime;
    protected float chargeDuration;

    [Header("Skill")]
    protected SkillInfo Space;
    protected SkillInfo Q;
    protected SkillInfo W;
    protected SkillInfo E;
    protected SkillInfo R;
    #endregion

    #region Components
    protected PlayerStatus status;
    protected PlayerMove move;
    protected Camera cam;
    protected Animator animator;
    #endregion

    #region Animator params
    protected readonly int hashAttackCount = Animator.StringToHash("AttackCount");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashIsAttacking = Animator.StringToHash("isAttacking");
    private readonly int hashSkillInt = Animator.StringToHash("SkillInteger");
    protected readonly int hashUsingSkill = Animator.StringToHash("usingSkill");

    protected bool IsAttacking => animator.GetBool(hashIsAttacking);
    protected bool UsingSkill => animator.GetBool(hashUsingSkill);
    protected int SkillInteger => animator.GetInteger(hashSkillInt);
    #endregion


    #region 초기화 & 저장 불러오기
    private void Awake()
    {
        status = GetComponent<PlayerStatus>();
        animator = GetComponent<Animator>();
        move = GetComponent<PlayerMove>();
        chargeBar = PlayerUIManager.Instance.castingBar;
        chargeBar.gameObject.SetActive(false);
        cam = Camera.main;
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        Init();
    }


    protected virtual void Init()
    {
        skillList = new List<SkillInstance>();

        damage = new Damage();

        SetPlayerDamage();

    }
    protected virtual void OnEnable()
    {
        lastRollTime = Time.time + rollInterval;
    }

    #endregion

    #region Update, HandleInput
    private void Update()
    {
        // 1) if(플레이어가 마을이라면) return;

        if (status.CanMove == false)
            return;

        if (status.Alive == false)
            return;

        if (isCharging)
            UpdateChargingBar();

        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0) && !UsingSkill)
        {
            Attack();
            move.ResetPath();
        }

        HandleSkillInput();
    }

    protected virtual void HandleSkillInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (skillList[(int)KeyBind.Space].Activate())
            {
                LookAtMousePosition();
                move.ResetPath();
                isCharging = false;
            }
        }
    }


    #region 기본 움직임
    private void Attack()
    {
        LookAtMousePosition();

        //animator.SetTrigger(hashAttack);
        //animator.SetInteger(hashAttackCount, 0);
        //animator.SetInteger("Class", (int)status.playerClass);

        photonView.RPC("RpcSetTrigger", RpcTarget.All, "Attack", true);
        photonView.RPC("RpcSetInteger", RpcTarget.All, "AttackCount", 0);
        photonView.RPC("RpcSetInteger", RpcTarget.All, "Class", (int)status.playerClass);

    }

    protected void LookAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    private void Roll()
    {
        StartCoroutine(ParryingCoroutine());
    }

    IEnumerator ParryingCoroutine()
    {
        status.Parrying = true;
        yield return new WaitForSeconds(1f);
        status.Parrying = false;
    }
    #endregion
    #endregion

    #region Effect & Damage
    public void SetSkillEffectRPC(int index, EffectType effectType)
    {
        if (skillList[index] == null)
            return;
        var playerId = photonView.ViewID;
        photonView.RPC("SetSkillEffect", RpcTarget.AllBuffered, playerId, index, effectType);
    }
    protected virtual void SetPlayerDamage()
    {
        damage.minDamage = 1;
        damage.maxDamage = 5;
        damage.accuracy = 0.3f;
    }
    #endregion

    #region RPC
    [PunRPC]
    public void SetSkillEffect(int playerId, int index, EffectType effectType)
    {
        // 모든 이펙트는 GameManager가 알고있어서 로컬에서 Get 후 스킬 바꿔주기
        var effectData = GameManager.Instance.GetEffectData(effectType);
        var targetPlayer = PhotonView.Find(playerId);

        if (targetPlayer != null)
        {
            targetPlayer.GetComponent<PlayerAttack>().skillList[index].SetEffectData(effectData);
        }
    }

    [PunRPC]
    protected virtual void CreateParticle(Vector3 effectPos, Vector3 dir, int skillIndex, int effectIndex, PhotonMessageInfo info)
    {
        
        var skill = skillList[skillIndex];

        // var effectController = Instantiate(skill.skillInfo.effectControllers[effectIndex]);
        OBPool.Instance.Get<EffectController>(skill.skillInfo.effectControllers[effectIndex].gameObject,out var effectController);
        effectController.transform.position = effectPos;
        effectController.transform.forward = dir;
        effectController.SetEffectData(skill.effectData);

        Damage skillDamage = new Damage();
        skillDamage.minDamage = damage.minDamage + skill.skillDamage;
        skillDamage.maxDamage = damage.maxDamage + skill.skillDamage;
        skillDamage.accuracy = damage.accuracy;

        if (info.photonView.IsMine)
        {
            effectController.SetDamage(skillDamage, true, photonView.ViewID);
        }
        else
        {
            effectController.SetDamage(new Damage(), false, photonView.ViewID);
        }

        var lifeTime = skill.skillInfo.lifeTime;
        lifeTime = lifeTime == 0 ? 2 : lifeTime;
         OBPool.Instance.Despawn(effectController.gameObject, lifeTime);
        //Destroy(effectController.gameObject, lifeTime);
    }



    public void HandleAnimatorTrigger(string paramName, bool on)
    {
        photonView.RPC("RpcSetTrigger", RpcTarget.All, paramName, on);
    }
    public void HandleAnimatorInteger(string paramName, int value)
    {
        photonView.RPC("RpcSetInteger", RpcTarget.All, paramName, value);
    }
    public void HandleAnimatorFloat(string paramName, float value)
    {
        photonView.RPC("RpcSetFloat", RpcTarget.All, paramName, value);
    }
    public void HandleAnimatorBool(string paramName, bool on)
    {
        photonView.RPC("RpcSetBool", RpcTarget.All, paramName, on);
    }

    [PunRPC]
    public void RpcSetInteger(string paramName, int value)
    {
        animator.SetInteger(paramName, value);
    }

    [PunRPC]
    public void RpcSetFloat(string paramName, float value)
    {
        animator.SetFloat(paramName, value);
    }

    [PunRPC]
    public void RpcSetBool(string param, bool value)
    {
        animator.SetBool(param, value);
    }

    [PunRPC]
    public void RpcSetTrigger(string paramName, bool on)
    {
        if (on)
        {
            animator.SetTrigger(paramName);
        }
        else
        {
            animator.ResetTrigger(paramName);
        }
    }
    #endregion

    #region Charging
    protected void StartCharging()
    {
        photonView.RPC("RpcSetTrigger", RpcTarget.All, "Cast", false);
        isCharging = true;
        chargeStartTime = Time.time;
        chargeBar.gameObject.SetActive(true);
        chargeBar.value = 0;

    }
    private void UpdateChargingBar()
    {
        float timeCharged = Time.time - chargeStartTime;
        chargeBar.value = timeCharged / chargeDuration;

        if (timeCharged >= chargeDuration)
        {
            // 차징 완료 처리
            CompleteCharge();
        }
    }
    protected void CompleteCharge()
    {
        isCharging = false;
        chargeBar.gameObject.SetActive(false);
        photonView.RPC("RpcSetTrigger", RpcTarget.All, "Cast", true);
    }
    protected void StopCharging()
    {
        float heldTime = Time.time - chargeStartTime;
        isCharging = false;
        chargeBar.gameObject.SetActive(false);
        if (heldTime >= chargeDuration)
        {
            photonView.RPC("RpcSetTrigger", RpcTarget.All, "Cast", true);
        }
        else
        {
            photonView.RPC("RpcSetTrigger", RpcTarget.All, "Cast", false);
            photonView.RPC("RpcSetTrigger", RpcTarget.All, "CastCancel", true);
        }
    }

    #endregion
}
