using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    public MovingSphere _player;
    void Update()
    {
        float GasAmount =_player.playerInput.z;
        transform.Rotate(Vector3.forward, GasAmount * 1000 * Time.deltaTime);
    }
}
