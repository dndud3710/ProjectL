using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Skill")]
public class SkillInfo : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    [Tooltip("�⺻ ��ų ���ݷ�")]
    public float baseAmount;
    [Tooltip("�⺻ ��ų ��Ÿ��")]
    public float baseCooldown;
    public float manaCost;
    [Tooltip("5 => 1/5�� ���")]
    public float animationSpeed;
    [Tooltip("��¡�ð�")]
    public float chargingDuration;
    [Tooltip("��ƼŬ ����ð� (�̼��� : 2��)")]
    public float lifeTime;
    public KeyBind bindingKey;  // Q = 0, W = 1, E = 2, R = 3

    public EffectController[] effectControllers;

}
