using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentVolume : MonoBehaviour
{
    public MovingSphere player;
    public Vector3 forceToAdd;

    void FixedUpdate()
    {
        if(player)
        {
            Vector3 localForce = (this.transform.forward * forceToAdd.z) + (this.transform.right* forceToAdd.x) + (this.transform.up * forceToAdd.y);
            player.body.AddForce(localForce);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovingSphere>(out MovingSphere _player))
        {
            player = _player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovingSphere>(out MovingSphere _player))
        {
            player = null;
        }
    }
}
