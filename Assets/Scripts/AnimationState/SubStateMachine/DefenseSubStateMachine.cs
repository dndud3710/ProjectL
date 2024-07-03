using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseSubStateMachine : EnemyBaseSubState
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        Owner.SetStateBools(DeathKnight.StateBools.IsDefensing, true);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        Owner.SetStateBools(DeathKnight.StateBools.IsDefensing, false);
        Owner.ChangeAnimRPC("CounterAttack", false);
    }
}
