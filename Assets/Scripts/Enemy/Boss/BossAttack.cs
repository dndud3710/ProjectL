using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    Animator animator;

    AnimationEventHandler eventHandler;

    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        eventHandler = GetComponent<AnimationEventHandler>();
    }

    public void AlertAttack(bool isIn) // TODO : ���Ŀ� Manager�� Player ����� �ڷᱸ���� ��Ƴ��´ٸ� �ش� Player����� �Ÿ��� ���ؼ� �ȹ� ���� ���� ���θ� �����ϱ�
    {
        // eventHandler.OnMakeAttackParticle(isIn? circleInData : circleOutData);
    }
}
