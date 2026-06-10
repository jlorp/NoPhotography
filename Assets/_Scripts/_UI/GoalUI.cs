using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoalUI : MonoBehaviour
{
    public TextMeshProUGUI goalText;
    public Image background;
    public GoalData relevantGoal;

    public void OnCreateGoalUI(GoalData goal)
    {
        UpdateGoalText(goal.GoalName);
        UncheckGoal();
        relevantGoal = goal;
    }

    public void CompleteGoal()
    {
        background.enabled = true;
    }

    void UncheckGoal()
    {
        background.enabled = false;
    }

    void UpdateGoalText(string _text)
    {
        goalText.text = _text;
    }
}
