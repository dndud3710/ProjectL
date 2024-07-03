using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSlashParticle : BossInstParticle
{
    public enum SlashType
    {
        Slash, EnhancedSlash, Wheel
    }

    [SerializeField] protected float moveSpeed;
    [SerializeField] float lifeTime;

    [SerializeField] int enhancedSlashCount;

    [SerializeField] SlashType slashType;
    
    protected override void Awake()
    {
        base.Awake();

        foreach(var particle in particles)
        {
            var main = particle.main;
            main.startLifetime = lifeTime;
        }
    }


    private void LateUpdate()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        if(col.gameObject.layer == playerLayer)
        {
            if(col.TryGetComponent<IEffectable>(out IEffectable target))
            {
                target.TakeDamageRPC(damage, transform.position, photonView.ViewID);
            }
            

            switch(slashType)
            {
                case SlashType.EnhancedSlash:

                    for (int i = 0; i < enhancedSlashCount; i++)
                    {
                        float angleInDegrees = 360f * i / enhancedSlashCount;

                        PhotonNetwork.Instantiate("Prefabs/Enemy/BossParticle/SwordSlashPrefab", col.transform.position, Quaternion.Euler(0, angleInDegrees, 0), 0, null);
                    }

                    PhotonNetwork.Destroy(gameObject);
                    break;

                case SlashType.Slash:

                    
                    PhotonNetwork.Destroy(gameObject);

                    break;
                case SlashType.Wheel:


                    break;

            }
        }
    }
}
