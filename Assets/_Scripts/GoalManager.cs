using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public List<GoalData> Goals;
    public List<GoalData> HeldGoals;
    public List<GoalData> FinishedGoals;
    public static GoalManager Instance;

    public GameObject emptyGoalPrefab;

    float expToAdd = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ClearGoalsList();
        PopulateGoalsList();
    }

    public void ReturnGoals()
    {
        List <GoalData> _localHeldGoals = HeldGoals;

        foreach(GoalData goal in _localHeldGoals)
        {
            CompleteGoal(goal);
        }

        GameManager.Instance.AddExperience(expToAdd);
        expToAdd=0;
        
        HeldGoals = new List<GoalData>();
    }

    public void FailGoals()
    {
        List <GoalData> _localHeldGoals = HeldGoals;

        foreach(GoalData goal in _localHeldGoals)
        {
            FailGoal(goal);
        }

        HeldGoals = new List<GoalData>();
    }

    public void CheckAgainstGoals(List<ItemData> photoContents)
    {
        List<GoalData> finishedGoals = new List<GoalData>();

        foreach(GoalData goal in Goals)
        {
            if(CompareLists(goal.RequiredPhotoContents, photoContents))
            {
                finishedGoals.Add(goal);
            }
        }

        foreach(GoalData _finishedGoal in finishedGoals)
        {
            HoldGoal(_finishedGoal);
        }
    }

    bool CompareLists(List<ItemData> requiredContents, List<ItemData> photoContents)
    {
        int requiredItems=0;
        foreach(ItemData item in requiredContents)
        {
            if(photoContents.Contains(item))
            {
                photoContents.Remove(item);
                requiredItems +=1;
            }
        }

        if(requiredItems == requiredContents.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void FailGoal(GoalData failedGoal)
    {
        Goals.Add(failedGoal);
        AdjustGoal(failedGoal, 0);
        UIManager.Instance.Popup("Goal Lost: Did not return to Boat", 0, 5);
    }

    void HoldGoal(GoalData heldGoal)
    {
        Goals.Remove(heldGoal);
        HeldGoals.Add(heldGoal);
        AdjustGoal(heldGoal,1);
        UIManager.Instance.Popup("Goal Held: " + heldGoal.GoalName, 1, 3);
    }

    void CompleteGoal(GoalData finishedGoal)
    {
        if(FinishedGoals.Contains(finishedGoal)) return;

        FinishedGoals.Add(finishedGoal);
        
        UIManager.Instance.Popup("Goals Redeemed", 2, 4);
        AdjustGoal(finishedGoal, 2);
    }

    void AdjustGoal(GoalData _goal, int _status)
    {
        //int 0 = incomplete
        //int 1 = held
        //int 2 = complete

        Transform goalParent = UIManager.Instance.goalListParent.transform;

        foreach(Transform child in goalParent)
        {
            if (child.TryGetComponent<GoalUI>(out GoalUI _ui))
            {
                if (_ui.relevantGoal == _goal)
                {
                    if (_status == 2)
                    {
                        _ui.CompleteGoal();
                        //Wallet.Instance.AddCash(_goal.cashReward);   
                        expToAdd += _goal.cashReward;
                    }
                    else if (_status == 1)
                    {
                        _ui.MarkGoalHeld();
                    }
                    if(_status == 0)
                    {
                        _ui.UncheckGoal();
                    }
                }
            }
        }
    }

    void ClearGoalsList()
    {
        GameObject goalParent = UIManager.Instance.goalListParent;
        int childCount = goalParent.transform.childCount;

        for(int i = 0; i < childCount; i++)
        {
            Destroy(goalParent.transform.GetChild(0).gameObject);
        }
    }

    void PopulateGoalsList()
    {
        for(int i = 0; i < Goals.Count; i++)
        {
            GameObject newGoal = Instantiate(emptyGoalPrefab, Vector3.zero, Quaternion.identity);
            newGoal.GetComponent<GoalUI>().OnCreateGoalUI(Goals[i]);
            newGoal.transform.SetParent(UIManager.Instance.goalListParent.transform);
            newGoal.transform.localScale = new Vector3(1,1,1);
        }
    }
}
