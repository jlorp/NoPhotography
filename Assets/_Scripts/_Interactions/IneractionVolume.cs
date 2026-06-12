using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IneractionVolume : MonoBehaviour
{
    public bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovingSphere>(out MovingSphere _player))
        {
            isActive = true;
            UIManager.Instance.AddInteractPrompt("Jump to Get in Boat");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovingSphere>(out MovingSphere _player))
        {
            isActive = false;
            UIManager.Instance.RemoveInteractPrompt();
        }
    }

     public void Update()
    {
        if(Input.GetButtonDown("Jump") && isActive)
        {
            UIManager.Instance.BlackFade();
        }
    }
}
