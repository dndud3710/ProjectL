using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMakeUI
{
    public void ObjectDestroy();
}
public interface IHittable
{
    public float CurHP {  get; set; }
    public float MaxHP { get; set; }
    
    public void TakeDamage(float damage, Vector3 attackerPos = default, int viewId = 0);
    public void TakeDamageRPC(float damage, Vector3 attackerPos, int viewId);
    
}

public interface IEffectable : IHittable
{
    public void ApplyEffect(StatusEffectData effectData, int viewId);

    IEnumerator EffectCoroutine(StatusEffectData data, int viewId);
}