using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/CheckTargetInfo")]
public class CheckTargetInfo : ScriptableObject
{
    public float checkDegree; // 해당 각도로 양옆을 확인함. 즉 40도일 때 총합 80도의 확인 반경을 갖음
    public float checkDistance;
    public bool customDamage;
    public float damage;
}
