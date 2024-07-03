using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarState : EnemyBaseState
{
    [SerializeField] float animSpeed;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Owner.ChangeAnimSpeedRPC(animSpeed);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Owner.ChangeAnimSpeedRPC(Owner.BaseSpeed);
    }
}
