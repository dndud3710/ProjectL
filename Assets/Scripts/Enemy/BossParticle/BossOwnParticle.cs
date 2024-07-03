using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossOwnParticle : BossParticle
{
    ParticleSystem[] particles;
    int viewId;
    public int ViewId => viewId;

    Coroutine attackCoroutine;

    [SerializeField] float attackInterval;
    [SerializeField] float attackRange;

    [SerializeField] bool isAttackable;

    protected override void Awake()
    {
        base.Awake();

        particles = GetComponentsInChildren<ParticleSystem>();
        viewId = photonView.ViewID;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(isAttackable == true && PhotonNetwork.IsMasterClient == true)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
        
    }

    private void OnDisable()
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    [PunRPC]
    public void StartParticle()
    {
        foreach(var particle in particles)
        {
            var main = particle.main;
            main.loop = true;

            particle.Play();
        }
    }

    IEnumerator AttackCoroutine()
    {
        if (PhotonNetwork.IsMasterClient == false) yield break;

        while(true)
        {
            Collider[] players = Physics.OverlapSphere(transform.position, attackRange , playerMask);
            if(players.Length > 0)
            {
                foreach(var col in players)
                {
                    if(col.TryGetComponent<IEffectable>(out IEffectable target) == true)
                    {
                        target.TakeDamageRPC(damage, transform.position, photonView.ViewID);
                    }
                    
                }
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    public void StartParticleRPC()
    {
        photonView.RPC("StartParticle", RpcTarget.All);
    }

    [PunRPC]
    public void FinishParticle()
    {
        foreach(var particle in particles)
        {
            var main = particle.main;
            main.loop = false;
        }

        gameObject.SetActive(false);
    }

    public void FinishParticleRPC()
    {
        photonView.RPC("FinishParticle", RpcTarget.All);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
