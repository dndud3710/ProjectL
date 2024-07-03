using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossInstParticle : BossParticle
{
    protected ParticleSystem[] particles;
    
    protected int playerLayer;

    [SerializeField] protected float maxDuration;

    protected object[] datas;

    [SerializeField] protected ParticleType particleType;
    public ParticleType ParticleType => particleType;

    Collider[] ownCols;

    protected override void Awake()
    {
        base.Awake();

        particles = GetComponentsInChildren<ParticleSystem>();

        datas = photonView.InstantiationData;
        playerLayer = LayerMask.NameToLayer("Player");

        foreach (var particle in particles)
        {
            var main = particle.main;
            if (main.startLifetime.constant > maxDuration)
            {
                maxDuration = main.startLifetime.constant;
            }
        }
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(WaitReturnCoroutine());
    }

    IEnumerator WaitReturnCoroutine()
    {
        float purposeTime = Time.time + maxDuration;

        while (Time.time < purposeTime)
        {
            yield return null;
        }

        Destroy(gameObject);
        // TODO : 오브젝트풀에 반납하는 로직 작성
    }

    public void Init(int ownerId)
    {
        PhotonView view = PhotonView.Find(ownerId);
        owner = view.GetComponent<DeathKnight>();
    }
}
