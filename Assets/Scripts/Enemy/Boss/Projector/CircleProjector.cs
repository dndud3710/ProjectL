using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CircleProjector : BossProjector
{
    [SerializeField] float maintainDuration;
    public float MaintainDuration => maintainDuration;


    public void StartViewRange(bool isIn = true)
    {
        projector.material = isIn ? projectorMats[0] : projectorMats[1];

        StartCoroutine(ProjectorCoroutine());
    }

    IEnumerator ProjectorCoroutine()
    {
        yield return new WaitForSeconds(maintainDuration);

        gameObject.SetActive(false);
    }
}
