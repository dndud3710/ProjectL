using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossProjector : MonoBehaviour
{
    protected Projector projector;

    [SerializeField] protected Material[] projectorMats;

    public virtual void Awake()
    {
        projector = GetComponent<Projector>();

        gameObject.SetActive(false);
    }

}
