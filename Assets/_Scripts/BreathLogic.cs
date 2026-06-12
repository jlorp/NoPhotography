using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathLogic : MonoBehaviour
{
    float breathRemaining;

    public float maxBreathCapacity;
    public MovingSphere _player;

    void Start()
    {
        breathRemaining = maxBreathCapacity;
        UIManager.Instance.breathMeter.SetMaxStat(maxBreathCapacity);
    }

    void FixedUpdate()
    {
        if(_player.Swimming)
        {
            breathRemaining -= Time.deltaTime;
            Mathf.Clamp(breathRemaining,0,maxBreathCapacity);

            UIManager.Instance.breathMeter.SetStat(breathRemaining);
        }
    }
}
