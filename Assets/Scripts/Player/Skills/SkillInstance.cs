using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class SkillInstance
{
    private Animator animator; // Player Animator
    private PlayerAttack player;
    private PlayerStatus status;

    public event Action<float> OnCooldownStart;
    public Timer cooldownTimer;
    private bool canUse = true;

    public float skillCooldown; // 속성에 따라 쿨타임 달라짐
    public float skillDamage;   // 속성 + 스킬 기본데미지

    public SkillInfo skillInfo;
    public StatusEffectData effectData;

    #region Animation Params Hash
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashUsingSkill = Animator.StringToHash("usingSkill");
    private readonly int hashSkillInt = Animator.StringToHash("SkillInteger");
    private bool UsingSkill => animator.GetBool(hashUsingSkill);
    #endregion

    public SkillInstance(PlayerAttack player, SkillInfo skillInfo)
    {
        this.skillInfo = skillInfo;
        this.player = player;
        CalcSkillAttributes();

        status = player.GetComponent<PlayerStatus>();
        animator = player.GetComponent<Animator>();
        effectData = GameManager.Instance.GetEffectData(EffectType.None);
        // 0 검사
        float cooldown = skillCooldown == 0 ? 1 : skillCooldown;

        cooldownTimer = new Timer(cooldown * 1000); // Timer는 ms단위
        cooldownTimer.Elapsed +=
            (sender, e) =>
            {
                canUse = true;
                OnCooldownStart?.Invoke(0); // 쿨다운 종료
                StopCooldown();
            };
        cooldownTimer.AutoReset = false;
    }


    private void CalcSkillAttributes()
    {
        // 속성에 맞게 데미지, 쿨다운 등 계산
        if (effectData != null)
        {
            skillDamage = skillInfo.baseAmount + effectData.ExtraDamage;
            skillCooldown = skillInfo.baseCooldown - effectData.ExtraCooldown;
        }
        else
        {
            skillDamage = skillInfo.baseAmount;
            skillCooldown = skillInfo.baseCooldown;
        }

        if (skillCooldown == 0)
        {
            skillCooldown = skillInfo.baseCooldown;
        }
    }

    public bool Activate()
    {
        if (!canUse)
        {
            PlayerUIManager.Instance.UI_ShowMessage("스킬을 사용할 수 없습니다.");
            status.ShakeCamera(0.1f, 0.2f);
            return false;
        }

        if (status.CurMP < skillInfo.manaCost)
        {
            PlayerUIManager.Instance.UI_ShowMessage("마나가 부족합니다.");
            status.ShakeCamera(0.1f, 0.2f);
            return false;
        }

        status.UseMana(skillInfo.manaCost);

        StartCooldown(); //쿨다운 시작

        var animationSpeed = skillInfo.animationSpeed == 0 ? 1 : skillInfo.animationSpeed;

        player.HandleAnimatorTrigger("Attack", false);
        player.HandleAnimatorInteger("Class", (int)status.playerClass);
        player.HandleAnimatorFloat("AnimationSpeed", 1 / animationSpeed);
        player.HandleAnimatorInteger("SkillInteger", (int)skillInfo.bindingKey);
        player.HandleAnimatorBool("usingSkill", true);

        return true;
    }

    public void StartCooldown()
    {
        canUse = false;
        OnCooldownStart?.Invoke(skillCooldown);
        cooldownTimer.Start();
    }

    public void StopCooldown()
    {
        canUse = true;
        cooldownTimer.Stop();
    }

    public void SetEffectData(StatusEffectData data)
    {
        effectData = data;
        CalcSkillAttributes();
    }
}
