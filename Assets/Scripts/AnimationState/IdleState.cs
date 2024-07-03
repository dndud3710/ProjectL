using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class IdleState : EnemyBaseState
{
    float rotationSpeed = 0;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        ResetVariables();

        // Owner.photonView.RPC("ChangeAnimRPC", RpcTarget.All, "IsMoving", false);
        Owner.ChangeAnimRPC("IsMoving", false);
        Owner.ChangeAnimRPC("IsCollide", false);


    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (Owner.FindTargets() == true)
        {
            Owner.SetTarget();            

            if (Owner.CurTarget != null)
            {
                agent.SetDestination(Owner.CurTarget.position);
            }
            
            bool movable = agent.remainingDistance > agent.stoppingDistance;

            Owner.photonView.RPC("ChangeAnimRPC", RpcTarget.All, "IsMoving", movable);
            // Owner.ChangeAnim("IsMoving", movable);
        }

        if (Owner.CurTarget != null)
        {
            float distance = Vector3.Distance(Owner.CurTarget.position, Owner.transform.position);

            if (distance <= agent.stoppingDistance)
            {
                Vector3 direction = (Owner.CurTarget.position - agent.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        //priorityQueue.Clear();
        //priorityQueue = null;

    }

    void ResetVariables()
    {
        Owner.ChangeAnimSpeedRPC(Owner.BaseSpeed);
        // Owner.ChangeAnimRPC("SkillAttack", false);

        agent.updateRotation = true;

        rotationSpeed = agent.angularSpeed / 60;
    }


}
