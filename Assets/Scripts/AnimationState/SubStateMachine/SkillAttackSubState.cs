using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackSubState : EnemyBaseSubState
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        Owner.ChangeAnimRPC("SkillAttack", false);
    }
}
