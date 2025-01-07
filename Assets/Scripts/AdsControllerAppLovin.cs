using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using static MaxSdkCallbacks;

public class AdsControllerAppLovin : MonoBehaviour
{
    public static AdsControllerAppLovin instance;

    public Color bannerBackgroundColor;
    
#if UNITY_IOS
    string bannerAdUnitId = "YOUR_IOS_BANNER_ID"; 
    string mrecAdUnitId = "YOUR_IOS_MREC_ID"; 
    string interstitialAdUnitId = "YOUR_IOS_INTERSTITIAL_ID"; 
    string rewardedAdUnitId = "YOUR_IOS_REWARDED_ID"; 
#else // UNITY_ANDROID
    public string bannerAdUnitId = "YOUR_ANDROID_BANNER_ID";
    public string mrecAdUnitId = "YOUR_ANDROID_MREC_ID";
    public string interstitialAdUnitId = "YOUR_ANDROID_INTERSTITIAL_ID";
    public string rewardedAdUnitId = "YOUR_ANDROID_REWARDED_ID";
#endif

    private bool isBannerLoaded = false;
    private bool isMrecLoaded = false;
    private bool isInterstitialLoaded = false;
    private bool isRewardedLoaded = false;

    int retryAttempt;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        InitializeBannerAds();
        InitializeMRecAds();
        InitializeInterstitialAds();
        InitializeRewardedAds();

        Invoke("ShowBanner", 2f);
        //Invoke("ShowMrec", 3f);
        //Invoke("ShowInterstitial", 4f);
        //Invoke("ShowRewarded", 5f);
    }

    // Khởi tạo quảng cáo Banner
    public void InitializeBannerAds()
    {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, bannerBackgroundColor);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isBannerLoaded = true;
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isBannerLoaded = false;
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void ShowBanner()
    {
        if (isBannerLoaded)
        {
            MaxSdk.ShowBanner(bannerAdUnitId);
            Debug.Log("Banner is shown.");
        }
        else
        {
            Debug.Log("Banner is not ready.");
        }
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
        Debug.Log("Banner is hidden.");
    }

    // Khởi tạo quảng cáo MREC
    public void InitializeMRecAds()
    {
        MaxSdk.CreateMRec(mrecAdUnitId, MaxSdkBase.AdViewPosition.Centered);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
        isMrecLoaded = true;
    }
    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) 
    {
        isMrecLoaded = false;
    }
    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void ShowMrec()
    {
        if (isMrecLoaded)
        {
            MaxSdk.ShowMRec(mrecAdUnitId);
            Debug.Log("MREC is shown.");
        }
        else
        {
            Debug.Log("MREC is not ready.");
        }
    }

    public void HideMrec()
    {
        MaxSdk.HideMRec(mrecAdUnitId);
        Debug.Log("MREC is hidden.");
    }

    // Khởi tạo quảng cáo Interstitial
    public void InitializeInterstitialAds()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isInterstitialLoaded = true;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isInterstitialLoaded = false;
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
    public void ShowInterstitial()
    {
        if (isInterstitialLoaded)
        {
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
            Debug.Log("Interstitial is shown.");
        }
        else
        {
            Debug.Log("Interstitial is not ready.");
        }
    }

    public void HideInterstitial()
    {
        Debug.Log("Interstitial is hidden.");
    }

    // Khởi tạo quảng cáo Rewarded
    public void InitializeRewardedAds()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isRewardedLoaded = true;
        Time.timeScale = 1;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isRewardedLoaded = false;
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    public void ShowRewarded()
    {
        if (isRewardedLoaded)
        {
            Time.timeScale = 0;
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
            Debug.Log("Rewarded Ad is shown.");
        }
        else
        {
            Debug.Log("Rewarded Ad is not ready.");
        }
    }

    public void HideRewarded()
    {
        Debug.Log("Rewarded Ad is hidden.");
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded Ad received reward: " + reward.Amount);
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
}
