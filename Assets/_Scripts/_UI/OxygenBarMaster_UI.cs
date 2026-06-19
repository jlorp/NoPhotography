using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBarMaster_UI : MonoBehaviour
{
    public GameObject energyUnitPrefab;

    public List<BarUI> EBars;

    [SerializeField]
    RectTransform bkg;

    float breathPerMeter;
    float width;

    public float stat, maxStat;

    void Awake()
    {
        width = bkg.sizeDelta.x;
    }

    public void CorrectAllMeters()
    {
        for(int i = 0 ; i <EBars.Count; i++)
        {
            EBars[i].SetMaxStat(breathPerMeter);
            EBars[i].SetStat(breathPerMeter);
        }
    }

    void Start()
    {
        breathPerMeter = GameManager.Instance.player._breath.breathCapacityPerUnit;
        EBars[0].SetMaxStat(breathPerMeter);
    }

    public void SetMaxStat(float _maxStat)
    {
        maxStat = _maxStat;
    }

    public void SetStat(float _stat)
    {
        stat = _stat;

        float totalMeters = EBars.Count;
        float relevantmeterFloat = totalMeters * (stat/maxStat);
        int relevantMeterInt = (int)relevantmeterFloat;
        relevantMeterInt = Mathf.Clamp(relevantMeterInt, 0, EBars.Count - 1);
        relevantMeterInt = (EBars.Count-1) - relevantMeterInt;

        float emptyMeters = relevantMeterInt;
        float fullMeters = totalMeters - (emptyMeters + 1);

        float relevantMeterStat = stat - (fullMeters * breathPerMeter);
        EBars[relevantMeterInt].SetStat(relevantMeterStat);

        //fill meters
        for (int i = relevantMeterInt + 1 ; i <= fullMeters; i++)
        {
            if(i > EBars.Count) return;
            EBars[i].SetStat(breathPerMeter);
        }
        //empty meters
        for (int i = relevantMeterInt - 1 ; i >= 0; i--)
        {
            if(i < 0) return;
            EBars[i].SetStat(0);
        }
    }

    public void ExpandMeter()
    {
        GameObject newMeter = Instantiate(energyUnitPrefab, Vector3.zero, Quaternion.identity);
        newMeter.transform.SetParent(this.transform);
        newMeter.transform.localScale = new Vector3(1,1,1);

        float newMeterHeight = newMeter.GetComponent<RectTransform>().rect.height;
        float newHeight = bkg.sizeDelta.y + newMeterHeight;

        bkg.sizeDelta = new Vector2(width, newHeight);

        BarUI newBar = newMeter.GetComponent<BarUI>();
        newBar.SetMaxStat(breathPerMeter);
        EBars.Add(newBar);

        SetMaxStat(GameManager.Instance.player._breath.maxBreathCapacity);
    }
}
