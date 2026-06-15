using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLogic : MonoBehaviour
{
    public Transform _player; 
    public Rigidbody rb;
    public MovingSphere _playerLogic;

    public Transform _playerParent;
    public Animation _animaiton;

    bool startState = true;

    public static ClawLogic Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(startState && Input.GetButtonDown("Jump"))
        {
            DropClaw();
            startState = false;
        }
    }

    public void DropClaw()
    {
        _animaiton.Play("SubDrop");
        _player.transform.localPosition = Vector3.zero;
    }

    void ReturnGoals()
    {
        GoalManager.Instance.ReturnGoals();
        UIManager.Instance.ResetPlayer();
    }

    void UnparentSub()
    {
        _player.SetParent(null);
        rb.isKinematic = false;
        _playerLogic.isHeldByClaw = false;
    }

    void ParentSub()
    {
        _player.SetParent(_playerParent);
        _player.localPosition = Vector3.zero;
        rb.isKinematic = true;
        _playerLogic.isHeldByClaw = true;
    }

    void ActivateCamera()
    {
        OrbitCamera.Instance.LerpToActivation();
        UIManager.Instance.CloseStartMenu();
    }
}
