using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossInstallParticle : BossInstParticle
{
    [SerializeField] float attackInterval;

    [Space(20)]

    [SerializeField] float moveDistX;
    [SerializeField] float moveDistY;
    [SerializeField] float moveDistZ;

    [Space(20)]

    [SerializeField] float attackRangeX;
    [SerializeField] float attackRangeY;
    [SerializeField] float attackRangeZ;

    [SerializeField] float attackRange;

    Coroutine attackCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();

        print($"PhotonNetwork.IsMasterClient : {PhotonNetwork.IsMasterClient}");

        if(PhotonNetwork.IsMasterClient == true)
        {
            attackCoroutine = StartCoroutine(AttackPlayerCoroutine());
        }
        
    }

    protected virtual void OnDisable()
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator AttackPlayerCoroutine()
    {
        while(true)
        {
            Vector3 localMove = new Vector3(moveDistX, moveDistY, moveDistZ);
            Vector3 worldMove = transform.TransformDirection(localMove);
            Vector3 position = transform.position + worldMove;

            Collider[] cols = Physics.OverlapSphere(position, attackRange, playerMask);

            if(cols.Length > 0)
            {
                foreach(var col in cols)
                {
                    if(col.TryGetComponent<IEffectable>(out var target))
                    {
                        target.TakeDamageRPC(damage, position, photonView.ViewID);
                    }                    
                }
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

}
