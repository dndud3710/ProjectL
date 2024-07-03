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

    public float skillCooldown; // �Ӽ��� ���� ��Ÿ�� �޶���
    public float skillDamage;   // �Ӽ� + ��ų �⺻������

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
        // 0 �˻�
        float cooldown = skillCooldown == 0 ? 1 : skillCooldown;

        cooldownTimer = new Timer(cooldown * 1000); // Timer�� ms����
        cooldownTimer.Elapsed +=
            (sender, e) =>
            {
                canUse = true;
                OnCooldownStart?.Invoke(0); // ��ٿ� ����
                StopCooldown();
            };
        cooldownTimer.AutoReset = false;
    }


    private void CalcSkillAttributes()
    {
        // �Ӽ��� �°� ������, ��ٿ� �� ���
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
            PlayerUIManager.Instance.UI_ShowMessage("��ų�� ����� �� �����ϴ�.");
            status.ShakeCamera(0.1f, 0.2f);
            return false;
        }

        if (status.CurMP < skillInfo.manaCost)
        {
            PlayerUIManager.Instance.UI_ShowMessage("������ �����մϴ�.");
            status.ShakeCamera(0.1f, 0.2f);
            return false;
        }

        status.UseMana(skillInfo.manaCost);

        StartCooldown(); //��ٿ� ����

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
