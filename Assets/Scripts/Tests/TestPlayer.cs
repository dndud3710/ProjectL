using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviourPun, IEffectable
{
    Renderer render;
    Material curMat;

    float curHp;
    float maxHp;
    public float CurHP { get => curHp; set => curHp = value; }
    public float MaxHP { get => maxHp; set => maxHp = value; }

    private void Awake()
    {
        render = GetComponent<Renderer>();
        curMat = render.material;
    }

    [PunRPC]
    public void TakeDamage(float damage, Vector3 attackerPos, int viewId)
    {
        curMat.color = Color.red;
    }

    public void TakeDamageRPC(float damage, Vector3 attackerPos, int viewId)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage,attackerPos, viewId);
    }


    public void ApplyEffect(StatusEffectData effectData, int viewId)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator EffectCoroutine(StatusEffectData data, int viewId)
    {
        yield return null;
    }
}
