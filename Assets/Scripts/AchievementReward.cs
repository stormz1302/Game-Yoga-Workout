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
    bool rewardReady = true;
    private const string RewardValueKey = "RewardValue";
    private const string RewardReady = "RewardReady";
    private void Start()
    {
        anim = chestbutton.GetComponent<Animation>();

        // Load the saved state
        LoadRewardState();
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
        LoadRewardState();
        Debug.Log("Update reward bar");
        level = GameManager.Instance.Level;
        Debug.Log("Level: " + level);

        if (level <= 5)
        {
            rewardInterval = 5;
            rewardValue = (float)level / 5;
        }
        else
        {
            rewardInterval = 10;
            rewardValue = (float)((level - 5) % rewardInterval) / rewardInterval;
        }

        if (rewardValue == 0 && level != 0)
        {
            rewardValue = 1;
        }
        if (rewardValue >= 1 && !rewardReady)
        {
            rewardBar.value = PlayerPrefs.GetFloat(RewardValueKey, 0.001f);
            rewardReady = true;
            return;
        }
        Debug.Log("Reward value: " + rewardValue);
        rewardBar.value = rewardValue;

        // Save the current state
        SaveRewardState();
    }

    public bool IsRewardReady()
    {
        return rewardValue >= 1;
    }

    public void ResetReward()
    {
        rewardValue = 0.001f;
        rewardBar.value = rewardValue;
        rewardReady = false;
        // Save the reset state
        SaveRewardState();
    }

    private void SaveRewardState()
    {
        // Save reward value and level to PlayerPrefs
        PlayerPrefs.SetFloat(RewardValueKey, rewardValue);
        PlayerPrefs.SetInt(RewardReady, rewardReady ? 1 : 0);
        PlayerPrefs.Save();  // Don't forget to save the preferences
        Debug.Log("Reward state saved");
    }

    private void LoadRewardState()
    {
        // Load reward value and level from PlayerPrefs
        if (PlayerPrefs.HasKey(RewardValueKey) && PlayerPrefs.HasKey(RewardReady))
        {
            rewardValue = PlayerPrefs.GetFloat(RewardValueKey);
            rewardReady = PlayerPrefs.GetInt(RewardReady) == 1;
            rewardBar.value = rewardValue;
            Debug.Log("Reward state loaded");
        }
        else
        {
            // If no saved state exists, set initial values
            rewardValue = 0;
            level = 0;
            rewardBar.value = rewardValue;
            Debug.Log("No saved reward state found, using default values");
        }
    }
}
