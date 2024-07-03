using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParamReset : StateMachineBehaviour
{
    public string[] resetBoolParameters;
    public string[] resetTriggerParameters;

    private PlayerAttack ownerObject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetParams(animator);
        foreach(var param in resetBoolParameters)
        {
            animator.SetBool(param, false);
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        ResetParams(animator);
        foreach (var param in resetBoolParameters)
        {
            animator.SetBool(param, false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetParams(animator);
        foreach (var param in resetBoolParameters)
        {
            animator.SetBool(param, false);
        }
    }

    private void ResetParams(Animator animator)
    {
        //ownerObject = animator.GetComponent<PlayerAttack>();
        ////foreach (var parameter in resetBoolParameters)
        ////{
        ////    ownerObject.HandleAnimatiorBool(parameter, false);
        ////}
        //foreach (var parameter in resetTriggerParameters)
        //{
        //    ownerObject.HandleAnimatorTrigger(parameter, false);
        //}
    }
}
