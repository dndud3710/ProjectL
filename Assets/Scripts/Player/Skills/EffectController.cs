using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EffectController : MonoBehaviour
{
    [Tooltip("룬에 따라 색상 변경 될 파티클")]
    public ParticleSystem[] colorParticleSystems;
    [Tooltip("몇 초 후에 충돌 감지를 사용할지")]
    public float delay;
    [Tooltip("콜라이더 생명시간")]
    [Range(0.1f, 5f)]
    public float lifeTime = 0.1f;
    [Tooltip("투사체 콜라이더 속도")]
    public float colliderSpeed;
    [Tooltip("충돌 감지 횟수")]
    [Range(1, 10)]
    public int colliderCount =1;
    [Header("카메라 흔들림 속성")]
    [Tooltip("흔들림 강도")]
    public float shakeIntensity;
    [Tooltip("흔들림 시간")]
    public float shakeTime;

    public StatusEffectData effectData { get; private set; }
    public Damage damage;
    public EffectTrigger attackCollider;
    private bool isMine = false;
    private int viewID;

    private void OnEnable()
    {
        StartCoroutine(ColliderEnableCoroutine());
    }

    public void SetEffectData(StatusEffectData effectData) // 스킬 강화 효과
    {
        this.effectData = effectData;
        var newColor = effectData.EffectColor;

        foreach (var particle in colorParticleSystems)
        {
            var main = particle.main;
            var alpha = main.startColor.color.a;
            main.startColor = new Color(newColor.r, newColor.g, newColor.b, alpha);
        }
    }
    

    public void SetDamage(Damage damage, bool isMine, int viewID)
    {
        this.damage = damage;
        this.isMine = isMine;
        this.viewID = viewID;
    }

    public void HandleTrigger(Collider other, Vector3 colliderPos)
    {
        //if (gameObject.layer == other.gameObject.layer)
        //    return;

        if (false == other.CompareTag("Enemy"))
            return;

        if (isMine == false)
            return;

        if (other.TryGetComponent(out IEffectable test))
        {
            test.TakeDamageRPC(damage.GetDamage(), colliderPos, viewID);
            
            test.ApplyEffect(effectData, viewID);
        }
    }

    IEnumerator ColliderEnableCoroutine()
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < colliderCount; i++)
        {
            attackCollider.EnableCollider(rigidVelocity: transform.forward * colliderSpeed);
            if(isMine)
                GameManager.Instance.playerStatus.ShakeCamera(shakeIntensity, shakeTime);
            float t = 0;
            while (t < lifeTime)
            {
                t += Time.deltaTime;

                yield return null;
            }
            attackCollider.DisableCollider();
            yield return new WaitForSeconds(1.5f);
        }
        
    }
}
