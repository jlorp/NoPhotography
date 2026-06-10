using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public List<GoalData> Goals;
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
        }
    }
}
