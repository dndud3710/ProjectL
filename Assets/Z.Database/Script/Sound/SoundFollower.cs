using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFollower : MonoBehaviour
{
    Transform target;
    bool targetOn;
    private void OnEnable()
    {
        
    }
    void Update()
    {
        if(targetOn)
            transform.position = target.position;
    }
    private void OnDisable()
    {
        target = null;
        targetOn = false;
    }
    public void setTarget(Transform tf)
    {
        targetOn = true;
        target = tf;
    }
}
