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
        ResetBreath();
        UIManager.Instance.breathMeter.SetMaxStat(maxBreathCapacity);
    }

    void FixedUpdate()
    {
        UIManager.Instance.breathMeter.SetStat(breathRemaining);
        
        if(_player.Swimming && breathRemaining > 0 && !_player.body.isKinematic)
        {
            breathRemaining -= Time.deltaTime;
            breathRemaining = Mathf.Clamp( breathRemaining ,0 , maxBreathCapacity );

            if(breathRemaining == 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        _player.drowning = true;
        UIManager.Instance.BlackFade();
    }

    public void ResetBreath()
    {
        breathRemaining = maxBreathCapacity;
    }
}
