using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLogic : MonoBehaviour
{
    public Transform _player; 
    public Rigidbody rb;

    public Transform _playerParent;
    public Animation _animaiton;

    bool startState = true;


    void Update()
    {
        if(startState && Input.GetButtonDown("Jump"))
        {
            DropClaw();
            startState = false;
        }
    }

    void DropClaw()
    {
        _animaiton.Play("SubDrop");
    }

    void UnparentSub()
    {
        _player.SetParent(null);
        rb.isKinematic = false;
    }

    void ParentSub()
    {
        _player.SetParent(_playerParent);
        _player.localPosition = Vector3.zero;
        rb.isKinematic = true;
    }

    void ActivateCamera()
    {
        OrbitCamera.Instance.LerpToActivation();
        UIManager.Instance.CloseStartMenu();
    }
}
