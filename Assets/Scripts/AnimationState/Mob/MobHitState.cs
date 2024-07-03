using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHitState : StateMachineBehaviour
{
    TestMob Owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Owner = animator.GetComponent<TestMob>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Owner.IsUnderAttack = false;
    }
}
