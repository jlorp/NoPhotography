using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    public float stat, maxStat;

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

    public void SetStat(float _stat)
    {
        stat = _stat;
        float newHeight = ( stat / maxStat) * height;
        statBar.sizeDelta = new Vector2(width, newHeight);
    }
}
