using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AttackSubStateMachine : EnemyBaseSubState
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        Owner.SetStateBools(DeathKnight.StateBools.IsAttacking, true);

    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        Owner.SetStateBools(DeathKnight.StateBools.IsAttacking, false);
    }
}
