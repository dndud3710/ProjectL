using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayObjectMakeHobin : _ObjectMakeBase
{

    public float m_startDelay;
    float m_Time;
    bool isMade = false;
    public EffectController effectController;

    void Start()
    {
        m_Time = Time.time;
        StartCoroutine(DelayedMake());
    }

    IEnumerator DelayedMake()
    {
        yield return new WaitForSeconds(m_startDelay);  // 지정된 지연 시간 대기

        if (!isMade)  // 객체가 아직 생성되지 않았는지 확인
        {
            isMade = true;
            var m_obj = Instantiate(m_makeObjs[0], transform.position, transform.rotation);
            m_obj.transform.parent = this.transform;

            if (m_obj.TryGetComponent(out EffectChanger changer))
            {
                changer.SetParticleColor(effectController.effectData);
            }

            if (m_movePos)
            {
                MoveToObject m_script = m_obj.GetComponent<MoveToObject>();
                if (m_script)
                {
                    m_script.m_movePos = m_movePos;
                }
            }
        }
    }
}
