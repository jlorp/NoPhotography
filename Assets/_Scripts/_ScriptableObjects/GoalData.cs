using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GoalData", order = 1)]
public class GoalData : ScriptableObject
{
    public string GoalName;
    public string GoalDescription;
}