using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AttackReset : StateMachineBehaviour
{
    [SerializeField] string triggerName;

    public string isAttacking;

    private PlayerMove ownerObject;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("usingSkill", false);
        animator.SetBool(isAttacking, false);
        animator.ResetTrigger(triggerName);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        animator.SetBool(isAttacking, false);
    }

    private void ResetParam(Animator animator)
    {

    }
}
