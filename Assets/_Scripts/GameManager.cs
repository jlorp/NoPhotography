using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Levelup Stuff
    public float expPerLevel = 100;
    public float expInMeter;
    public int level = 1;

    //dependencies
    public MovingSphere player;

    [HideInInspector]
    public float expToAdd;
    public int NextUnlock;
    public List<RewardData> LevelUnlocks;

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

        UnlockItem(NextUnlock);

        
        AddExperience(0);
        
    }

    public void UnlockItem(int toUnlock)
    {
        if(toUnlock == 1)
        {
            UpgradeMaxBreath();
        }
        if(toUnlock == 2)
        {
            UnlockBoost();
        }
    }

    void UnlockBoost()
    {
        player.boostUnlocked = true;
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
        
        if (expToAdd  >= expToNextLevel)
        {
            UIManager.Instance.expMeter.StartLerpStat(expToNextLevel, 2, true, true);
            expInMeter += expToNextLevel;
            expToAdd -= expToNextLevel;
        }
        else
        {
            UIManager.Instance.expMeter.StartLerpStat(expToAdd, 2,true , false);
            expInMeter += expToAdd;
            expToAdd -= expToAdd;
        }
    }

    public void OnDoneAddingExperience()
    {
        UIManager.Instance.ResetPlayer();
        UIManager.Instance.expParent.SetActive(false);
    }
}
