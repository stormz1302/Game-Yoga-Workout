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
    [SerializeField] TMP_Text timeText;

    void Start()
    {
        LoadState();

        // Kiểm tra trạng thái cooldown
        if (isCooldownActive)
        {
            UpdateCooldownState();
            ButtonCoolDown buttonCoolDown = FindObjectOfType<ButtonCoolDown>();
            if (buttonCoolDown != null)
            {
                int cooldownDuration = (int)(cooldownEndTime - DateTime.Now).TotalSeconds;
                buttonCoolDown.StartCooldown(videoGitsbutton, cooldownDuration, CooldownTimePerUse);
            }
        }

        UpdateUI();
    }

    void Update()
    {
        // Cập nhật trạng thái cooldown
        if (isCooldownActive)
        {
            TimeSpan remainingTime = cooldownEndTime - DateTime.Now;
            videoGitsbutton.gameObject.GetComponent<Animation>().Stop();
            if (remainingTime <= TimeSpan.Zero)
            {
                // Cooldown đã kết thúc nhưng không reset Timer
                isCooldownActive = false;
                videoGitsbutton.gameObject.GetComponent<Animation>().Play();
                SaveState();
            }
        }

        // Cập nhật nếu qua ngày mới (reset vào 0h hôm sau)
        if (DateTime.Now >= GetNextDailyResetTime())
        {
            ResetDailyUsage();
        }

        UpdateUI();
    }

    public void SetCoolDownTime()
    {
        // Khi nhấn nút, đặt thời gian cooldown
        videoGitsbutton.gameObject.GetComponent<Animation>().Stop();
        if (Timer < MaxDailyUsage)
        {
            cooldownEndTime = DateTime.Now.AddSeconds(CooldownTimePerUse);
            Timer++;
            isCooldownActive = true;
            SaveState();

            // Gọi hàm để bắt đầu cooldown
            ButtonCoolDown buttonCoolDown = FindObjectOfType<ButtonCoolDown>();
            if (buttonCoolDown != null)
            {
                int cooldownDuration = (int)(cooldownEndTime - DateTime.Now).TotalSeconds;
                buttonCoolDown.StartCooldown(videoGitsbutton, cooldownDuration, CooldownTimePerUse);
            }
        }
    }

    private void ResetDailyUsage()
    {
        // Reset số lần sử dụng vào lúc 0h ngày mới
        Timer = 0;
        isCooldownActive = false;
        cooldownEndTime = DateTime.MinValue;
        SaveState();
    }

    private DateTime GetNextDailyResetTime()
    {
        // Trả về thời điểm reset tiếp theo (0h hôm sau)
        DateTime nextReset = DateTime.Now.Date.AddDays(1).AddHours(DailyResetHour);
        return nextReset;
    }

    private void UpdateCooldownState()
    {
        // Nếu cooldown vẫn hoạt động
        if (DateTime.Now >= cooldownEndTime)
        {
            isCooldownActive = false;
        }

        // Nếu qua ngày mới, reset vào 0h hôm sau
        if (DateTime.Now >= GetNextDailyResetTime())
        {
            ResetDailyUsage();
        }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("TimerVideoAds", Timer);
        PlayerPrefs.SetString("VideoGiftCooldownEndTime", cooldownEndTime.ToString());
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        Timer = PlayerPrefs.GetInt("TimerVideoAds", 0);

        if (PlayerPrefs.HasKey("VideoGiftCooldownEndTime"))
        {
            cooldownEndTime = DateTime.Parse(PlayerPrefs.GetString("VideoGiftCooldownEndTime"));

            // Nếu đã qua ngày mới, reset vào 0h hôm sau
            if (DateTime.Now >= GetNextDailyResetTime())
            {
                ResetDailyUsage();
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
        videoGitsbutton.interactable = !isCooldownActive && Timer < MaxDailyUsage;
    }
}
