using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorClass : PlayerAttack
{
    protected override void Init()
    {
        base.Init();
        Space = GameManager.Instance.GetSkillInfo("Warrior0_Space");
        Q = GameManager.Instance.GetSkillInfo("Warrior1_Q");
        W = GameManager.Instance.GetSkillInfo("Warrior2_W");
        E = GameManager.Instance.GetSkillInfo("Warrior3_E");
        R = GameManager.Instance.GetSkillInfo("Warrior4_R");

        animator.SetInteger("Class", 0);
        skillList.Add(new SkillInstance(this, Space));
        skillList.Add(new SkillInstance(this, Q));
        skillList.Add(new SkillInstance(this, W));
        skillList.Add(new SkillInstance(this, E));
        skillList.Add(new SkillInstance(this, R));

        //if(photonView.IsMine)
        //    GameManager.Instance.UI_SetSkill(skillList);
        if(photonView.IsMine)
            PlayerUIManager.Instance.UI_SetSkill(skillList);
    }

    protected override void HandleSkillInput()
    {
        base.HandleSkillInput();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skillList[(int)KeyBind.Q].Activate())
            {
                LookAtMousePosition();
                move.ResetPath();

                isCharging = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (skillList[(int)KeyBind.W].Activate())
            {
                LookAtMousePosition();
                move.ResetPath();

                isCharging = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (skillList[(int)KeyBind.E].Activate())
            {
                LookAtMousePosition();
                move.ResetPath();

                isCharging = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (skillList[(int)KeyBind.R].Activate())
            {
                LookAtMousePosition();
                move.ResetPath();

                chargeDuration = skillList[(int)KeyBind.R].skillInfo.chargingDuration;
                StartCharging();
            }
        }

        if (Input.GetKeyUp(KeyCode.R) && isCharging)
        {
            StopCharging();
        }
    }
    protected override void SetPlayerDamage()
    {
        damage.minDamage = (status.playerStat.STR * 1.1f) + (status.playerStat.DEX + 2);
        damage.maxDamage = (status.playerStat.STR * 1.5f) + (status.playerStat.DEX + 2);
        damage.accuracy = (status.playerStat.DEX * 0.8f) / 100;
    }

    #region Attack EventHandler
    private void WarriorAttack(int particleIndex)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.Space;

        
        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, particleIndex);
    }

    private void WarriorQSkillEvent(float damageMultiply)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.Q;
        
        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void WarriorWSkillEvent(int particleIndex)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.W;

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, particleIndex);
    }

    private void WarriorESkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.E;
        
        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void WarriorRSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.R;
        
        photonView.RPC("CreateParticle", RpcTarget.All, transform.position + transform.forward * 4.5f, transform.forward, skillIndex, 0);
    }

    #endregion
    public float radius;
    private void OnDrawGizmos()
    {
        Vector3 attackSphereCenter = transform.position + transform.forward;

        Gizmos.DrawWireSphere(attackSphereCenter, 1.5f);
    }
}
