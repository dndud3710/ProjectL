using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Skill")]
public class SkillInfo : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    [Tooltip("기본 스킬 공격력")]
    public float baseAmount;
    [Tooltip("기본 스킬 쿨타임")]
    public float baseCooldown;
    public float manaCost;
    [Tooltip("5 => 1/5로 재생")]
    public float animationSpeed;
    [Tooltip("차징시간")]
    public float chargingDuration;
    [Tooltip("파티클 생명시간 (미설정 : 2초)")]
    public float lifeTime;
    public KeyBind bindingKey;  // Q = 0, W = 1, E = 2, R = 3

    public EffectController[] effectControllers;

}
