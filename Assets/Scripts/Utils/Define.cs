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
/// attackRating(명중률) = 0~1로 제한
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
        float randomFactor = Random.value; // 0 ~ 1 사이의 랜덤 값

        // accuracy 값에 따라 랜덤 팩터를 조정하여 분포를 왜곡시킵니다.
        // accuracy가 높을수록 (1에 가까울수록) randomFactor 값은 더 자주 높은 값 (1에 가까운 값)을 가질 것입니다.
        // Mathf.Pow 함수를 사용하여 이 효과를 강화합니다.
        randomFactor = Mathf.Pow(randomFactor, 1.0f - accuracy);

        // 최종적으로 계산된 데미지는 minDamage와 maxDamage 사이의 값입니다.
        float damage = Mathf.Lerp(minDamage, maxDamage, randomFactor);
        return damage;
    }
}