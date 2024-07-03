using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : EnemyBaseState
{
    
    [SerializeField] float minRunDist;
    [SerializeField] float checkTime;
    float curWaitTime;

    Vector2 Velocity;
    Vector2 SmoothDeltaPosition;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        

        curWaitTime = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        curWaitTime += Time.deltaTime;

        if (curWaitTime > checkTime)
        {
            if (Owner.CurTarget.gameObject.activeSelf == false || Owner.CurTarget == null) // ObjectPool에 해당 Target 객체가 반납되거나 게임 오브젝트가 꺼질 경우 일단 Idle로 보냄
            {
                Owner.photonView.RPC("ChangeAnimRPC",RpcTarget.All, "IsMoving", false); // TODO : 디버깅 해보고 잘되면 유지
                // Owner.ChangeAnim("IsMoving", false);

                return;
            }
            
            curWaitTime = 0;
        }

        agent.SetDestination(Owner.CurTarget.position);

        ChaseTarget();

        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        
    }

   
    void ChaseTarget()
    {
        if (agent.pathPending/* && agent.remainingDistance <= agent.stoppingDistance*/)
        {
            return;
        }

        Vector3 worldDeltaPosition = agent.nextPosition - Owner.transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(Owner.transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(Owner.transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);


        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

        float ratio = agent.remainingDistance / minRunDist;

        Velocity = (agent.remainingDistance >= minRunDist) ? 
            (SmoothDeltaPosition / Time.deltaTime).normalized : Vector2.Lerp(Vector2.zero, (SmoothDeltaPosition / Time.deltaTime).normalized, ratio);

        
        bool isMoving = Velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;


        //Owner.photonView.RPC("ChangeAnimRPC", RpcTarget.All, "IsMoving", isMoving);
        //Owner.photonView.RPC("ChangeAnimRPC", RpcTarget.All, "deltaX", Velocity.x);
        //Owner.photonView.RPC("ChangeAnimRPC", RpcTarget.All, "deltaZ", Velocity.y);
        //Owner.ChangeAnimRPC("IsMoving", isMoving);
        //Owner.ChangeAnimRPC("deltaX", Velocity.x);
        //Owner.ChangeAnimRPC("deltaZ", Velocity.y);
        Owner.ChangeAnim("IsMoving", isMoving);
        Owner.ChangeAnim("deltaX", Velocity.x);
        Owner.ChangeAnim("deltaZ", Velocity.y);

        
    }

    /*
     void ChaseTarget()
    {
        agent.SetDestination(curTarget.position);

        Debug.Log(agent.remainingDistance);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            return;
        }

        Vector3 worldDeltaPosition = agent.nextPosition - Owner.transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(Owner.transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(Owner.transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

        Velocity = SmoothDeltaPosition / Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Velocity = Vector2.Lerp(Vector2.zero, Velocity, agent.remainingDistance);
        }

        bool shouldMove = Velocity.magnitude > 0.5f && agent.remainingDistance > agent.stoppingDistance;

        Owner.ChangeAnim("IsMoving", shouldMove);
        Owner.ChangeAnim("deltaX", Velocity.x);
        Owner.ChangeAnim("deltaZ", Velocity.y);

        
    }
     */
}
