using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EffectController : MonoBehaviour
{
    [Tooltip("�鿡 ���� ���� ���� �� ��ƼŬ")]
    public ParticleSystem[] colorParticleSystems;
    [Tooltip("�� �� �Ŀ� �浹 ������ �������")]
    public float delay;
    [Tooltip("�ݶ��̴� ����ð�")]
    [Range(0.1f, 5f)]
    public float lifeTime = 0.1f;
    [Tooltip("����ü �ݶ��̴� �ӵ�")]
    public float colliderSpeed;
    [Tooltip("�浹 ���� Ƚ��")]
    [Range(1, 10)]
    public int colliderCount =1;
    [Header("ī�޶� ��鸲 �Ӽ�")]
    [Tooltip("��鸲 ����")]
    public float shakeIntensity;
    [Tooltip("��鸲 �ð�")]
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

    public void SetEffectData(StatusEffectData effectData) // ��ų ��ȭ ȿ��
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
