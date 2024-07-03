using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEndState : EnemyBaseState
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Owner.ChangeAnimRPC("NormalAttack", false);
        Debug.Log("NormalAttack �ʱ�ȭ��");

        Owner.ChangeAnimRPC("SkillAttack", false);
    }
}
