using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationListener : MonoBehaviour
{
    void UnlockPlayerInput()
    {
        if(GameManager.Instance.player.drowning) return;
        GameManager.Instance.player.UnlockInput();
    }
}
