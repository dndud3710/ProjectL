using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csMoveHobin : MonoBehaviour {

    public float MoveSpeed;
    float m_Time;

	void Update ()
    {
        m_Time += Time.deltaTime;
        var dir = transform.InverseTransformDirection(transform.forward);
        transform.Translate(dir * Time.deltaTime * MoveSpeed);
    }
}
