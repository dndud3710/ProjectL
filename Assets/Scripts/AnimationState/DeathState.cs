using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : EnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        agent.updatePosition = false;
        agent.updateRotation = false;
    }
}
