using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class CooldownSpin : MonoBehaviour
{
    public Button freeButton;
    public Button adsButton;
    public TMP_Text freeButtonTimerText; // Text to show countdown for FreeButton
    public TMP_Text adsButtonTimerText;  // Text to show countdown for AdsButton
    public TMP_Text freeButtonUsageText; // Text to show remaining usage for FreeButton
    public TMP_Text adsButtonUsageText;  // Text to show remaining usage for AdsButton

    private DateTime freeButtonNextAvailableTime;
    private DateTime adsButtonNextAvailableTime;

    private TimeSpan freeCooldown = TimeSpan.FromDays(1); // 1 day cooldown for FreeButton
    private TimeSpan initialAdsCooldown = TimeSpan.FromMinutes(5);  // 5 minutes cooldown for AdsButton on first click
    private TimeSpan regularAdsCooldown = TimeSpan.FromDays(1);  // 24 hours cooldown for AdsButton after the first click

    private int freeButtonUsageCount = 0;
    private int adsButtonUsageCount = 0;
    private const int freeButtonMaxUsage = 1; // Max usage per day for FreeButton
    private const int adsButtonMaxUsage = 2;  // Max usage per day for AdsButton

    private const string FreeButtonTimeKey = "FreeButtonNextAvailableTime";
    private const string AdsButtonTimeKey = "AdsButtonNextAvailableTime";
    private const string FreeButtonUsageKey = "FreeButtonUsageCount";
    private const string AdsButtonUsageKey = "AdsButtonUsageCount";
    private const string AdsButtonLastUsedTimeKey = "AdsButtonLastUsedTime"; // Track last used time for Ads button

    void Start()
    {
        // Load saved states
        LoadState();

        // Initialize buttons and check their states
        freeButton.onClick.AddListener(OnFreeButtonClick);
        adsButton.onClick.AddListener(OnAdsButtonClick);

        UpdateButtonStates();
        UpdateUsageTexts();
    }

    void Update()
    {
        // Update button states and countdowns in real-time
        UpdateButtonStates();
        UpdateCountdownTexts();
        UpdateUsageTexts();
    }

    void OnFreeButtonClick()
    {
        freeButtonTimerText.gameObject.SetActive(true);
        if (DateTime.Now >= freeButtonNextAvailableTime && freeButtonUsageCount < freeButtonMaxUsage)
        {
            Debug.Log("Free button clicked!");
            freeButtonNextAvailableTime = DateTime.Now.Add(freeCooldown); // Set cooldown for 1 day
            freeButtonUsageCount++;
            SaveState();
        }
        else if (freeButtonUsageCount >= freeButtonMaxUsage)
        {
            Debug.Log("Free button usage limit reached!");
        }
        else
        {
            Debug.Log("Free button is on cooldown!");
        }
    }

    void OnAdsButtonClick()
    {
        adsButtonTimerText.gameObject.SetActive(true);

        // Check if 24 hours have passed since last use
        if (DateTime.Now - GetAdsButtonLastUsedTime() >= TimeSpan.FromDays(1))
        {
            adsButtonUsageCount = 0; // Reset usage count after 24 hours
        }

        if (DateTime.Now >= adsButtonNextAvailableTime && adsButtonUsageCount < adsButtonMaxUsage)
        {
            Debug.Log("Ads button clicked!");

            // Set cooldown based on the number of usages
            if (adsButtonUsageCount == 0)
            {
                // First click: 5 minutes cooldown
                adsButtonNextAvailableTime = DateTime.Now.Add(initialAdsCooldown);
            }
            else
            {
                // Subsequent clicks: 24 hours cooldown
                adsButtonNextAvailableTime = DateTime.Now.Add(regularAdsCooldown);
            }

            adsButtonUsageCount++;
            SaveState();
        }
        else if (adsButtonUsageCount >= adsButtonMaxUsage)
        {
            Debug.Log("Ads button usage limit reached!");
        }
        else
        {
            Debug.Log("Ads button is on cooldown!");
        }
    }

    void UpdateButtonStates()
    {
        // Enable or disable FreeButton based on cooldown and usage count
        freeButton.interactable = DateTime.Now >= freeButtonNextAvailableTime && freeButtonUsageCount < freeButtonMaxUsage;

        // Enable or disable AdsButton based on cooldown and usage count
        adsButton.interactable = DateTime.Now >= adsButtonNextAvailableTime && adsButtonUsageCount < adsButtonMaxUsage;
    }

    void UpdateCountdownTexts()
    {
        // Update FreeButton timer text
        if (DateTime.Now < freeButtonNextAvailableTime)
        {
            TimeSpan remainingTime = freeButtonNextAvailableTime - DateTime.Now;
            freeButtonTimerText.text = FormatTime(remainingTime);
        }
        else
        {
            freeButtonTimerText.gameObject.SetActive(false);
        }

        // Update AdsButton timer text
        if (DateTime.Now < adsButtonNextAvailableTime)
        {
            TimeSpan remainingTime = adsButtonNextAvailableTime - DateTime.Now;
            adsButtonTimerText.text = FormatTime(remainingTime);
        }
        else
        {
            adsButtonTimerText.gameObject.SetActive(false);
        }
    }

    void UpdateUsageTexts()
    {
        // Update usage count texts
        freeButtonUsageText.text = $" {freeButtonMaxUsage - freeButtonUsageCount}/{freeButtonMaxUsage}";
        adsButtonUsageText.text = $" {adsButtonMaxUsage - adsButtonUsageCount}/{adsButtonMaxUsage}";
    }

    string FormatTime(TimeSpan time)
    {
        if (time.TotalHours >= 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        else
            return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

    void SaveState()
    {
        PlayerPrefs.SetString(FreeButtonTimeKey, freeButtonNextAvailableTime.ToString());
        PlayerPrefs.SetString(AdsButtonTimeKey, adsButtonNextAvailableTime.ToString());
        PlayerPrefs.SetInt(FreeButtonUsageKey, freeButtonUsageCount);
        PlayerPrefs.SetInt(AdsButtonUsageKey, adsButtonUsageCount);
        PlayerPrefs.SetString(AdsButtonLastUsedTimeKey, DateTime.Now.ToString()); // Save last use time for Ads button
        PlayerPrefs.Save();
    }

    void LoadState()
    {
        if (PlayerPrefs.HasKey(FreeButtonTimeKey))
        {
            freeButtonNextAvailableTime = DateTime.Parse(PlayerPrefs.GetString(FreeButtonTimeKey));
        }
        else
        {
            freeButtonNextAvailableTime = DateTime.Now;
        }

        if (PlayerPrefs.HasKey(AdsButtonTimeKey))
        {
            adsButtonNextAvailableTime = DateTime.Parse(PlayerPrefs.GetString(AdsButtonTimeKey));
        }
        else
        {
            adsButtonNextAvailableTime = DateTime.Now;
        }

        freeButtonUsageCount = PlayerPrefs.GetInt(FreeButtonUsageKey, 0);
        adsButtonUsageCount = PlayerPrefs.GetInt(AdsButtonUsageKey, 0);
    }

    DateTime GetAdsButtonLastUsedTime()
    {
        // Get the last used time for Ads Button
        if (PlayerPrefs.HasKey(AdsButtonLastUsedTimeKey))
        {
            return DateTime.Parse(PlayerPrefs.GetString(AdsButtonLastUsedTimeKey));
        }
        else
        {
            return DateTime.Now; // Default to current time if not found
        }
    }
}
