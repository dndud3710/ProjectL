using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite EffectIcon;
    [Tooltip("도트데미지")]
    public float DOTAmount;
    [Tooltip("도트 데미지 속도 (1 => 1초에 한번씩)")]
    public float TickSpeed;
    [Tooltip("이동속도 패널티 (animator Parameter) (0.2 => 20%속도)")]
    [Range(0f, 1f)]
    public float MovementPenalty;
    [Tooltip("몇 초 동안 (3 => 3초동안)")]
    public float Lifetime; // 몇 초 동안

    [Space(10)]
    public EffectType EffectType;
    [Tooltip("Skill Particle Color")]
    public Color EffectColor = Color.white; // particle Color
    [Tooltip("속성 추가 공격력")]
    public float ExtraDamage;
    [Tooltip("속성 추가 쿨다운 (-2 => 쿨다운 2초 감소)")]
    public float ExtraCooldown;
    
}
