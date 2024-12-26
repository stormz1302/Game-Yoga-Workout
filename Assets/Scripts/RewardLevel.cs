using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardLevel : MonoBehaviour
{
    [Header("Reward Chest:")]
    [SerializeField] Button chestButton;
    [SerializeField] GameObject giftsPopup;
    [SerializeField] List<GiftScript> gifts = new List<GiftScript>();
    [SerializeField] Image flashImage;
    float flashDuration = 0.5f;
    float fadeDuration = 1f;
    bool isFirstOpenGift = true;

    [Header("Gifts Popup:")]
    [SerializeField] GameObject GiftsPopup;
    [SerializeField] GameObject Money;
    [SerializeField] GameObject Skin;
    [SerializeField] Image SkinSprite;
    [SerializeField] TMP_Text MoneyValue;

    private void Start()
    {
        chestButton.onClick.AddListener(OpenGiftsPopup);
        giftsPopup.SetActive(false);
        isFirstOpenGift = PlayerPrefs.GetInt("FirstOpenGift", 1) == 1;
        Debug.Log("Is first open gift: " + isFirstOpenGift);
    }

    private void OpenGiftsPopup()
    {
        giftsPopup.SetActive(true);
        GiftsPopup.SetActive(false);
    }

    public void CloseGiftsPopup()
    {
        ResetReward();
        giftsPopup.SetActive(false);
    }

    public void WatchAd()
    {
        // Show ad
        // Add reward to player
        AudioManager.Instance.PlaySound("UnlockGift");
        PlayerPrefs.SetInt("FirstOpenGift", 0);
        FlashScreen();
        RandomGift();
        // Close the gift popup
        //CloseGiftsPopup();
    }


    private void ResetReward()
    {
        AchievementReward achievementReward = FindObjectOfType<AchievementReward>();
        if (achievementReward != null)
        {
            achievementReward.ResetReward();
        }
    }

    private void FlashScreen()
    {
        if (flashImage == null) return;

        flashImage.gameObject.SetActive(true);
        flashImage.color = new Color(1, 1, 1, 1); // Đặt độ trong suốt ban đầu (1 = không trong suốt)
        // Bắt đầu hiệu ứng
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        float timer = 0f;

        // Lóa sáng
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Mờ dần
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration); // Giảm alpha từ 1 về 0
            flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    private void RandomGift()
    {
        if (gifts.Count == 0) return;

        // Chọn quà tặng ngẫu nhiên
        GiftScript selectedGift = RandomGiftsRatio();
        

        // Hiển thị quà tặng
        GiftsPopup.SetActive(true);

        if (isFirstOpenGift)
        {
            selectedGift = gifts[0];
            selectedGift.giftType = GiftScript.GiftType.Skin;
            Money.SetActive(false);
            Skin.SetActive(true);
            int skinID = Random.Range(6, 7);
            foreach (Character skin in selectedGift.skins)
            {
                if (skinID == skin.ID)
                    SkinSprite.sprite = skin.CharacterIcon;
            }
            // Unlock skin
            SkinsManager.instance.UnlockCharacter(skinID);
            return;
        }
        // Xử lý quà tặng theo GiftType

        switch (selectedGift.giftType)
        {
            case GiftScript.GiftType.Money:
                // Thêm logic để cộng tiền cho người chơi
                Money.SetActive(true);
                Skin.SetActive(false);
                MoneyValue.text = selectedGift.value.ToString();
                GameManager.Instance.money += selectedGift.value;
                SaveData saveData = new SaveData();
                saveData.Save();                
                break;
            case GiftScript.GiftType.Skin:
                // Thêm logic để unlock skin cho người chơi
                Money.SetActive(false);
                Skin.SetActive(true);
                int skinID = selectedGift.GetSkinID();
                bool isOwned = SkinsManager.instance.CheckOwnedCharacter(skinID);
                if (isOwned)
                {
                    selectedGift = gifts[1];
                    goto case GiftScript.GiftType.Money;
                }
                foreach (Character skin in selectedGift.skins)
                {
                    if (skinID == skin.ID)
                        SkinSprite.sprite = skin.CharacterIcon;
                }
                // Unlock skin
                SkinsManager.instance.UnlockCharacter(skinID);
                break;
            default:
                Debug.Log("Unknown gift type.");
                break;
        }
    }

    private GiftScript RandomGiftsRatio()
    {
        // Tính tổng tỉ lệ quà tặng
        float totalRatio = 0;
        foreach (var gift in gifts)
        {
            totalRatio += gift.ratio;
        }
        // Random một số từ 0 đến tổng tỉ lệ
        float randomValue = Random.Range(0, totalRatio);
        // Duyệt qua từng quà tặng
        float ratioSum = 0;
        foreach (var gift in gifts)
        {
            ratioSum += gift.ratio;
            if (randomValue <= ratioSum)
            {
                return gift;
            }
        }
        return gifts[1];
    }
}
