using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TestMob : Enemy, IEffectable
{
    public enum MobState
    {
        Idle, Chase, /*Patrol,*/ GoBack, Dead,
    }

    Animator animator;
    NavMeshAgent agent;
    SkinnedMeshRenderer render;

    [Header("피격시 반짝임 컨트롤 Property")]
    [SerializeField] int maxGlitterCount;
    [SerializeField] float glitterDuration;
    [SerializeField] Color goalColor;

    [Header("RagDoll 적용할 부위의 Rigidbody")]
    [SerializeField] Rigidbody targetForceRb;

    [Header("데미지 판정 Collider")]
    [SerializeField] Collider curCol;

    [Header("죽음 이후 파괴 대기 시간")]
    [SerializeField] float destroyWaitTime;

    [Header("전방 공격 내적 각도")]
    [SerializeField] float attackDegree;

    [Space(20)]

    #region Variables
    public float curHp;
    public float maxHp;
    public float CurHP { get => curHp; set => curHp = value; }
    public float MaxHP { get => maxHp; set => maxHp = value; }

    public bool IsAttacking { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool IsInvincible { get; set; }

    MobState curState;
    #endregion

    #region InspectorVariables
    [Header("플레이어 탐지 배열")]
    Collider[] targetCols;
    [SerializeField] int detectPlayerCount;


    [Header("탐색 주기")]
    float curWaitFindTime;
    [SerializeField] float maxWaitFindTime;

    [Header("순찰 주기")]
    float curWaitPatrolTime;
    [SerializeField] float maxWaitPatrolTime;

    [Header("순찰 최대 거리")]
    [SerializeField] float maxPatrolDist;
    Vector3 patrolTarget;

    [Header("추적 최대 거리")]
    [SerializeField] float maxChaseDist;

    [Header("공격 주기")]
    float curWaitAttackTime;
    [SerializeField] float attackInterval; // 공격 주기
    [SerializeField] float attackToMoveDelay; // 공격 후 경직시간

    [Header("탐지 내적 각도")]
    [SerializeField] float detectDegree;

    [Header("데미지")]
    [SerializeField] float damage;

    [Header("Death 관련 이벤트 설정하기")]
    [HideInInspector] public UnityEvent MobDeathEvent; // 필요시 해당 이벤트에 구독하거나 별도의 이벤트를 다룰 매니저를 이용할 것

    #endregion

    Vector3 lastPos;

    Dictionary<EffectType, Coroutine> effectCoroutine;

    float animatorBaseSpeed;
    float agentBaseSpeed;

    private void Awake()
    {
        targetCols = new Collider[detectPlayerCount];

        render = GetComponentInChildren<SkinnedMeshRenderer>();
        Material mat = new Material(render.sharedMaterial);
        render.material = mat;

        curState = MobState.Idle;
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange - 1f;


        SetRagDollState(false);

        effectCoroutine = new()
        {
            { EffectType.None, null},
            { EffectType.Fire, null },
            { EffectType.Lightning, null },
            { EffectType.Ice, null },
            { EffectType.Poison, null }
        };

        animatorBaseSpeed = 1f;
        agentBaseSpeed = agent.speed;
    }

    #region StateUpdate
    private IEnumerator Start()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient == false) yield break;

            if (curState == MobState.Dead) yield break;

            if (IsUnderAttack == true)
            {
                SetAgentStoppedRPC(true);

                yield return new WaitUntil(() => IsUnderAttack == false);

                SetAgentStoppedRPC(false);
            }


            //ChangeAnimRPC("Speed", RpcTarget.All, agent.velocity.magnitude);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            if (IsAttacking == true && agent.velocity.magnitude > 0.5f) // 공격 후 경직시간
            {
                SetAgentStoppedRPC(true);

                yield return new WaitForSeconds(attackToMoveDelay);

                SetAgentStoppedRPC(false);
            }

            switch (curState) // curState를 변경할 시 ResetVariables를 먼저 해줄 것
            {
                case MobState.Idle:
                    if (curWaitFindTime > maxWaitFindTime)
                    {
                        int num = Physics.OverlapSphereNonAlloc(transform.position, detectRange, targetCols, TargetMask);
                        if (num > 0)
                        {
                            SelectTarget();
                            lastPos = transform.position;

                            ResetVariables();
                            curState = MobState.Chase;
                            break;
                        }
                        curWaitFindTime = 0;
                    }
                    /*
                    if (curWaitPatrolTime > maxWaitPatrolTime)
                    {
                        SetPatrolPos();

                        ResetVariables();
                        curState = MobState.Patrol;
                        
                        break;
                    }
                    */
                    curWaitFindTime += Time.deltaTime;
                    curWaitPatrolTime += Time.deltaTime;
                    break;

                case MobState.Chase:

                    if (curTarget == null || curTarget.gameObject.activeSelf == false)
                    {
                        curTarget = null;
                        curState = MobState.Idle;
                        break;
                    }

                    float chaseDist = Vector3.Distance(lastPos, transform.position);
                    if (chaseDist > maxChaseDist)
                    {
                        curState = MobState.GoBack;
                        IsInvincible = true;

                        ChangeAnimSpeedRPC(2f);
                        ChangeAgentSpeedRPC(agent.speed * 2);
                        break;
                    }

                    ChaseTarget();
                    CheckAttackTarget();
                    break;
                /*
            case MobState.Patrol:
                float patrolDist = Vector3.Distance(transform.position, patrolTarget);

                if (curWaitFindTime > maxWaitFindTime)
                {
                    Collider[] cols = Physics.OverlapSphere(transform.position, detectRange, TargetMask);
                    if (cols.Length > 0)
                    {
                        SelectTarget();
                        lastPos = transform.position;

                        curState = MobState.Chase;
                        ResetVariables();
                        break;
                    }
                    curWaitFindTime = 0;
                }

                if (patrolDist < 0.3f)
                {
                    curState = MobState.Idle;
                    ResetVariables();
                    break;
                }
                break;
                */
                case MobState.GoBack:
                    agent.SetDestination(lastPos);

                    float dist = Vector3.Distance(transform.position, lastPos);

                    if (dist < agent.stoppingDistance + 0.5f)
                    {
                        IsInvincible = false;

                        curTarget = null;
                        curState = MobState.Idle;

                        ChangeAnimSpeedRPC(1f);
                        ChangeAgentSpeedRPC(agent.speed / 2);

                        ResetVariables();
                    }
                    break;

            }

            yield return null;
        }

    }
    #endregion

    private void OnEnable()
    {
        curHp = maxHp;
        curState = MobState.Idle;
    }

    private void OnDisable()
    {
        animator.enabled = true;
        agent.enabled = true;
        GetComponent<PhotonAnimatorView>().enabled = true;
        SetRagDollState(false);

        ResetVariables();
    }

    void ResetVariables()
    {
        curWaitFindTime = 0;
        curWaitPatrolTime = 0;
        curWaitAttackTime = 0;
        if (curState == MobState.Idle)
        {
            targetCols = new Collider[detectPlayerCount];
        }

    }

    void SetPatrolPos()
    {
        Vector3 randomPos = Random.insideUnitSphere * maxPatrolDist;
        randomPos.y = 0;

        Vector3 targetPos = transform.position + randomPos;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, maxPatrolDist, NavMesh.AllAreas) == true)
        {
            patrolTarget = targetPos;
            agent.SetDestination(patrolTarget);
        }
    }

    void SelectTarget()
    {
        if (targetCols.Length <= 0)
        {
            print("타겟이 없습니다");
            return;
        }

        float minDist = 9999; // 
        Transform tempTarget = null;

        foreach (Collider col in targetCols)
        {
            if (col == null) continue;

            float dist = Vector3.Distance(col.transform.position, transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                tempTarget = col.transform;
            }
        }
        curTarget = tempTarget;
    }



    void ChaseTarget()
    {
        if (curTarget == null || curTarget.gameObject.activeSelf == false /*|| curTarget.root == GameManager.Instance.transform/* 풀에 반납됐는지 여부 확인 */)
        {
            curState = MobState.Idle;
            curTarget = null;
        }
        else
        {
            agent.SetDestination(curTarget.position);
        }
    }

    void CheckAttackTarget()
    {
        float dist = Vector3.Distance(curTarget.position, transform.position);
        curWaitAttackTime += Time.deltaTime;

        if (dist < attackRange && IsAttacking == false && curWaitAttackTime > attackInterval)
        {
            Vector3 enemyDir = (curTarget.transform.position - transform.position).normalized;
            Vector3 forwardDir = transform.forward.normalized;

            float dotResult = Vector3.Dot(forwardDir, enemyDir);
            float cosResult = Mathf.Cos(detectDegree * Mathf.Deg2Rad);

            // print($"{dotResult} , {cosResult}");

            if (dotResult > cosResult) // 내적은 방향이 완전히 같을 때 1, 수직일 때 0, 완전 반대 방향일 때 -1
            {
                IsAttacking = true;
                ChangeAnimRPC("Attack");

                curWaitAttackTime = 0;
            }
        }
    }

    #region RefAgent
    [PunRPC]
    public void SetAgentStopped(bool isStopped)
    {
        agent.updatePosition = !isStopped;
        // agent.isStopped = isStopped;
    }

    public void SetAgentStoppedRPC(bool isStopped)
    {
        photonView.RPC("SetAgentStopped", RpcTarget.All, isStopped);
    }

    [PunRPC]
    public void ChangeAgentSpeed(float maxSpeed)
    {
        agent.speed = maxSpeed;
    }

    public void ChangeAgentSpeedRPC(float maxSpeed)
    {
        photonView.RPC("ChangeAgentSpeed", RpcTarget.All, maxSpeed);
    }

    #endregion

    #region RefAnimation

    [PunRPC]
    public void ChangeAnim(string name)
    {
        animator.SetTrigger(name);
    }
    [PunRPC]
    public void ChangeAnimRPC(string name)
    {
        photonView.RPC("ChangeAnim", RpcTarget.All, name);
    }

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

    #region TakeDamage


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

    public IEnumerator EffectCoroutine(StatusEffectData data, int viewID)
    {
        float elapsedTime = 0f;
        while (elapsedTime < data.Lifetime)
        {
            TakeDamageRPC(data.DOTAmount, transform.position, viewID);
            // 이동속도 data.MovementPenalty

            animator.speed = animatorBaseSpeed * data.MovementPenalty;
            agent.speed = agentBaseSpeed * data.MovementPenalty;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.speed = animatorBaseSpeed;
        agent.speed = agentBaseSpeed;

    }

    [PunRPC]
    public void TakeDamage(float damage, Vector3 attackerPos, int viewId)
    {
        if (curState == MobState.Dead) return;

        if (IsInvincible == true)
        {
            //GameManager.Instance.UI_ShowDamage(0, transform.position + new Vector3(0, 2f, 0));
            return;
        }

        IsUnderAttack = true;

        curHp -= damage;

        if(damage > 0)
        {
            GameManager.Instance.UI_ShowDamage(damage, transform.position);
        }
        

        if (curHp <= 0)
        {
            float tempExp = 2f;

            foreach(var player in DungeonManager.Instance.playerList)
            {
                GiveExp(player, tempExp);
            }

            StartCoroutine(ForceRagDollCoroutine(damage, attackerPos));

            return;
        }

        SetBlinkEffect();
        animator.SetTrigger("Hit");
    }

    public void TakeDamageRPC(float damage, Vector3 attackerPos, int viewId)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage, attackerPos, viewId);
    }

    IEnumerator ForceRagDollCoroutine(float damage, Vector3 attackerPos)
    {
        curState = MobState.Dead;

        DungeonManager.Instance.RemoveMonster(gameObject);

        Vector3 curPos = transform.position;
        float setWaitTime = 0.15f;

        animator.enabled = false;
        agent.enabled = false;
        GetComponent<PhotonAnimatorView>().enabled = false;
        SetRagDollState(true);

        yield return new WaitForSeconds(setWaitTime);


        // 밀려 날아가는 방향에 대한 설정을 지면으로부터 45도로 설정할지 , 맞은 위치로부터 반대로 설정할지

        Vector3 newDir = (transform.position - attackerPos).normalized;
        newDir *= 600;

        /*       =>      아래 주석처리된 로직은 맞은 위치의 반대 방향이긴 하되, 지면으로부터 45도각도 고정
        Vector3 temp = attackerPos;
        temp.y = transform.position.y;
        Vector3 dir = (transform.position - temp).normalized;

        Vector3 horizontalDirection = new Vector3(dir.x, 0, dir.z).normalized;

        // 지면으로부터 45도 각도를 유지하려면, y 성분은 수평 벡터 길이와 같아야 함
        float newY = horizontalDirection.magnitude; // 수평 방향 벡터의 길이 계산

        // 새로운 방향 벡터 생성
        Vector3 newDir = new Vector3(horizontalDirection.x, newY, horizontalDirection.z).normalized;

        float forceWeight = 0;
        switch(damage)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }

        //newDir *= forceWeight;
        newDir *= 600;
        */

        targetForceRb.AddForce(newDir, ForceMode.Impulse);

        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(destroyWaitTime);

            PhotonNetwork.Destroy(gameObject);
        }

    }




    void SetBlinkEffect()
    {
        StartCoroutine(BlinkEffectCoroutine());
    }

    IEnumerator BlinkEffectCoroutine() // mob에 적용된 shader의 property 조절 - 반짝임 컨트롤 메서드
    {
        int curCount = 0;
        float curTime = 0;

        float curGlossiness = 0;
        Color curColor = Color.white;

        while (curCount < maxGlitterCount)
        {
            while (curTime < glitterDuration)
            {
                curTime += Time.deltaTime;
                float t = curTime / glitterDuration;

                curGlossiness = Mathf.Lerp(0, 2, t);
                curColor = Color.Lerp(Color.white, goalColor, t);

                render.material.SetFloat("_GlossMapScale", curGlossiness);
                render.material.SetColor("_Color", curColor);

                yield return null;
            }

            curTime = 0;

            while (curTime < glitterDuration)
            {
                curTime += Time.deltaTime;
                float t = curTime / glitterDuration;

                curGlossiness = Mathf.Lerp(2, 0, t);
                curColor = Color.Lerp(goalColor, Color.white, t);

                render.material.SetFloat("_GlossMapScale", curGlossiness);
                render.material.SetColor("_Color", curColor);

                yield return null;
            }

            curCount++;
        }

    }

    #endregion

    #region Attack

    public void OnAttackPoint()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Vector3 attackPos = transform.position + transform.forward * attackRange;

        Collider[] cols = Physics.OverlapSphere(attackPos, attackRange, TargetMask);

        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.TryGetComponent<IEffectable>(out IEffectable effactable))
                {
                    effactable.TakeDamage(damage);
                }
            }
        }
    }

    public void OnAttackForward()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        AttackTargetRPC();
    }


    [PunRPC]
    public void AttackTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, attackRange, TargetMask);
        if (cols.Length > 0)
        {
            float detectDegree = 35;

            foreach (var col in cols)
            {
                if (col.TryGetComponent<IEffectable>(out IEffectable target) == true)
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    float dirDot = Vector3.Dot(dir, transform.forward);
                    float standard = Mathf.Cos(detectDegree * Mathf.Deg2Rad);

                    if (dirDot > standard)
                    {
                        target.TakeDamageRPC(damage, transform.position, photonView.ViewID);
                    }
                }
            }
        }
    }

    public void AttackTargetRPC()
    {
        photonView.RPC("AttackTarget", RpcTarget.All);
    }

    #endregion

    #region SetRagDoll
    void SetRagDollState(bool isOn)
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        Collider[] cols = GetComponentsInChildren<Collider>();

        foreach (var rb in rbs)
        {
            rb.isKinematic = !isOn;
        }

        foreach (var col in cols)
        {
            col.enabled = isOn;
        }

        curCol.enabled = !isOn;
    }
    #endregion

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }


}
