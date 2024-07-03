using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossWeapon : MonoBehaviour
{
    public enum Booleans
    {
        IsChecking, // ���� ���� ���� ���� ������ ���� ����
        IsSmashing, // Smashing�� ���� ��ü�� ȸ������ �ʵ��� ����
    }

    protected DeathKnight Owner;

    [SerializeField] WeaponType type;
    private Dictionary<Booleans, bool> boolDic;

    [SerializeField] Transform[] detectPoints;

    [SerializeField] float meleeDamage;

    LayerMask targetLayer;

    public WeaponType Type => type;
    public Transform[] DetectPoints => detectPoints;
    public float MeleeDamage => meleeDamage;

    private void Awake()
    {
        boolDic = new Dictionary<Booleans, bool>() { { Booleans.IsChecking, false } , { Booleans.IsSmashing, false} };
        targetLayer = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        Owner = transform.root.GetComponent<DeathKnight>();
        /*
        Owner.WeaponChangedEvent.AddListener((Type) =>
        {
            if(Type != this.type)
            {
                transform.localPosition = Vector3.zero;
            }
        });
        */
    }


    public void CheckAndAttackTarget(in HashSet<GameObject> hashSet, in Vector3[] PrevPos) // TODO : Photon RPC �Լ��� �ۼ� / �浹 ���� ���ο� �������� �޴� �Լ� �и��� ��
    {
       if (PrevPos.Length <= 0)
        {
            for (int i = 0; i < detectPoints.Length; i++)
            {
                PrevPos[i] = detectPoints[i].position;
            }
        }

        for(int i = 0; i < detectPoints.Length; i++)
        {
            float dist = Vector3.Distance(PrevPos[i], detectPoints[i].position);
            Vector3 dir = (PrevPos[i] - detectPoints[i].position).normalized;

            RaycastHit[] hits = Physics.RaycastAll(detectPoints[i].position, dir, dist, targetLayer);

            if(hits.Length > 0)
            {
                foreach(var hit in hits)
                {
                    if (hashSet.Contains(hit.collider.gameObject)) continue;

                    Debug.Log($"{hit.collider.name} ������ ����");
                    hashSet.Add(hit.collider.gameObject);

                    IHittable target; 
                    hit.transform.TryGetComponent<IHittable>(out target);
                    target?.TakeDamageRPC(meleeDamage, Owner.transform.position, Owner.photonView.ViewID);


                }
            }
        }
    }

    public bool GetWeaponBools(Booleans param)
    {
        return boolDic[param];
    }

    public void SetWeaponBools(Booleans state, bool boolean)
    {
        boolDic[state] = boolean;
    }
}
