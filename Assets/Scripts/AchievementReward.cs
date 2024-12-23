using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementReward : MonoBehaviour
{
    [SerializeField] Slider rewardBar;
    float rewardValue;
    int level = 0;

    public void UpdateRewardBar()
    {
        Debug.Log("Update reward bar");
        level = GameManager.Instance.Level;
        Debug.Log("Level: " + level);
        rewardValue = (((level + 1f) % 5f) / 5f);
        Debug.Log("Reward value: " + rewardValue);
        rewardBar.value = rewardValue;
    }
}
