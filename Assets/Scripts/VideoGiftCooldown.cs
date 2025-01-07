using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoGiftCooldown : MonoBehaviour
{
    private int Timer = 0; // Số lần đã sử dụng trong ngày
    public Button videoGitsbutton;
    private DateTime cooldownEndTime; // Thời điểm kết thúc cooldown
    private bool isCooldownActive = false;
    private const int MaxDailyUsage = 3; // Số lần sử dụng tối đa mỗi ngày
    private const int DailyResetHour = 0; // Giờ reset (0h mỗi ngày)
    private const int CooldownTimePerUse = 300; // Cooldown 5 phút mỗi lần sử dụng
    private int coolDownTime;
    [SerializeField] TMP_Text timeText;

    void Start()
    {
        LoadState();

        // Nếu cooldown vẫn đang hoạt động
        if (isCooldownActive)
        {
            UpdateCooldownState();
            ButtonCoolDown buttonCoolDown = FindObjectOfType<ButtonCoolDown>();
            if (buttonCoolDown != null)
            {
                int cooldownDuration = (int)(cooldownEndTime - DateTime.Now).TotalSeconds;
                buttonCoolDown.StartCooldown(videoGitsbutton, cooldownDuration, coolDownTime);
            }
        }

        UpdateUI();
    }

    void Update()
    {
        if (isCooldownActive)
        {
            TimeSpan remainingTime = cooldownEndTime - DateTime.Now;

            // Kiểm tra nếu cooldown đã kết thúc
            if (remainingTime <= TimeSpan.Zero)
            {
                ResetCooldown();
                videoGitsbutton.gameObject.GetComponent<Animation>().Play();
            }
            else
            {
                videoGitsbutton.gameObject.GetComponent<Animation>().Stop();
                
            }
        }
    }

    public void SetCoolDownTime()
    {
        videoGitsbutton.gameObject.GetComponent<Animation>().Stop();

        if (Timer < MaxDailyUsage - 1)
        {
            cooldownEndTime = DateTime.Now.AddSeconds(CooldownTimePerUse); // Cooldown 5 phút cho mỗi lần sử dụng
        }
        else
        {
            cooldownEndTime = GetNextDailyResetTime(); // Cooldown đến ngày mới
        }
        Timer++;
        isCooldownActive = true;
        SaveState();
        UpdateUI();

        // Gọi hàm để bắt đầu cooldown (nếu cần)
        ButtonCoolDown buttonCoolDown = FindObjectOfType<ButtonCoolDown>();
        if (buttonCoolDown != null)
        {
            int cooldownDuration = (int)(cooldownEndTime - DateTime.Now).TotalSeconds;
            coolDownTime = cooldownDuration;
            buttonCoolDown.StartCooldown(videoGitsbutton, cooldownDuration, cooldownDuration);
        }
    }

    private void ResetCooldown()
    {
        isCooldownActive = false;
        Timer = 0;
        cooldownEndTime = DateTime.MinValue; // Đặt lại thời gian cooldown
        SaveState();
        UpdateUI();
    }

    private void UpdateCooldownState()
    {
        // Nếu đã qua ngày mới, reset số lần sử dụng
        if (DateTime.Now.Date > cooldownEndTime.Date)
        {
            ResetCooldown();
        }
        else if (DateTime.Now >= cooldownEndTime)
        {
            isCooldownActive = false;
        }
    }

    private DateTime GetNextDailyResetTime()
    {
        // Tính thời gian reset vào 0h ngày hôm sau
        DateTime nextReset = DateTime.Now.Date.AddDays(1).AddHours(DailyResetHour);
        return nextReset;
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("TimerVideoAds", Timer);
        PlayerPrefs.SetString("VideoGiftCooldownEndTime", cooldownEndTime.ToString());
        PlayerPrefs.SetInt("CoolDownTime", coolDownTime);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        Timer = PlayerPrefs.GetInt("TimerVideoAds", 0);
        coolDownTime = PlayerPrefs.GetInt("CoolDownTime", 0);
        if (PlayerPrefs.HasKey("VideoGiftCooldownEndTime"))
        {
            cooldownEndTime = DateTime.Parse(PlayerPrefs.GetString("VideoGiftCooldownEndTime"));

            // Nếu qua ngày mới, reset
            if (DateTime.Now.Date > cooldownEndTime.Date)
            {
                ResetCooldown();
            }
            else
            {
                isCooldownActive = DateTime.Now < cooldownEndTime;
            }
        }
        else
        {
            cooldownEndTime = DateTime.MinValue;
            isCooldownActive = false;
        }
    }

    private void UpdateUI()
    {
        timeText.text = $"{MaxDailyUsage - Timer}/{MaxDailyUsage}";
        videoGitsbutton.interactable = !isCooldownActive || Timer < MaxDailyUsage;
    }
}
