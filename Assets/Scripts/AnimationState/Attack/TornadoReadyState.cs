using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoReadyState : EnemyBaseState
{
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

        Owner.SetObjectActiveRPC(Owner.DeathSpiral.ViewId, true);
        Owner.DeathSpiral.StartParticleRPC();
    }
}
