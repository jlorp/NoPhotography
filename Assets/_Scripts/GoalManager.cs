using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public List<GoalData> Goals;
    public List<GoalData> FinishedGoals;
    public static GoalManager Instance;

    public GameObject emptyGoalPrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ClearGoalsList();
        PopulateGoalsList();
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
            CompleteGoal(_finishedGoal);
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

    void CompleteGoal(GoalData finishedGoal)
    {
        Goals.Remove(finishedGoal);
        FinishedGoals.Add(finishedGoal);
        
        Transform goalParent = UIManager.Instance.goalListParent.transform;

        foreach(Transform child in goalParent)
        {
            if (child.TryGetComponent<GoalUI>(out GoalUI _ui))
            {
                if (_ui.relevantGoal == finishedGoal)
                {
                    _ui.CompleteGoal();
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
