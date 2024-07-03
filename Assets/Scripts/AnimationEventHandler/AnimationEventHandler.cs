using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviourPun
{
    Animator animator;
    DeathKnight Owner;
    MotionTrailer motionTrailer;

    [SerializeField] float alertWaitTime;

    
    [Header("범위 표시 알림")]
    [SerializeField] CircleProjector circleProjector;
    [SerializeField] SectorFormProjector sectorProjector;
    [SerializeField] SquareProjector squareProjector;
    

    [Header("Animation Event의 매개변수로 받지 않고 직접 할당해주는 SO")]
    [SerializeField] BossParticleData circleInData;
    [SerializeField] BossParticleData circleOutData;

    Vector3 lastPos;

    bool IsContacted; // 접촉했으면 콤보를 이어나가고 아니면 중단
    bool isIn; // 안밖 패턴의 여부를 Random으로 설정. OnAlert와 OnAlertAttack에서 모두 쓰임

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Owner = GetComponent<DeathKnight>();
        motionTrailer = GetComponent<MotionTrailer>();
    }
    
    public void OnEquipWeapon(int weaponType)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ChangeWeaponRPC(weaponType);

        Owner.SetWeaponTransformRPC(true);
    }

    public void OnUnequipWeapon()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.SetWeaponTransformRPC(false);

        Owner.ChangeWeaponRPC(-1); // 보스의 무기 자체를 null로 만들어주고자 할 때, RPC로는 객체를 넘겨줄 수 없으므로 일부러 범위를 벗어나게 하여 전달
    }
    public void OnAttack()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ChangeAnimRPC("Attack");
    }

    public void OnAttackContinue()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        if (IsContacted == true)
        {
            Owner.ChangeAnimRPC("Attack");
            
        }
    }

    public void OnCheckCollide(int param)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        bool isChecking = (param == 0) ? true : false;
        Owner.CurWeapon.SetWeaponBools(BossWeapon.Booleans.IsChecking, isChecking);

        print($"현재 공격 판정 여부 : {isChecking}");

        if(param < 0 || param > 1)
        {
            Debug.Log("오류 발생. 공격 애니메이션의 Animation Event를 확인");
        }        
    }

    public void OnReadyToTeleport(float waitTime)
    {
        // if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ReadyToTeleportRPC(waitTime);
    }

    public void OnMakeAttackParticle(BossParticleData data) // 같은 파티클이더라도 데미지를 다르게 설정할 수 있기 때문에 여기서 data를 기반으로 설정
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        var particle = Resources.Load<BossInstParticle>(data.particlePath);
        var type = particle.ParticleType;

        switch(type)
        {
            case ParticleType.Install:
                PhotonNetwork.Instantiate(data.particlePath, Owner.transform.position + (transform.forward * data.createDistance), Owner.transform.rotation);
                break;

            case ParticleType.WeaponExplosion:
                Transform detectTrans = Owner.CurWeapon.DetectPoints[0];
                LayerMask ground = LayerMask.GetMask("Ground");
                RaycastHit hit;
                if(Physics.Raycast(detectTrans.position + new Vector3(0, 10 , 0), Vector3.down, out hit, 50, ground))
                {
                    Vector3 targetPos = hit.point;
                    PhotonNetwork.Instantiate(data.particlePath, targetPos, transform.rotation);
                }
                break;

            case ParticleType.PatternExplosion:
                PhotonNetwork.Instantiate(data.particlePath, lastPos + new Vector3(0, 2.5f, 0), Quaternion.identity);
                break;
        }
    }

    public void OnMakeMotionTrail(int isOn)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        switch (isOn)
        {
            case 0:
                motionTrailer.StartMotionTrailRPC();
                break;
            case 1:
                motionTrailer.FinishMotionTrailRPC();
                break;
        }
    }

    public void OnCircleAlert()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        int value = Random.Range(0, 2);
        isIn = (value == 0);

        photonView.RPC("AlertRPC", RpcTarget.All, isIn);
    }

    #region CircleAttackMethods

    [PunRPC]
    public void AlertRPC(bool isIn)
    {
        animator.speed = 0f;

        lastPos = transform.position;

        StartCoroutine(CircleAlertCoroutine(isIn));
    }

    IEnumerator CircleAlertCoroutine(bool isIn)
    {
        yield return new WaitForSeconds(alertWaitTime);

        circleProjector.gameObject.SetActive(true);

        circleProjector.StartViewRange(isIn);

        // 원래는 cirleProjector의 범위 표현이 끝나면 그에 맞춰서 Event를 Invoke 하려고 했으나 Projector 객체는 평소에 꺼져있고 찰나의 순간에만 켜지기 때문에 Event 구독이 무의미함
        yield return new WaitForSeconds(circleProjector.MaintainDuration);

        animator.speed = 1f;
    }
    #endregion

    public void OnCircleAlertAttack()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        // bossAttack.AlertAttack(isIn);
        OnMakeAttackParticle(isIn ? circleInData : circleOutData);
    }

    public void OnStartCharge()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ChangeAnimSpeedRPC(0.25f);
        photonView.RPC("SetSquareProjectorRPC", RpcTarget.All, true);        
    }

    public void OnFinishCharge()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ChangeAnimSpeedRPC(Owner.BaseSpeed);
        photonView.RPC("SetSquareProjectorRPC", RpcTarget.All, false);
    }

    #region SquareAlertMethods

    [PunRPC]
    public void SetSquareProjectorRPC(bool isStart)
    {
        if (isStart)
        {
            squareProjector.gameObject.SetActive(true);
            squareProjector.StartViewRange();
        }
        else
        {
            squareProjector.gameObject.SetActive(false);
        }
    }

    #endregion

    #region BossOwnParticle
    public void OnFinishSpiral()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.DeathSpiral.FinishParticleRPC();
    }


    #endregion

    public void OnChangeSkin()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.SkinChangeRPC();
    }

    public void OnAlertSectorForm()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ChangeAnimSpeedRPC(0f);
        AlertSectorFormRPC();
    }

    #region AlertSectorForm
    [PunRPC]
    public void AlertSectorForm()
    {
        StartCoroutine(AlertSectorFormCoroutine());
    }

    public void AlertSectorFormRPC()
    {
        photonView.RPC("AlertSectorForm", RpcTarget.All);
    }

    IEnumerator AlertSectorFormCoroutine()
    {
        float waitTime = Owner.SectorProejctor.HoldingTime;
        float tempAnimSpeed = 2f;

        Owner.SectorProejctor.gameObject.SetActive(true);

        yield return new WaitForSeconds(waitTime);

        Owner.ChangeAnimSpeed(tempAnimSpeed);
    }

    #endregion


    public void OnSettingShield(int isOn)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        photonView.RPC("SettingShield", RpcTarget.All, isOn);
    }

    [PunRPC]
    public void SettingShield(int isOn)
    {
        bool setOn = (isOn == 0);
        Owner.InfernoShield.SetActive(setOn);
        Owner.SetStateBools(DeathKnight.StateBools.IsDefensing, setOn);
    }

    public void OnStartPatternCasting()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Owner.ChangeAnimSpeedRPC(0f);
        Owner.CurWeapon.gameObject.SetActive(false);

        GameObject obj = PhotonNetwork.Instantiate("Prefabs/Enemy/Pattern/FirstPattern", transform.position, Quaternion.identity);
        obj.GetComponent<FirstPatternAttack>().OnFinishPattern.AddListener(() =>
        {
            Owner.ChangeAnimSpeedRPC(Owner.BaseSpeed);
            Owner.CurWeapon.gameObject.SetActive(true);
        });
    }

    public void OnAttackTarget(CheckTargetInfo info)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        float detectDegree = info.checkDegree;
        float attackRange = info.checkDistance;

        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, Owner.TargetMask);


        if(targets.Length > 0)
        {
            Vector3 standardDir = transform.forward.normalized;

            foreach (var target in targets)
            {
                Vector3 targetDir = (target.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(targetDir, standardDir);
                float comparison = Mathf.Cos(detectDegree * Mathf.Deg2Rad);

                if(dot > comparison)
                {
                    if (target.TryGetComponent(out IEffectable effectable))
                    {
                        effectable.TakeDamageRPC(info.customDamage? info.damage : Owner.CurWeapon.MeleeDamage, Owner.transform.position, Owner.photonView.ViewID);
                    }
                }
            }
        }
    }    
}
