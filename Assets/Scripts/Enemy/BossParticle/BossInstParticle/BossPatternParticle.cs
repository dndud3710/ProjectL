using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternParticle : BossInstParticle
{
    [SerializeField] bool isIn;
    [SerializeField] float attackRange;

    protected override void OnEnable()
    {
        base.OnEnable();

        if(PhotonNetwork.IsMasterClient == true)
        {
            foreach(var target in DungeonManager.Instance.playerList)
            {
                float dist = Vector3.Distance(target.transform.position, transform.position);

                if((isIn == true && dist < attackRange) || (isIn == false && dist > attackRange))
                {
                    if(target != null)
                    {
                        target.GetComponent<IEffectable>().TakeDamageRPC(damage, transform.position, photonView.ViewID);
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
