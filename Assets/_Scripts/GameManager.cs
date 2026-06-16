using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Levelup Stuff
    public float expPerLevel = 100;
    public float expInMeter;
    public int level;

    //dependencies
    public MovingSphere player;

    [HideInInspector]
    public float expToAdd;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        expInMeter = 0;
        UIManager.Instance.expMeter.SetMaxStat(expPerLevel);
        UIManager.Instance.expMeter.SetStat(0);
        UpgradeMaxBreath();
        player._breath.ResetBreath();
    }

    public void LevelUp()
    {
        expInMeter = 0;
        level +=1;
        UIManager.Instance.expMeter.SetStat(expInMeter);
        UnlockItem();

        if (expToAdd != 0)
        {
           AddExperience(0);
        }
    }

    public void UnlockItem()
    {
        UpgradeMaxBreath();
    }

    public void UpgradeMaxBreath()
    {
        player._breath.maxBreathCapacity += player._breath.breathCapacityPerUnit;
        UIManager.Instance.breathMeter.ExpandMeter();
    }

    public void AddExperience(float _expToadd)
    {
        UIManager.Instance.expParent.SetActive(true);

        expToAdd += _expToadd;
        float expToNextLevel = expPerLevel - expInMeter;
      
        if (expToAdd >= expToNextLevel)
        {
            UIManager.Instance.expMeter.StartLerpStat(expToNextLevel, 2, true);
            expToAdd -= expToNextLevel;
            expInMeter += expToNextLevel;
        }
        else
        {
            UIManager.Instance.expMeter.StartLerpStat(expToAdd, 2,true);
            expToAdd -= expToAdd;
            expInMeter += expToAdd;
        }
    }

    public void OnDoneAddingExperience()
    {
        UIManager.Instance.ResetPlayer();
        UIManager.Instance.expParent.SetActive(false);
    }
}
