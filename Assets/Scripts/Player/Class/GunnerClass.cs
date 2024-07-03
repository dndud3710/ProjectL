using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GunnerClass : PlayerAttack
{
    protected override void Init()
    {
        base.Init();
        Space = GameManager.Instance.GetSkillInfo("Gunner0_Space");
        Q = GameManager.Instance.GetSkillInfo("Gunner1_Q");
        W = GameManager.Instance.GetSkillInfo("Gunner2_W");
        E = GameManager.Instance.GetSkillInfo("Gunner3_E");
        R = GameManager.Instance.GetSkillInfo("Gunner4_R");

        animator.SetInteger("Class", 1);
        skillList.Add(new SkillInstance(this, Space));
        skillList.Add(new SkillInstance(this, Q));
        skillList.Add(new SkillInstance(this, W));
        skillList.Add(new SkillInstance(this, E));
        skillList.Add(new SkillInstance(this, R));

        //if (photonView.IsMine)
        //    GameManager.Instance.UI_SetSkill(skillList);
        if (photonView.IsMine)
        {
            PlayerUIManager.Instance.UI_SetSkill(skillList);
        }
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
        damage.minDamage = (status.playerStat.DEX * 1.1f) + (status.playerStat.STR + 1.5f);
        damage.maxDamage = (status.playerStat.DEX * 1.5f) + (status.playerStat.STR + 2f);
        damage.accuracy = (status.playerStat.DEX * 0.5f) / 100;
    }


    #region Attack EventHandler
    private void GunnerShoot()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (Physics.BoxCast(transform.position, castBox / 2, transform.forward, out var hit, transform.rotation, castDistance, enemyLayer))
        {
            
            photonView.RPC("CreateParticle", RpcTarget.All, hit.collider.transform.position, transform.forward, (int)KeyBind.Space,0);
        }
    }

    private void GunnerQSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.Q;
        
        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void GunnerWSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.W;

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    private void GunnerESkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.E;

        var skillPos = transform.position + transform.forward * 2f;
        photonView.RPC("CreateParticle", RpcTarget.All, skillPos, transform.forward, skillIndex,0);
    }

    private void GunnerRSkillEvent()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        int skillIndex = (int)KeyBind.R;

        photonView.RPC("CreateParticle", RpcTarget.All, transform.position, transform.forward, skillIndex, 0);
    }

    #endregion

    private Vector3 castBox = new Vector3(1.5f, 1, -5);
    public float castDistance = 15;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 15f);
    }
}
