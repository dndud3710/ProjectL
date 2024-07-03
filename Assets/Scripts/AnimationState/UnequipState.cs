using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnequipState : EnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Owner == null)
        {
            photonview = animator.GetComponent<DeathKnight>().photonView;
            Owner = animator.GetComponent<DeathKnight>();
            agent = Owner.GetComponent<NavMeshAgent>();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Owner.CurWeapon == null) // animation event로 curWeapon이 할당되는 지점까지 return 
        {
            return;
        }

        Owner.CurWeapon.transform.localPosition = Vector3.zero;
        Owner.CurWeapon.transform.localRotation = Quaternion.identity;

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        int nextWeaponIndex = (animator.GetInteger("CurWeaponState") == 0) ? 1 : 0;

        Owner.ChangeAnimRPC("CurWeaponState", nextWeaponIndex);
    }
}
