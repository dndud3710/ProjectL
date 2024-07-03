using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class EffectTrigger : MonoBehaviour
{
    private Rigidbody rigid;
    private Collider coll;
    private EffectController parent;

    public bool InitDisable = true;

    private void Awake()
    {
        parent = transform.parent.GetComponent<EffectController>();
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    private void Start()
    {
        coll.isTrigger = true;
        rigid.useGravity = false;
        if(InitDisable)
            DisableCollider();
    }
    private void OnTriggerEnter(Collider other)
    {
        parent.HandleTrigger(other, transform.position);
    }

    public void EnableCollider(Vector3 rigidVelocity)
    {
        rigid.velocity = rigidVelocity;
        coll.enabled = true;
    }

    public void DisableCollider()
    {
        rigid.velocity = Vector3.zero;
        coll.enabled = false;
    }
}
