using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComboSubState : EnemyBaseSubState
{
    [SerializeField] float animatorComboSpeed;
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        animator.speed = animatorComboSpeed;
        Owner.ChangeAnimRPC("IsCollide", false);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        animator.speed = 1f;

        
        // Owner.ChangeAnim("IsCollide", false);
    }
}
