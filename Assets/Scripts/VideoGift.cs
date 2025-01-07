using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoGift : MonoBehaviour
{
    public Button rewardButton; 
    public int rewardAmount = 100;
    public GameObject adsRewardPop;
    public GameObject iconMoney;

    private void Start()
    {
        // Gắn sự kiện khi nhấn vào nút
        rewardButton.onClick.AddListener(OnRewardButtonClicked);
    }

    public void OnRewardButtonClicked()
    {
        // Kiểm tra nếu quảng cáo đã sẵn sàng
        if (AdsController.instance != null)
        {
            AdsController.instance.ShowReward(() =>
            {
                // Hành động khi người chơi xem xong quảng cáo
                GiveReward();
            }, "Reward_Button_Click");
        }
        else
        {
            Debug.LogWarning("AdsController chưa được khởi tạo!");
        }
    }

    private void GiveReward()
    {
        // Thực hiện logic nhận quà
        adsRewardPop.SetActive(true);
        // Cộng tiền cho người chơi
        GameManager.Instance.money += rewardAmount;
        SaveData saveData = new SaveData();
        saveData.Save();
        VideoGiftCooldown videoGiftCooldown = FindObjectOfType<VideoGiftCooldown>();
        if (videoGiftCooldown != null)
        {
            videoGiftCooldown.SetCoolDownTime();
        }
    }
    
    public void CloseRewardPop()
    {
        adsRewardPop.SetActive(false);
        iconMoney.SetActive(true);
        iconMoney.GetComponent<Animation>().Play();
        AudioManager.Instance.PlaySound("UnlockGift");
        Invoke("InActiveIconMoney", 1f);
    }

    private void InActiveIconMoney()
    {
        iconMoney.SetActive(false);
    }
}
