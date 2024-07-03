using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportReadyState : EnemyBaseState
{
    [SerializeField] float plusDist;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Owner.SetTarget(false);

        Owner.TryTeleport(Owner.CurTarget, plusDist);

    }
}
