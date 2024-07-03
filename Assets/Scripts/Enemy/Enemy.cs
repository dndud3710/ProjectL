using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviourPun
{
    [SerializeField] protected float detectRange;
    [SerializeField] protected float attackRange;
    [SerializeField] LayerMask targetMask;
    protected Transform curTarget;


    public float DetectRange => detectRange;
    public float AttackRange => attackRange;
    public LayerMask TargetMask => targetMask;
    public Transform CurTarget => curTarget;
    
    protected void GiveExp(PlayerStatus targetPlayer, float exp)
    {
        targetPlayer.GainExp(exp);
        print($"{targetPlayer.name}이 경험치 {exp}을 얻음 ");
    }
}

