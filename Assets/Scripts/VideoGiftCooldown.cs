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
            if (remainingTime <= TimeSpan.Zero)
            {
                // Cooldown đã kết thúc nhưng không reset Timer
                isCooldownActive = false;
                SaveState();
            }
        }

        // Cập nhật nếu qua ngày mới (reset vào 0h hôm sau)
        DateTime lastReset = GetLastDailyResetTime();
        if (PlayerPrefs.HasKey("LastResetTime"))
        {
            DateTime savedResetTime = DateTime.Parse(PlayerPrefs.GetString("LastResetTime"));
            if (savedResetTime < lastReset)
            {
                ResetDailyUsage();
                PlayerPrefs.SetString("LastResetTime", lastReset.ToString());
                PlayerPrefs.Save();
            }
        }

        UpdateUI();
    }

    public void SetCoolDownTime()
    {
        // Khi nhấn nút, đặt thời gian cooldown
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

    private DateTime GetLastDailyResetTime()
    {
        // Tính thời điểm reset gần nhất
        DateTime lastReset = DateTime.Now.Date.AddHours(DailyResetHour);

        // Nếu thời gian hiện tại nhỏ hơn thời điểm reset hôm nay, lấy reset hôm qua
        if (DateTime.Now < lastReset)
        {
            lastReset = lastReset.AddDays(-1);
        }

        return lastReset;
    }

    private void UpdateCooldownState()
    {
        // Nếu cooldown vẫn hoạt động
        if (DateTime.Now >= cooldownEndTime)
        {
            isCooldownActive = false;
        }

        // Kiểm tra nếu qua ngày mới
        DateTime lastReset = GetLastDailyResetTime();
        if (PlayerPrefs.HasKey("LastResetTime"))
        {
            DateTime savedResetTime = DateTime.Parse(PlayerPrefs.GetString("LastResetTime"));

            if (savedResetTime < lastReset)
            {
                ResetDailyUsage();
                PlayerPrefs.SetString("LastResetTime", lastReset.ToString());
                PlayerPrefs.Save();
            }
        }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("TimerVideoAds", Timer);
        PlayerPrefs.SetString("VideoGiftCooldownEndTime", cooldownEndTime.ToString());
        PlayerPrefs.SetString("LastResetTime", GetLastDailyResetTime().ToString());
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        Timer = PlayerPrefs.GetInt("TimerVideoAds", 0);

        if (PlayerPrefs.HasKey("VideoGiftCooldownEndTime"))
        {
            cooldownEndTime = DateTime.Parse(PlayerPrefs.GetString("VideoGiftCooldownEndTime"));
        }
        else
        {
            cooldownEndTime = DateTime.MinValue;
        }

        // Kiểm tra nếu qua nhiều ngày không mở app
        DateTime lastReset = GetLastDailyResetTime();
        if (PlayerPrefs.HasKey("LastResetTime"))
        {
            DateTime savedResetTime = DateTime.Parse(PlayerPrefs.GetString("LastResetTime"));

            if (savedResetTime < lastReset)
            {
                ResetDailyUsage();
            }
        }
        else
        {
            ResetDailyUsage();
        }

        isCooldownActive = DateTime.Now < cooldownEndTime;
    }

    private void UpdateUI()
    {
        timeText.text = $"{MaxDailyUsage - Timer}/{MaxDailyUsage}";
        videoGitsbutton.interactable = !isCooldownActive && Timer < MaxDailyUsage;
    }
}
