using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MageClass : PlayerAttack
{
    protected override void Init()
    {
        base.Init();
        Space = GameManager.Instance.GetSkillInfo("Mage0_Space");
        Q = GameManager.Instance.GetSkillInfo("Mage1_Q");
        W = GameManager.Instance.GetSkillInfo("Mage2_W");
        E = GameManager.Instance.GetSkillInfo("Mage3_E");
        R = GameManager.Instance.GetSkillInfo("Mage4_R");


        animator.SetInteger("Class", 2);

        skillList.Add(new SkillInstance(this, Space));
        skillList.Add(new SkillInstance(this, Q));
        skillList.Add(new SkillInstance(this, W));
        skillList.Add(new SkillInstance(this, E));
        skillList.Add(new SkillInstance(this, R));

        if (photonView.IsMine)
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

                chargeDuration = skillList[(int)KeyBind.E].skillInfo.chargingDuration;
                StartCharging();
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && isCharging)
        {
            StopCharging();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (skillList[(int)KeyBind.R].Activate())
            {
                LookAtMousePosition();
                move.ResetPath();

                isCharging = false;
            }
        }

    }

    protected override void SetPlayerDamage()
    {
        damage.minDamage = (status.playerStat.INT * 1.3f) + (status.playerStat.DEX + 2);
        damage.minDamage = (status.playerStat.INT * 1.6f) + (status.playerStat.DEX + 2);
        damage.accuracy = (status.playerStat.DEX * 0.5f) / 100;
    }

    #region Attack EventHandler
    private void MageAttack()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, (int)KeyBind.Space, 0);
    }

    private void MageQSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.Q;

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void MageWSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.W;

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void MageESkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.E;

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void MageRSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.R;
        Vector3 skillPos = transform.position + transform.forward * 3;
        photonView.RPC("CreateParticle", RpcTarget.All, skillPos, transform.forward, skillIndex, 0);
    }
    #endregion
}
