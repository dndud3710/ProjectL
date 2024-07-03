using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviourPun
{
    public LayerMask groundLayerMask;
    public LayerMask excludeLayerMask;

    private PlayerStatus status;
    private Camera cam;
    private NavMeshAgent agent;

    private Animator animator;

    #region Animator params

    private readonly int hashIsAttacking = Animator.StringToHash("isAttacking");
    private readonly int hashUsingSkill = Animator.StringToHash("usingSkill");
    private bool IsAttacking => animator.GetBool(hashIsAttacking);
    private bool UsingSkill => animator.GetBool(hashUsingSkill);
    #endregion


    private void Awake()
    {
        status = GetComponent<PlayerStatus>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        StartCoroutine(AgentEnableCoroutine());
    }
    IEnumerator AgentEnableCoroutine()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(0.5f);
        agent.enabled = true;
        animator.SetBool("isMoving", false);

    }
    private void Start()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        agent.updateRotation = !photonView.IsMine;
        agent.updatePosition = !photonView.IsMine;
        animator.SetInteger("Class", (int)status.playerClass);
    }

    private void Update()
    {
        if (status.CanMove == false)
            return;

        if (status.Alive == false)
            return;

        Move();
        UpdateAnimation();

    }

    private void Move()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButton(1) && !IsAttacking && !UsingSkill)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, groundLayerMask))
            {   
                if (Vector3.Distance(hit.point, transform.position) <= 0.2f)
                    return;
                agent.SetDestination(hit.point);

                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            }
            else
            {
                ResetPath();
            }
        }
    }
    
    private void UpdateAnimation()
    {
        // 이동 중이면서 목적지에 가까워졌는지 확인
        if (agent.hasPath && !agent.pathPending)
        {
            // remainingDistance가 stoppingDistance 이하면 목적지에 도달한 것으로 간주
            if (agent.remainingDistance <= agent.stoppingDistance || UsingSkill)
            {
                // 목적지에 도착하면 이동을 멈춤
                ResetPath();
            }
            else
            {
                animator.SetBool("isMoving", true);
            }
        }

        agent.nextPosition = transform.position;
        transform.position = agent.nextPosition;
    }

    public void ResetPath()
    {
        agent.ResetPath();
        animator.SetBool("isMoving", false);
    }



    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            for (var i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.blue);
            }
        }
    }

}
