using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class DeathKnight : Enemy, IEffectable
{
    public string BossName { get; private set; }
    public enum StateBools
    {
        IsMoving, IsAttacking, IsDefensing, IsTeleporting, IsInvincible, IsDead, All
    }

    private Animator animator;
    private NavMeshAgent agent;
    private MotionTrailer motionTrailer;
    private BossSkinChanger skinChanger;

    Collider[] detectedCols; // Player 인원 수만큼 생성할 Array??

    private Dictionary<StateBools, bool> stateParams;

    private float curHp;
    [SerializeField] private float maxHp;

    int skillAttackHash;
    int normalAttackHash;

    [Header("패턴 시전시 순간이동 위치")]
    [SerializeField] Transform resetTrans;

    [Header("반격 시 스턴 지속 시간")]
    [SerializeField] float stunTime;

    [Header("애니메이션 속도")]
    [SerializeField] float baseSpeed = 1f;

    [Header("광폭화 추가 계수")]
    bool isRageState = false;
    [SerializeField] float plusSpeed;
    [SerializeField] float plusDamage;

    [Header("Check Duration")]
    [SerializeField] float skillAttackInterval;
    [SerializeField] float normalAttackInterval;
    [SerializeField] float weaponChangeInterval;
    [SerializeField] float momentaryWait;

    [Header("플레이어 탐지 내적 각도")]
    [SerializeField] float detectDegree;

    [Header("Projectors")]
    [SerializeField] CircleProjector circleProjector;
    [SerializeField] SectorFormProjector sectorProjector;
    [SerializeField] SquareProjector squareProjector;

    [Header("Boss Own Particles")]
    [SerializeField] BossOwnParticle deathSpiral;
    [SerializeField] GameObject infernoShield;

    public bool FirstPatternTriggered { get; set; }
    public bool SecondPatternTriggered { get; set; }

    #region Weapons
    private Dictionary<WeaponType, BossWeapon> weapons;
    public int WeaponsCount => weapons.Count;

    [Header("Weapons")]
    [SerializeField] GreatSword greatSword;
    [SerializeField] Katana katana;

    BossWeapon curWeapon;

    #endregion

    #region weaponTransform

    [Header("WeaponTransform")]
    // 순서를 맞출 것
    [SerializeField] List<Transform> unequipWeaponTransform;
    [SerializeField] List<Transform> weaponHolder;

    #endregion

    #region Properties
    public BossWeapon CurWeapon => curWeapon;
    public List<Transform> UnequipWeaponTransform => unequipWeaponTransform;
    public List<Transform> WeaponHolder => weaponHolder;
    public CircleProjector CircleProjector => circleProjector;
    public SectorFormProjector SectorProejctor => sectorProjector;
    public SquareProjector SquareProjector => squareProjector;
    public BossOwnParticle DeathSpiral => deathSpiral;
    public GameObject InfernoShield => infernoShield;
    public BossSkinChanger SkinChanger => skinChanger;
    public Transform ResetTrans => resetTrans;

    public float CurHP { get => curHp; set => curHp = value; }
    public float MaxHP { get => maxHp; set => maxHp = value; }

    public bool IsRageState => isRageState;
    public float BaseSpeed => IsRageState ? baseSpeed + plusSpeed : baseSpeed;
    public float PlusSpeed => plusSpeed;
    public float PlusDamage => plusDamage;

    #endregion

    [HideInInspector] public UnityEvent CounterAttackEvent; // TakeDamage일 때 IsAttacking/IsDefensing 시 CounterAttack Invoke 하기
    [HideInInspector] public UnityEvent<WeaponType> WeaponChangedEvent;

    List<int> startPatternIndexes;

    Dictionary<EffectType, Coroutine> effectCoroutine;

    private void Awake()
    {
        BossName= "DeathKnight";
        animator = GetComponent<Animator>();
        motionTrailer = GetComponent<MotionTrailer>();
        skinChanger = GetComponent<BossSkinChanger>();

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;

        stateParams = new Dictionary<StateBools, bool>() { {StateBools.IsMoving, false }, {StateBools.IsAttacking, false }, { StateBools.IsDefensing, false}
             , { StateBools.IsTeleporting, false}, { StateBools.IsInvincible, false}, { StateBools.IsDead, false} };
        weapons = new Dictionary<WeaponType, BossWeapon>() { { WeaponType.GreatSword, greatSword }, { WeaponType.Katana, katana } };

        curWeapon = weapons[(WeaponType)animator.GetInteger("CurWeaponState")];

        skillAttackHash = Animator.StringToHash("SkillAttack");
        normalAttackHash = Animator.StringToHash("NormalAttack");

        curHp = maxHp;

        List<List<int>> skillPatternIndexes = new List<List<int>>{
            new List<int>() {0, 1, 2, 3 },
            new List<int>() {0, 3 ,2, 1 },
            new List<int>() {0, 2, 3, 1 },
            new List<int>() {1, 0 ,2, 3 },
            new List<int>() {1, 3, 2, 0 },
            new List<int>() {1, 2, 0, 3 },
            new List<int>() {2, 0, 1, 3 },
            new List<int>() {2, 3, 0, 1 },
            new List<int>() {2, 0, 3, 1 },
            new List<int>() {3, 1, 2, 0 },
            new List<int>() {3, 0, 2, 1 },
            new List<int>() {3, 2, 0, 1 }
            };

        startPatternIndexes = skillPatternIndexes[UnityEngine.Random.Range(0, skillPatternIndexes.Count - 1)];

        StringBuilder str = new StringBuilder();
        foreach(var i in startPatternIndexes)
        {
            str.Append($"{i} ");
        }
        print(str.ToString());

        effectCoroutine = new()
        {
            { EffectType.None, null},
            { EffectType.Fire, null },
            { EffectType.Lightning, null },
            { EffectType.Ice, null },
            { EffectType.Poison, null }
        };
    }

    private void Start()
    {
        agent.updatePosition = false;
        agent.updateRotation = true;

        if (PhotonNetwork.IsMasterClient == true) // MasterClient 일 때만 스킬 주기 확인
        {
            StartCoroutine(CheckTimerCoroutine());
        }
    }


    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    IEnumerator CheckTimerCoroutine()
    {
        float curNormalWaitTime = 0;
        float curSkillWaitTime = 0;
        float curWeaponChangeTime = 0;

        int curListIndex = 0;

        while (true)
        {
            if (stateParams[StateBools.IsDead] == true) break;

            curNormalWaitTime += Time.deltaTime;
            curSkillWaitTime += Time.deltaTime;
            curWeaponChangeTime += Time.deltaTime;

            if (animator.GetBool(skillAttackHash) == true || animator.GetBool(normalAttackHash) == true)
            {
                print("공격 중이라 일시정지");
                yield return new WaitUntil(() => animator.GetBool(skillAttackHash) == false && animator.GetBool(normalAttackHash) == false);
                print("공격 종료. 공격 주기 계산으로 전환");
            }

            if (curWeaponChangeTime > weaponChangeInterval) // 무기 교체 주기
            {
                yield return new WaitForSeconds(momentaryWait);

                ChangeAnimRPC("UnEquip");
                curWeaponChangeTime = 0;
                continue;
            }

            if (curSkillWaitTime > skillAttackInterval) // 스킬 공격 주기
            {
                yield return new WaitForSeconds(momentaryWait);

                if (curTarget == null && FindTargets() == true)
                {
                    SetTarget();
                }

                if(curListIndex >= startPatternIndexes.Count)
                {
                    curListIndex = 0;
                }

                int curComboIndex = startPatternIndexes[curListIndex];

                ChangeAnimRPC("AttackCombo", curComboIndex);
                ChangeAnimRPC("SkillAttack", true);
                ChangeAnimRPC("Attack");

                curSkillWaitTime = 0;
                curListIndex++;
                continue;
            }

            if (curNormalWaitTime > normalAttackInterval) // 일반 공격 주기
            {
                yield return new WaitForSeconds(momentaryWait);

                CheckAttackTarget();
                curNormalWaitTime = 0;
            }

            yield return null;
        }
    }


    #region ChangeWeapon
    [PunRPC]
    public void ChangeWeapon(int weaponType)
    {
        if (weaponType < 0 || weaponType > weapons.Count - 1)
        {
            curWeapon = null;
            return;
        }

        curWeapon = weapons[(WeaponType)weaponType];
        WeaponChangedEvent?.Invoke((WeaponType)weaponType);
    }
    [PunRPC]
    public void ChangeWeaponRPC(int weaponType)
    {
        photonView.RPC("ChangeWeapon", RpcTarget.All, weaponType);
    }

    [PunRPC]
    public void SetWeaponTransform(bool isEquip = true)
    {
        WeaponType type = CurWeapon.Type;

        if (isEquip == true)
        {
            CurWeapon.transform.parent = WeaponHolder[(int)type];
            CurWeapon.transform.localPosition = Vector3.zero;
            CurWeapon.transform.localRotation = Quaternion.identity;
        }
        else
        {
            CurWeapon.transform.parent = UnequipWeaponTransform[(int)type];
            CurWeapon.transform.localPosition = Vector3.zero;
            CurWeapon.transform.localRotation = Quaternion.identity;
        }
    }
    [PunRPC]
    public void SetWeaponTransformRPC(bool isEquip)
    {
        photonView.RPC("SetWeaponTransform", RpcTarget.All, isEquip);
    }

    #endregion

    #region ChangeAnimation

    [PunRPC]
    public void ChangeAnim(string conditionName)
    {
        animator.SetTrigger(conditionName);
    }
    [PunRPC]
    public void ChangeAnim(string conditionName, int param)
    {
        animator.SetInteger(conditionName, param);
    }
    [PunRPC]
    public void ChangeAnim(string conditionName, float param)
    {
        animator.SetFloat(conditionName, param);
    }
    [PunRPC]
    public void ChangeAnim(string conditionName, bool param)
    {
        animator.SetBool(conditionName, param);
    }

    [PunRPC]
    public void ChangeAnimRPC(string conditionName)
    {
        photonView.RPC("ChangeAnim", RpcTarget.All, conditionName);
    }

    [PunRPC]
    public void ChangeAnimRPC(string conditionName, int param)
    {
        photonView.RPC("ChangeAnim", RpcTarget.All, conditionName, param);
    }

    [PunRPC]
    public void ChangeAnimRPC(string conditionName, float param)
    {
        photonView.RPC("ChangeAnim", RpcTarget.All, conditionName, param);
    }
    [PunRPC]
    public void ChangeAnimRPC(string conditionName, bool param)
    {
        photonView.RPC("ChangeAnim", RpcTarget.All, conditionName, param);
    }

    #endregion

    #region RefAnimatorSpeed

    [PunRPC]
    public void ChangeAnimSpeed(float speed)
    {
        animator.speed = speed;
    }

    [PunRPC]
    public void ChangeAnimSpeedRPC(float speed)
    {
        photonView.RPC("ChangeAnimSpeed", RpcTarget.All, speed);
    }

    #endregion

    #region GetSetBools
    public bool GetStateBools(StateBools param)
    {
        if (param == StateBools.All)
        {
            foreach (bool boolean in stateParams.Values)
            {
                if (boolean == true)
                {
                    return true;
                }
            }

            return false;
        }

        return stateParams[param];
    }

    public void SetStateBools(StateBools state, bool boolean)
    {
        stateParams[state] = boolean;
    }

    #endregion

    #region GetSetTarget
    public bool FindTargets()
    {
        detectedCols = Physics.OverlapSphere(transform.position, DetectRange, TargetMask);
        return (detectedCols.Length > 0) ? true : false;
    }
    public void SetTarget(bool isClosest = true) // TODO : GameManager / DungeonManager에서 플레이어를 담아두는 배열이 완성되면 배열을 순회해서 타겟을 선정할 것
    {
        Transform tempTarget = null;

        if (isClosest == true)
        {
            float minDist = 9999;

            foreach (var col in detectedCols)
            {
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);

                // if (dist < agent.stoppingDistance) continue; // TODO : 제동거리보다 짧더라도 Target으로 설정해줄지 여부 고려

                if (minDist > dist)
                {
                    minDist = dist;
                    tempTarget = col.transform;
                }
            }
        }
        else
        {
            float maxDist = 0;

            foreach (var col in detectedCols)
            {
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);


                if (maxDist < dist)
                {
                    maxDist = dist;
                    tempTarget = col.transform;
                }
            }
        }

        curTarget = tempTarget;
    }

    public void CheckAttackTarget() // 추후 플레이어를 리스트에 담을 시 overlapSphere 하지 말고 리스트의 요소를 순회하는 로직으로 변경
    {
        Collider[] playerCols = Physics.OverlapSphere(transform.position, attackRange + 1f, TargetMask);

        if (playerCols.Length > 0)
        {
            float minDist = 999;
            Transform target = null;

            Vector3 forwardDir = transform.forward.normalized;
            float cosResult = Mathf.Cos(detectDegree * Mathf.Deg2Rad);

            foreach (var p in playerCols)
            {
                float dist = Vector3.Distance(transform.position, p.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = p.transform;
                }
            }
            if (target != null)
            {
                Vector3 enemyDir = (target.transform.position - transform.position).normalized;
                float dotResult = Vector3.Dot(forwardDir, enemyDir);

                if (dotResult > cosResult) // 내적은 방향이 완전히 같을 때 1, 수직일 때 0, 완전 반대 방향일 때 -1
                {
                    int comboIndex = UnityEngine.Random.Range(0, 2);

                    ChangeAnimRPC("AttackCombo", comboIndex);
                    ChangeAnimRPC("NormalAttack", true);
                    ChangeAnimRPC("Attack");
                }
            }
        }


    }
    #endregion

    #region TakeDamage
    [PunRPC]
    public void TakeDamage(float damage, Vector3 attackerPos, int viewId)
    {
        if (stateParams[StateBools.IsDead] == true || stateParams[StateBools.IsInvincible] == true) return;

        if (stateParams[StateBools.IsDefensing] == true)
        {
            ChangeAnimRPC("CounterAttack");

            print($"CounterAttack 호출, {attackerPos}에게 텔레포트");

            curTarget = PhotonView.Find(viewId).transform;
            curTarget.GetComponent<PlayerStatus>().Stun(stunTime);

            TryTeleport(curTarget, -3);

            CounterAttackEvent?.Invoke();

            return;
        }
        curHp -= damage;

        GameManager.Instance.UI_ShowDamage(damage, transform.position);

        float hpPercent = curHp / maxHp * 100;

        if (PhotonNetwork.IsMasterClient == true) //
        {
            if (FirstPatternTriggered == false && hpPercent < 70)
            {
                SetPattern();

                FirstPatternTriggered = true;
            }

            if (SecondPatternTriggered == false && hpPercent < 40)
            {
                SetPattern();

                SecondPatternTriggered = true;
            }

            if (curHp <= 0)
            {
                Vector3 dir = (attackerPos - transform.position).normalized;
                float dirDot = Vector3.Dot(dir, transform.forward);
                bool attackedFront = (dirDot > 0);

                float tempExp = 50f;

                foreach (var player in DungeonManager.Instance.playerList)
                {
                    GiveExp(player, tempExp);
                }

                photonView.RPC("SetDeath", RpcTarget.All, attackedFront);
            }
        }
        if (DungeonManager.Instance.bossHpbar != null)
        {
            Debug.Log($"sethp{CurHP}");
            photonView.RPC("HpbarSet", RpcTarget.All, CurHP);
        }
    }

    [PunRPC]
    public void SetDeath(bool attackedFront)
    {
        stateParams[StateBools.IsDead] = true;
        agent.updateRotation = false;
        agent.updatePosition = false;

        ChangeAnim("AttackedFront", attackedFront);
        ChangeAnim("Death");

        // DungeonManager

        StartCoroutine(WaitPortalCoroutine());
    }

    IEnumerator WaitPortalCoroutine()
    {
        float waitTime = 2f;
        yield return new WaitForSeconds(waitTime);

        DungeonManager.Instance.DungeonClear();
    }

    [PunRPC]
    public void HpbarSet(float curhp)
    {
        DungeonManager.Instance.bossHpbar.setHp(curhp);
    }
    void SetPattern()
    {
        int index = animator.GetInteger("PatternIndex");
        index++;

        ChangeAnimRPC("PatternIndex", index);
        ChangeAnimRPC("Pattern");
    }

    public void TakeDamageRPC(float damage, Vector3 attackerPos, int viewId)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage, attackerPos, viewId);
    }

    

    public void ApplyEffect(StatusEffectData effectData, int viewId)
    {
        var curCoroutine = effectCoroutine[effectData.EffectType];
        if (curCoroutine == null)
        {
            curCoroutine = StartCoroutine(EffectCoroutine(effectData, viewId));
        }
        else
        {
            StopCoroutine(curCoroutine);
            curCoroutine = StartCoroutine(EffectCoroutine(effectData, viewId));
        }
    }

    public IEnumerator EffectCoroutine(StatusEffectData data, int viewId)
    {
        TakeDamageRPC(data.DOTAmount, transform.position, viewId);

        yield return null;
    }
    #endregion

    #region RefTeleport
    public bool TryTeleport(Transform target, float plusDist = 0, bool isBackSide = true) // 텔레포트 위치 설정하고 이동
    {
        if(target == null)
        {
            print("target 없음");
            return false;
        }

        if (photonView.IsMine == false) return false;

        GetComponent<PhotonTransformView>().enabled = false;

        // animator.speed = 0;
        Vector3 pos = target.position + Vector3.up;
        Vector3 dir = isBackSide? -target.transform.forward : target.transform.forward;
        LayerMask targetMask = LayerMask.GetMask("Ground");

        bool wallExist = Physics.Raycast(pos, dir, attackRange + plusDist, targetMask);

        if (wallExist == true)
        {
            print("벽 감지됨");
            return false;
        }
        else
        {
            float dist = (plusDist == 0) ? attackRange : attackRange + plusDist;

            Vector3 temp = target.position;
            temp.y = transform.position.y;
            Vector3 destination = temp + dir * dist;

            agent.Warp(destination);

            GetComponent<PhotonTransformView>().enabled = true;


            animator.rootPosition = destination;
            transform.position = destination;

            Vector3 tempTarget = target.position;
            tempTarget.y = destination.y;

            Vector3 curDir =  (tempTarget - destination).normalized;

            transform.forward = curDir;
            
            return true;
        }
    }

    [PunRPC]
    public void ReadyToTeleportRPC(float waitTime) // TODO : 학원 가서 캐릭터 동기화 되는 방향으로 재작성할 것
    {
        ChangeAnimSpeed(0);

        StartCoroutine(TeleportAttackCoroutine(waitTime));
    }

    IEnumerator TeleportAttackCoroutine(float waitTime)
    {
        if (CurTarget == null)
        {
            print("현재 타겟이 없습니다");
            yield break;
        }

        // motionTrailer.FinishMotionTrail();
        

        yield return new WaitForSeconds(waitTime);


        TryTeleport(CurTarget);

        ChangeAnimSpeed(1f);

        motionTrailer.StartMotionTrail(); // TODO : 텔레포트 자체를 RPC로 하지 않고 있어서 MotionTrail도 RPC로 실행 X
    }
    #endregion


    [PunRPC]
    public void SetObjectActiveRPC(int viewId, bool isOn)
    {
        photonView.RPC("SetObjectActive", RpcTarget.All, viewId, isOn);
    }
    [PunRPC]
    public void SetObjectActive(int viewId, bool isOn)
    {
        foreach (PhotonView pv in PhotonNetwork.PhotonViewCollection)
        {
            if(pv.ViewID == viewId)
            {
                pv.gameObject.SetActive(isOn);
            }
        }
    }

    [PunRPC]
    public void SkinChange()
    {
        skinChanger.ChangeNextSkin();
    }
    public void SkinChangeRPC()
    {
        photonView.RPC("SkinChange", RpcTarget.All);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // 휴면 상태 해제 / 메인 패턴 진입 메서드
    public void StartPattern()
    {
        ChangeAnimRPC("Start");
    }

    

}
