using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SquareProjector : BossProjector
{
    [SerializeField] float targetRatio;
    float baseRatio = 0.01f;

    [SerializeField] float completeDuration;
    float curTime = 0;

    public override void Awake()
    {
        base.Awake();

        projector.aspectRatio = baseRatio;
    }

    private void OnEnable()
    {
        ResetSetting();
    }

    public void StartViewRange()
    {
        StartCoroutine(ProjectorCoroutine());

        
    }

    IEnumerator ProjectorCoroutine()
    {
        while (curTime < completeDuration)
        {
            curTime += Time.deltaTime;
            float t = curTime / completeDuration;

            projector.aspectRatio = Mathf.Lerp(baseRatio, targetRatio, t);

            yield return null;
        }
    }

    void ResetSetting()
    {
        projector.aspectRatio = baseRatio;
        curTime = 0;
    }
}
