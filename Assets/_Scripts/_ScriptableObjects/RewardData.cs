using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RewardData", order = 1)]
public class RewardData : ScriptableObject
{
    public string rewardName;
    public string rewardDescription;
    public Sprite rewardImage;
    public int ItemCode;
}
