using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorFormProjector : BossProjector
{
    [SerializeField] float holdingTime;

    public float HoldingTime => holdingTime;

    private void OnEnable()
    {
        StartCoroutine(SetProjectorLifeCoroutine());
    }

    IEnumerator SetProjectorLifeCoroutine()
    {
        float curWaitTime = 0;

        while(curWaitTime < holdingTime)
        {
            curWaitTime += Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
