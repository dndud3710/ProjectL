using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseState : AttackState // TODO : 추후에 Player 기절 상태와 TakeDamage 메서드 호출시 IsDefensing 여부를 확인해서 기절 상태로 넘기는 이벤트가 작성되면 이 상태 작성하기
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) // TODO : TakeDamage가 발생할 때와 해당 상태가 겹칠 때 
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // Owner.ChangeAnim()
    }
}
