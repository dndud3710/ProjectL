using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Class
{
    Warrior = 0,
    Gunner = 1,
    Mage = 2,
}

public enum EffectType
{
    None,
    Fire,
    Lightning,
    Ice,
    Poison
}

public enum KeyBind
{
    Space = 0,
    Q = 1,
    W = 2,
    E = 3,
    R = 4
}

[Serializable]
public class Status
{
    public float STR;
    public float DEX;
    public float INT;
    public float DEF;

    public Status()
    {
        STR = 4;
        DEX = 4;
        INT = 4;
        DEF = 4;
    }

    public Status(float STR, float DEX, float INT, float DEF)
    {
        if (STR <= 0)
            STR = 0;
        if (DEX <= 0)
            DEX = 0;
        if(INT <= 0)
            INT = 0;
        if(DEF <= 0)
            DEF = 0;

        this.STR = STR;
        this.DEX = DEX;
        this.INT = INT;
        this.DEF = DEF;
    }
    public void LevelUp()
    {
        STR += 5;
        DEX += 5;
        INT += 5;
        DEF += 5;
    }

    public static Status operator +(Status a, Status b)
    {
        return new Status(a.STR + b.STR, a.DEX + b.DEX, a.INT + b.INT, a.DEF + b.DEF);
    }

    public static Status operator -(Status a, Status b)
    {
        return new Status(a.STR - b.STR, a.DEX - b.DEX, a.INT - b.INT, a.DEF - b.DEF);
    }
}

/// <summary>
/// attackRating(���߷�) = 0~1�� ����
/// </summary>
[Serializable]
public class Damage
{
    public float minDamage;
    public float maxDamage;

    [Range(0f, 1f)]
    public float accuracy;
    public float GetDamage()
    {
        float randomFactor = Random.value; // 0 ~ 1 ������ ���� ��

        // accuracy ���� ���� ���� ���͸� �����Ͽ� ������ �ְ��ŵ�ϴ�.
        // accuracy�� �������� (1�� ��������) randomFactor ���� �� ���� ���� �� (1�� ����� ��)�� ���� ���Դϴ�.
        // Mathf.Pow �Լ��� ����Ͽ� �� ȿ���� ��ȭ�մϴ�.
        randomFactor = Mathf.Pow(randomFactor, 1.0f - accuracy);

        // ���������� ���� �������� minDamage�� maxDamage ������ ���Դϴ�.
        float damage = Mathf.Lerp(minDamage, maxDamage, randomFactor);
        return damage;
    }
}