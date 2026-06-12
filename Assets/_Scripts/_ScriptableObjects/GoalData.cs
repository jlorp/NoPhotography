using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GoalData", order = 1)]
public class GoalData : ScriptableObject
{
    public string GoalName;
    public List<ItemData> RequiredPhotoContents;
    public float cashReward;
}