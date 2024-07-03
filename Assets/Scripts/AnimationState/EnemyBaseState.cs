using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseState : StateMachineBehaviour
{
    protected DeathKnight Owner;
    protected NavMeshAgent agent;
    protected PhotonView photonview;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Destroy(this);
        }

        Init(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        if (Owner.GetStateBools(DeathKnight.StateBools.IsDead) == true) return;

        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        //Owner.SetActiveStates(ActiveState.IsTransition, false);
    }

    protected virtual void Init(Animator animator)
    {
        if (Owner == null)
        {
            photonview = animator.GetComponent<DeathKnight>().photonView;
            Owner = animator.GetComponent<DeathKnight>();
            agent = Owner.GetComponent<NavMeshAgent>();
        }
    }
    
}
