using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class BossParticle : MonoBehaviourPun
{
    protected DeathKnight owner;
    
    [SerializeField] protected float damage;

    public DeathKnight Owner => owner;
    public float Damage => damage;

    protected LayerMask playerMask;

    protected virtual void Awake()
    {
        playerMask = LayerMask.GetMask("Player");
    }
}
