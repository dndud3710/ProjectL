using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSkillState : EnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        agent.updateRotation = false;
    }
}
