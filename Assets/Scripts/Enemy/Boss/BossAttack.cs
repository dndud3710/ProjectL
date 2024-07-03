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

    public void AlertAttack(bool isIn) // TODO : 추후에 Manager가 Player 목록을 자료구조에 담아놓는다면 해당 Player들과의 거리를 비교해서 안밖 공격 판정 여부를 적용하기
    {
        // eventHandler.OnMakeAttackParticle(isIn? circleInData : circleOutData);
    }
}
