using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SkillEndState : EnemyBaseState
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Owner.ChangeAnimRPC("SkillAttack", false);
        Debug.Log("SkillAttack √ ±‚»≠µ ");

        Owner.ChangeAnimRPC("NormalAttack", false);
    }
}
