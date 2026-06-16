using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    public float stat, maxStat;
    public bool horizontal;

    float height, width;

    [SerializeField]
    private RectTransform statBar;

    void Awake()
    {
        height = statBar.sizeDelta.y;
        width = statBar.sizeDelta.x;
    }

    public void SetMaxStat(float _maxStat)
    {
        maxStat = _maxStat;
    }

    public void StartLerpStat(float amountToAdd, float lerpSpeed, bool isExpBar)
    {
        if(amountToAdd == 0)
        {
            GameManager.Instance.OnDoneAddingExperience();
            return;
        }

        StartCoroutine(LerpBar(amountToAdd, lerpSpeed, isExpBar));
    }

    IEnumerator LerpBar(float amountToadd, float lerpSpeed, bool isExpBar)
    {
        float time = 0f;
        float duration = (amountToadd/maxStat) * lerpSpeed;
        float startingStat = stat;

        while (time < duration) 
        {
            time += Time.deltaTime;
            SetStat(startingStat + ((time/duration)*amountToadd));
            yield return null; 
        }
        SetStat(startingStat + amountToadd);


        if(isExpBar)
        {
            if(GameManager.Instance.expInMeter >= GameManager.Instance.expPerLevel)
            {
                UIManager.Instance._itemUI.PlayAnimation();
            }
            else
            {
                GameManager.Instance.OnDoneAddingExperience();
            }
        }
    }

    public void SetStat(float _stat)
    {
        stat = _stat;

        if(horizontal)
        {
            float newWidth = ( stat / maxStat) * width;
            statBar.sizeDelta = new Vector2(newWidth, height);
        }
        else
        {
            float newHeight = ( stat / maxStat) * height;
            statBar.sizeDelta = new Vector2(width, newHeight);
        }
    }
}
