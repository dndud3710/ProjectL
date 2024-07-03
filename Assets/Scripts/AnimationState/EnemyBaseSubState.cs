using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseSubState : StateMachineBehaviour
{
    protected DeathKnight Owner;
    protected NavMeshAgent agent;
    protected PhotonView photonview;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (Owner == null)
        {
            photonview = animator.GetComponent<DeathKnight>().photonView;
            Owner = animator.GetComponent<DeathKnight>();
            agent = Owner.GetComponent<NavMeshAgent>();
        }
    }
}
