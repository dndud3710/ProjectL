using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    Vector3[] PrevPos;
    HashSet<GameObject> hashSet = new();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        PrevPos = new Vector3[Owner.CurWeapon.DetectPoints.Length];
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (Owner.CurWeapon.GetWeaponBools(BossWeapon.Booleans.IsChecking) == true)
        {
            Owner.CurWeapon.CheckAndAttackTarget(hashSet, PrevPos);
        }
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        hashSet.Clear();
        PrevPos = null;
    }
}
