using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackSubState : EnemyBaseSubState
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        agent.updateRotation = false;
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        agent.updateRotation = true;
    }
}
