using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementReward : MonoBehaviour
{
    [SerializeField] Slider rewardBar;
    [SerializeField] Button chestbutton;
    Animation anim;
    float rewardValue;
    int level = 0;
    int rewardInterval;

    private void Start()
    {
        anim = chestbutton.GetComponent<Animation>();
    }

    private void Update()
    {
        if (rewardValue >= 1)
        {
            chestbutton.interactable = true;
            anim.Play();
        }
        else
        {
            chestbutton.interactable = false;
            anim.Stop();
        }
    }
    public void UpdateRewardBar()
    {
        Debug.Log("Update reward bar");
        level = GameManager.Instance.Level;
        Debug.Log("Level: " + level);
        if (level <= 5) rewardInterval = 5;
        else rewardInterval = 10;    
        rewardValue = (float) (level % rewardInterval) / rewardInterval;
        if (rewardValue == 0 && level != 0)
        {
            rewardValue = 1;
        }
        Debug.Log("Reward value: " + rewardValue);
        rewardBar.value = rewardValue;
    }

    public bool IsRewardReady()
    {
        return rewardValue >= 1;
    }

    public void ResetReward()
    {
        rewardValue = 0.001f;
        rewardBar.value = rewardValue;
    }
}
