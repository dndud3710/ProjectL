using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAttackSubState : EnemyBaseSubState
{
    [SerializeField] float animSpeed;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        agent.updateRotation = false;

        Owner.SetStateBools(DeathKnight.StateBools.IsTeleporting, true);

        Owner.ChangeAnimSpeedRPC(animSpeed);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        agent.updateRotation = true;

        Owner.SetStateBools(DeathKnight.StateBools.IsTeleporting, false);

        Owner.ChangeAnimSpeedRPC(Owner.BaseSpeed);
    }
}
