using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    public MovingSphere _player;
    public ParticleSystem particleEffect;

    void Update()
    {
        float GasAmount =_player.playerInput.z;
        transform.Rotate(Vector3.forward, GasAmount * 1000 * Time.deltaTime);

        if (GasAmount > 0 && !particleEffect.isPlaying)
        {
            particleEffect.Play();
        }
        
        if(GasAmount == 0 && particleEffect.isPlaying)
        {
            particleEffect.Stop();
        }
    }
}
