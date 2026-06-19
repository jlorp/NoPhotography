using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathLogic : MonoBehaviour
{
    float breathRemaining;

    public float maxBreathCapacity;
    [HideInInspector]
    public float breathCapacityPerUnit;

    public MovingSphere _player;
    public Animation _animation;
    public ParticleSystem _explosion;

    void Awake()
    {
        breathCapacityPerUnit = maxBreathCapacity;
    }

    void Start()
    {
        ResetBreath();
    }

    void FixedUpdate()
    {
        UIManager.Instance.breathMeter.SetStat(breathRemaining);
        
        if(_player.Swimming && breathRemaining > 0 && _player.playerInput.z > 0 && !_player.body.isKinematic && !_player.CameraOpen)
        {
            breathRemaining -= Time.deltaTime * _player.playerInput.z;
            breathRemaining = Mathf.Clamp( breathRemaining ,0 , maxBreathCapacity );

            if(breathRemaining == 0)
            {
                Die();
            }
        }
    }

    public void RemoveBreath(float amount)
    {
        breathRemaining -= amount;
        if(breathRemaining <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(_player.drowning) return;

        UIManager.Instance.CloseCamera();
        _explosion.Play();
        _player.submarine.gameObject.SetActive(false);
        _player.drowning = true;
        UIManager.Instance.BlackFade();
        GoalManager.Instance.FailGoals();

        SoundManager.Instance.PlaySound(SoundManager.Instance.explosionSound, new Vector2(0.95f,1.5f));
    }

    public void ResetBreath()
    {
        breathRemaining = maxBreathCapacity;
        _player.drowning = false;
    }

    void Bonk(Collision collision)
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.bonkSound, new Vector2(0.95f,1.5f));

        _animation.Play("Player_WiggleDamage", PlayMode.StopAll); 
        Vector3 hitNormal = collision.contacts[0].normal;
        float forceMagnitude = _player.body.velocity.magnitude;
        forceMagnitude = Mathf.Clamp(forceMagnitude, 0.75f , 3);
        _player.LockInput();
        _player.body.AddForce( hitNormal * forceMagnitude * 2, ForceMode.Impulse);
        breathRemaining -= forceMagnitude * 1.33f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Bonk(collision);

        if(breathRemaining <= 0)
        {
            Die();
        }
    }
}
