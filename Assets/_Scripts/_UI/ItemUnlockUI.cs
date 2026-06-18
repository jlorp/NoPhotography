using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUnlockUI : MonoBehaviour
{
    public Animation _animaiton;
    public GameObject _AnimParent;

    bool closeWindowAvailable = false;

    public TextMeshProUGUI rewardNameText, rewardDescriptionText;
    public Image rewardImage;


    public void PlayAnimation(RewardData _data)
    {
        rewardNameText.text = _data.rewardName;
        rewardDescriptionText.text = _data.rewardDescription;
        rewardImage.sprite = _data.rewardImage;
        GameManager.Instance.NextUnlock = _data.ItemCode;

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
