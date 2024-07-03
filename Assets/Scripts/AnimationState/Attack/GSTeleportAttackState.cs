using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GSTeleportAttackState : AttackState
{
    //[SerializeField] float animatorSpeed;
    [SerializeField] float plusDist;
    [SerializeField] bool EnterInit;
    [SerializeField] bool ExitInit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        //animator.speed = animatorSpeed;

        if(EnterInit == true)
        {
            Owner.SetTarget(false);

            Owner.TryTeleport(Owner.CurTarget, plusDist);
        }
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        if (ExitInit == true)
        {
            Owner.SetTarget(false);

            Owner.TryTeleport(Owner.CurTarget, plusDist);
        }
    }
}
