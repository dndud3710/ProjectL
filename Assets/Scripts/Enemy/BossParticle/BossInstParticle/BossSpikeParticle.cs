using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpikeParticle : BossInstParticle
{
    [SerializeField] float attackRange;

    protected override void OnEnable()
    {
        base.OnEnable();

        if(PhotonNetwork.IsMasterClient == true)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, attackRange, playerMask);
            if (cols.Length > 0)
            {
                foreach (var col in cols)
                {
                    if(col.TryGetComponent<IEffectable>(out IEffectable target))
                    {
                        target.TakeDamageRPC(damage, transform.position, photonView.ViewID);
                    }
                    
                }
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
