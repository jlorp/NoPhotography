using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    float motorStartVolume, motorVolumeGoal;

    public AudioSource motorSound, bonkSound, shutterSound, dingSound, explosionSound;

    void Awake()
    {
        Instance = this;
        motorStartVolume = motorSound.volume;
        motorSound.volume = 0;
    }
    void Update()
    {
        motorSound.volume = Mathf.MoveTowards(motorSound.volume, motorVolumeGoal, Time.deltaTime * .15f);
    }
    public void PlaySound(AudioSource _sound, Vector2 pitchrange, float volume = 1)
    {
        if (pitchrange != Vector2.zero)
        {
            float _pitch = UnityEngine.Random.Range(pitchrange.x, pitchrange.y);
            _sound.pitch = _pitch;
        }
        _sound.Play();
    }

    public void MotorSound(float volume)
    {
        motorVolumeGoal = Mathf.Lerp(0,motorStartVolume, volume);
    }
}
