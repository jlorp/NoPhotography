using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalUI : MonoBehaviour
{
    public TextMeshProUGUI goalText;

    public void OnCreateGoalUI(GoalData goal)
    {
        UpdateGoalText(goal.GoalName);
    }

    public void CompleteGoal()
    {

    }

    void UpdateGoalText(string _text)
    {
        goalText.text = _text;
    }
}
