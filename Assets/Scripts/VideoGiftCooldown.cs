using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoGiftCooldown : MonoBehaviour
{
    private int Timer = 0;
    public Button videoGitsbutton;
    private DateTime cooldownEndTime; // Thời điểm kết thúc cooldown
    private bool isCooldownActive = false;
    private const int MaxCooldownTime = 86400; // 24h (86400 giây)
    [SerializeField] TMP_Text timeText;

    void Start()
    {
        // Kiểm tra nếu đã có cooldown lưu trong PlayerPrefs
        if (PlayerPrefs.HasKey("VideoGiftCooldownEndTime"))
        {
            string savedTime = PlayerPrefs.GetString("VideoGiftCooldownEndTime");
            cooldownEndTime = DateTime.Parse(savedTime);

            // Nếu cooldown vẫn còn hiệu lực
            if (DateTime.Now < cooldownEndTime)
            {
                isCooldownActive = true;
                ButtonCoolDown buttonCoolDown = FindObjectOfType<ButtonCoolDown>();
                if (buttonCoolDown != null)
                {
                    TimeSpan remainingTime = cooldownEndTime - DateTime.Now;
                    if (remainingTime.TotalSeconds > MaxCooldownTime)
                    {
                        ResetCooldown();
                    }
                    else
                    {
                        int cooldownTime = PlayerPrefs.GetInt("VideoGiftCooldownTime");
                        buttonCoolDown.StartCooldown(videoGitsbutton, (int)remainingTime.TotalSeconds, cooldownTime);
                    }
                }
            }
        }
        Timer = PlayerPrefs.GetInt("TimerVideoAds", 0);
        timeText.text = (3 - Timer).ToString() + "/3";
    }

    void Update()
    {
        if (isCooldownActive)
        {
            TimeSpan remainingTime = cooldownEndTime - DateTime.Now;
            // Nếu cooldown đã kết thúc hoặc vượt quá thời gian tối đa
            if (remainingTime <= TimeSpan.Zero || remainingTime.TotalSeconds > MaxCooldownTime)
            {
                ResetCooldown();
                videoGitsbutton.gameObject.GetComponent<Animation>().Play();
            }else videoGitsbutton.gameObject.GetComponent<Animation>().Stop();
        }
    }

    public void SetCoolDownTime()
    {
        videoGitsbutton.gameObject.GetComponent<Animation>().Stop();
        Timer++;
        int cooldownTime = 0;

        switch (Timer)
        {
            case 1:
                cooldownTime = 300; 
                break;
            case 2:
                cooldownTime = 300; 
                break;
            case 3:
                cooldownTime = 600; 
                break;
        }

        if (Timer > 3)
        {
            Timer = 0;
            cooldownTime = MaxCooldownTime; // 3 giờ (10800 giây)
        }
        PlayerPrefs.SetInt("TimerVideoAds", Timer);
        PlayerPrefs.SetInt("VideoGiftCooldownTime", cooldownTime);
        // Tính thời gian kết thúc cooldown
        cooldownEndTime = DateTime.Now.AddSeconds(cooldownTime);
        isCooldownActive = true;
        timeText.text = (3 - Timer).ToString() + "/3";
        PlayerPrefs.SetString("VideoGiftCooldownEndTime", cooldownEndTime.ToString());
        PlayerPrefs.Save();
        ButtonCoolDown buttonCoolDown = FindObjectOfType<ButtonCoolDown>();
        if (buttonCoolDown != null)
        {
            buttonCoolDown.StartCooldown(videoGitsbutton, cooldownTime, cooldownTime);
        }
    }

    private void ResetCooldown()
    {
        isCooldownActive = false;
        PlayerPrefs.DeleteKey("VideoGiftCooldownEndTime");
        Timer = 0;
        PlayerPrefs.SetInt("TimerVideoAds", Timer);
    }
}
