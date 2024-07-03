using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EquipState : EnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Owner.CurWeapon == null) return; // animation event로 curWeapon이 할당되는 지점까지 return 

        Owner.CurWeapon.transform.localPosition = Vector3.zero;
        Owner.CurWeapon.transform.localRotation = Quaternion.identity;

    }

}
