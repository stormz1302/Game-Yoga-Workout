using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsRewardInGame : MonoBehaviour
{
    [SerializeField] private Button adsButton;
    [SerializeField] private int rewardValue;
    [SerializeField] private GameObject iconMoney;
    [SerializeField] private GameObject rewardView;
    [SerializeField] private Animation rewardAnim;

    private void Start()
    {
        adsButton.onClick.AddListener(OnAdsButtonClicked);
        Time.timeScale = 0;
        rewardAnim[rewardAnim.clip.name].speed = 1f;
        rewardView.SetActive(true);
        iconMoney.SetActive(false);
    }

    private void OnAdsButtonClicked()
    {
        // Show ads here
        AdsController.instance.ShowReward(() =>
        {
            
            GameManager.Instance.AddMoney(rewardValue);
            iconMoney.SetActive(true);
            rewardAnim.Play();
            AudioManager.Instance.PlaySound("MoneyPickup");
            gameObject.SetActive(false);
            Invoke("CloseAdsPopp", 1.1f);
        }, "Reward-Money-Ads-In-Game");
        
    }

    private void CloseAdsPopp()
    {
        iconMoney.SetActive(false);
        Time.timeScale = 1;
    }
}
