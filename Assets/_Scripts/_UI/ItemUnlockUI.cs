using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnlockUI : MonoBehaviour
{
    public Animation _animaiton;
    public GameObject _AnimParent;

    bool closeWindowAvailable = false;

    public void PlayAnimation()
    {
        _animaiton.Play("Item Reveal");
        _AnimParent.SetActive(true);
    }

    public void ClosePanel()
    {
        GameManager.Instance.LevelUp();
        
        _AnimParent.SetActive(false);
    }
    void Update()
    {
        if(Input.GetButtonDown("Jump") && closeWindowAvailable)
        {
            ClosePanel();
        }   
    }

    void AnimationComplete()
    {
        closeWindowAvailable = true;
    }
}
