using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseState : AttackState // TODO : ���Ŀ� Player ���� ���¿� TakeDamage �޼��� ȣ��� IsDefensing ���θ� Ȯ���ؼ� ���� ���·� �ѱ�� �̺�Ʈ�� �ۼ��Ǹ� �� ���� �ۼ��ϱ�
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) // TODO : TakeDamage�� �߻��� ���� �ش� ���°� ��ĥ �� 
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // Owner.ChangeAnim()
    }
}
