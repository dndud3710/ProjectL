using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite EffectIcon;
    [Tooltip("��Ʈ������")]
    public float DOTAmount;
    [Tooltip("��Ʈ ������ �ӵ� (1 => 1�ʿ� �ѹ���)")]
    public float TickSpeed;
    [Tooltip("�̵��ӵ� �г�Ƽ (animator Parameter) (0.2 => 20%�ӵ�)")]
    [Range(0f, 1f)]
    public float MovementPenalty;
    [Tooltip("�� �� ���� (3 => 3�ʵ���)")]
    public float Lifetime; // �� �� ����

    [Space(10)]
    public EffectType EffectType;
    [Tooltip("Skill Particle Color")]
    public Color EffectColor = Color.white; // particle Color
    [Tooltip("�Ӽ� �߰� ���ݷ�")]
    public float ExtraDamage;
    [Tooltip("�Ӽ� �߰� ��ٿ� (-2 => ��ٿ� 2�� ����)")]
    public float ExtraCooldown;
    
}
