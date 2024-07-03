using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/CheckTargetInfo")]
public class CheckTargetInfo : ScriptableObject
{
    public float checkDegree; // �ش� ������ �翷�� Ȯ����. �� 40���� �� ���� 80���� Ȯ�� �ݰ��� ����
    public float checkDistance;
    public bool customDamage;
    public float damage;
}
