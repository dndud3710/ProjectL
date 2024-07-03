using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNTeleportAttackState : AttackState
{
    [SerializeField] bool closestTarget;
    // [SerializeField] 

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if(Owner.FindTargets() == true)
        {
            Owner.SetTarget(false);
        }
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }


}
