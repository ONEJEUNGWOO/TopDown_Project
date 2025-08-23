using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;
    private float shakeTimeReamining;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (shakeTimeReamining > 0)
        {
            shakeTimeReamining -= Time.deltaTime;
            if (shakeTimeReamining <= 0f)
            {
                StopShake();
            }
        }
    }

    public void ShakeCamera(float duration, float amplitude, float frequency)
    {
        if (shakeTimeReamining > duration) return;

        shakeTimeReamining = duration;

        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
    }

    public void StopShake()
    {
        shakeTimeReamining = 0;
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }
}
