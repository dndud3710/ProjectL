using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csObjectMake2Hobin : MonoBehaviour
{

    public EffectController effectController;
    public EffectChanger effectChanger;

    public float m_object_MakeDelay;
    public float m_object_MakeRange;
    public int m_object_MakeCount;
    public float m_startDelay;
    float m_Time;
    float m_object_MakeRange2;
    float m_count;
    public bool isX;
    public bool isY;
    public bool isZ;
    public bool isMinusZ;

    void Start()
    {
        StartCoroutine(CreateObjectsWithDelay());
    }

    IEnumerator CreateObjectsWithDelay()
    {
        yield return new WaitForSeconds(m_startDelay); // 초기 지연

        while (m_count < m_object_MakeCount)
        {
            Vector3 addedPosition = new Vector3(0, 0, 0);
            if (isX)
                addedPosition += new Vector3(m_object_MakeRange2, 0, 0);
            if (isY)
                addedPosition += new Vector3(0, m_object_MakeRange2, 0);
            if (isZ)
            {
                float zOffset = isMinusZ ? -m_object_MakeRange2 : m_object_MakeRange2;
                addedPosition += transform.forward * zOffset;
            }

            EffectChanger ob = Instantiate(effectChanger, transform.position + addedPosition, transform.rotation);
            ob.transform.parent = this.transform;

            var effectData = effectController.effectData;
            if (effectData != null)
            {
                ob.SetParticleColor(effectData);
            }

            Destroy(ob.gameObject, 6f); // 생성된 객체 파괴
            m_object_MakeRange2 += m_object_MakeRange;
            m_count++;

            yield return new WaitForSeconds(m_object_MakeDelay); // 다음 생성까지 대기
        }
    }
}
