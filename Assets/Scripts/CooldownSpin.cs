using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownSpin : MonoBehaviour
{
    public Button freeButton;
    public Button adsButton;
    public TMP_Text freeButtonTimerText;
    public TMP_Text adsButtonTimerText;
    public TMP_Text freeButtonUsageText;
    public TMP_Text adsButtonUsageText;

    private DateTime freeButtonNextAvailableTime;
    private DateTime adsButtonNextAvailableTime;

    private TimeSpan freeCooldown = TimeSpan.FromDays(1);
    private TimeSpan initialAdsCooldown = TimeSpan.FromMinutes(5);
    private TimeSpan regularAdsCooldown = TimeSpan.FromDays(1);

    private int freeButtonUsageCount = 0;
    private int adsButtonUsageCount = 0;
    private const int freeButtonMaxUsage = 1;
    private const int adsButtonMaxUsage = 2;

    private const string FreeButtonTimeKey = "FreeButtonNextAvailableTime";
    private const string AdsButtonTimeKey = "AdsButtonNextAvailableTime";
    private const string FreeButtonUsageKey = "FreeButtonUsageCount";
    private const string AdsButtonUsageKey = "AdsButtonUsageCount";
    private const string AdsButtonLastUsedDateKey = "AdsButtonLastUsedDate";

    void Start()
    {
        LoadState();

        freeButton.onClick.AddListener(OnFreeButtonClick);
        adsButton.onClick.AddListener(OnAdsButtonClick);

        UpdateButtonStates();
        UpdateUsageTexts();
    }

    void Update()
    {
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
            freeButtonNextAvailableTime = DateTime.Now.Add(freeCooldown);
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

        ResetAdsButtonIfNeeded(); // Reset usage count if the date has changed

        if (DateTime.Now >= adsButtonNextAvailableTime && adsButtonUsageCount < adsButtonMaxUsage)
        {
            Debug.Log("Ads button clicked!");

            if (adsButtonUsageCount == 0)
            {
                adsButtonNextAvailableTime = DateTime.Now.Add(initialAdsCooldown);
            }
            else
            {
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
        // Kiểm tra và reset FreeButton nếu đã hết cooldown
        if (DateTime.Now >= freeButtonNextAvailableTime && freeButtonUsageCount >= freeButtonMaxUsage)
        {
            freeButtonUsageCount = 0; // Reset số lần sử dụng
            SaveState();
        }
        freeButton.interactable = DateTime.Now >= freeButtonNextAvailableTime && freeButtonUsageCount < freeButtonMaxUsage;

        // Kiểm tra và reset AdsButton nếu đã hết cooldown hoặc ngày mới bắt đầu
        ResetAdsButtonIfNeeded(); // Reset nếu ngày đã thay đổi
        if (DateTime.Now >= adsButtonNextAvailableTime && adsButtonUsageCount >= adsButtonMaxUsage)
        {
            adsButtonUsageCount = 0; // Reset số lần sử dụng
            SaveState();
        }
        adsButton.interactable = DateTime.Now >= adsButtonNextAvailableTime && adsButtonUsageCount < adsButtonMaxUsage;
    }

    void UpdateCountdownTexts()
    {
        if (DateTime.Now < freeButtonNextAvailableTime)
        {
            TimeSpan remainingTime = freeButtonNextAvailableTime - DateTime.Now;
            freeButtonTimerText.text = FormatTime(remainingTime);
        }
        else
        {
            freeButtonTimerText.gameObject.SetActive(false);
        }

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
        PlayerPrefs.SetString(AdsButtonLastUsedDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
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

    void ResetAdsButtonIfNeeded()
    {
        string lastUsedDate = PlayerPrefs.GetString(AdsButtonLastUsedDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
        DateTime lastUsed = DateTime.Parse(lastUsedDate);

        if (lastUsed.Date < DateTime.Now.Date)
        {
            adsButtonUsageCount = 0;
            adsButtonNextAvailableTime = DateTime.Now;
            SaveState();
        }
    }
}
