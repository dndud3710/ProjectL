using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin perlinNoise;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        perlinNoise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        ResetIntensity();
    }

    public void ShakeCamera(float intensity, float shakeTime)
    {
        perlinNoise.m_AmplitudeGain = intensity;
        StartCoroutine(WaitTime(shakeTime));
    }

    IEnumerator WaitTime(float shakeTime)
    {
        yield return new WaitForSeconds(shakeTime);
        ResetIntensity();
    }

    private void ResetIntensity()
    {
        perlinNoise.m_AmplitudeGain = 0f;
    }

    public void SetFollower(Transform follower)
    {
        SetPriority(20);
        virtualCam.Follow = follower;
        virtualCam.LookAt = follower;
    }

    public void SetPriority(int priority)
    {
        virtualCam.Priority = priority;
    }
}
