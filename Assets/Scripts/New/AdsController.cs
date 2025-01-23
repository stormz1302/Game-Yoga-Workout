
using SolarEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsController : MonoBehaviour
{
    public static AdsController instance;
    DataManager _dataController;
    AudioManager audioManager;

    public string DevKeyAndroid;
    public string DevKeyIOS;

    public string bannerIdAndroid;
    public string interIdAndroid;
    public string videoIdAndroid;
    public string mrecIdAndroid;


    public string bannerIdIOS;
    public string interIdIOS;
    public string videoIdIOS;
    public string mrecIdIOS;
    

    string bannerId;
    string interId;
    string videoId;
    string mrecId;

    string appIdTemp;

    bool loadbannerDone;
    bool doneWatchAds = false;
    [HideInInspector]public bool Showing_applovin_ads;
    bool showingBanner;
    bool showingMrec;
    bool bannerOK;

    private void Start()
    {
        if (_dataController == null)
        {
            _dataController = DataManager.instance;
            audioManager = AudioManager.Instance;


#if UNITY_ANDROID
            appIdTemp = DevKeyAndroid;
            bannerId = bannerIdAndroid;
            interId = interIdAndroid;
            videoId = videoIdAndroid;
            mrecId = mrecIdAndroid;


#elif UNITY_IOS
                appIdTemp = DevKeyIOS;
                bannerId = bannerIdIOS;
                interId = interIdIOS;
                videoId = videoIdIOS;
                mrecId = mrecIdIOS;
         
#endif
            Init();
            StartCoroutine(ShowBannerInLoad());
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
    public void Init()
    {

        //MaxSdk.SetSdkKey(appIdTemp);
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
            InitializeMRecAds();
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidForSolarEngine;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidForSolarEngine;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidForSolarEngine;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidForSolarEngine;
        };
    }
    #region Mrect

    //bool mrecOk;
    public void InitializeMRecAds()
    {
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(mrecId, MaxSdkBase.AdViewPosition.Centered);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }
    public void ShowMrec()
    {
        if (DataManager.instance.saveData.removeAds)
            return;
        showingMrec = true;
        Debug.Log("=== show mrec");
        MaxSdk.ShowMRec(mrecId);
    }
    public void HideMrec()
    {
        if (showingMrec)
        {
            MaxSdk.HideMRec(mrecId);
            showingMrec = false;
        }
    }
    public void ShowMrecCentered()
    {
        MaxSdk.UpdateMRecPosition(mrecId, MaxSdkBase.AdViewPosition.Centered);
        ShowMrec();
    }
    public void ShowMrecBottomCenter()
    {
        MaxSdk.UpdateMRecPosition(mrecId, MaxSdkBase.AdViewPosition.BottomCenter);
        ShowMrec();
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //mrecOk = true;
        Debug.Log("====== load native success:");
    }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
    {
        //mrecOk = false;
        Debug.LogError("====== load native false:" + error.Message);
    }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    
    #endregion

    #region banner
    public void InitializeBannerAds()
    {
        MaxSdk.CreateBanner(bannerId, MaxSdkBase.BannerPosition.BottomCenter);

        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += Banner_OnAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += Banner_OnAdLoadedEvent;

#if UNITY_EDITOR

#else
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerId, colorBanner);
#endif
        //MaxSdk.SetBannerWidth(bannerId, 750);
    }

    private void Banner_OnAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        Debug.Log("====load banner success ");
        bannerOK = true;
/*        if(showingBanner == false)
        {
            ShowBanner();
        }*/
    }

    private void Banner_OnAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        Debug.LogError("====load banner false ");
        bannerOK = false;
    }

    public void ShowBanner()
    {
        if (DataManager.instance.saveData.removeAds)
        {
            Debug.Log("remove ads = true || remote firebase off");
            return;
        }
            
        Debug.Log("=== show banner");
        if (bannerOK)
        {
            showingBanner = true;
            MaxSdk.ShowBanner(bannerId);
        }

    }
    public void StartCoroutineBanner()
    {
        StartCoroutine(ShowBannerInLoad());
    }
    IEnumerator ShowBannerInLoad(){
        while (!bannerOK)
        {
            yield return null;
        }
        ShowBanner();
    }
    public void HideBanner()
    {
        Debug.Log("=== hide banner");
        if (showingBanner)
        {
            MaxSdk.HideBanner(bannerId);
            showingBanner = false;
        }

    }
    public Color colorBanner;

    #endregion


    #region Inter
    public void InitializeInterstitialAds()
    {
        // Gắn kết các callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += Interstitial_OnAdLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += Interstitial_OnAdLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += Interstitial_OnAdDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += Interstitial_OnAdDisplayFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += Interstitial_OnAdClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += Interstitial_OnAdHiddenEvent;

        // Yêu cầu tải quảng cáo ban đầu
        RequestInter();
    }

    private void Interstitial_OnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log($"Quảng cáo interstitial tải không thành công. Lỗi: {errorInfo.Message}");

        // Cơ chế thử lại: Yêu cầu tải lại quảng cáo sau 5 giây
        Invoke(nameof(RequestInter), 5f);
    }

    private void Interstitial_OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Quảng cáo interstitial đã tải thành công.");
    }

    private void Interstitial_OnAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        EventController.SUM_INTER_ALL_GAME();
        // EventController.AB_INTER_ID(DataParam.showInterType);

        // Dừng âm thanh và thời gian game
        audioManager.StopMusic();
        audioManager.StopSound();
        Time.timeScale = 0;

        Debug.Log("Quảng cáo interstitial đã được hiển thị.");
    }

    private void Interstitial_OnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Quảng cáo interstitial đã ẩn, yêu cầu tải quảng cáo tiếp theo
        Time.timeScale = 1; // Tiếp tục game
        ChangeSettingAudio();
        RequestInter(); // Tải quảng cáo tiếp theo
        DataParam.beginShowInter = System.DateTime.Now;
        Showing_applovin_ads = false;

        Debug.Log("Quảng cáo interstitial đã đóng.");
    }

    private void Interstitial_OnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Quảng cáo interstitial đã được nhấp.");
        // Bạn có thể thêm các hành động theo dõi hoặc các hành động bổ sung ở đây
    }

    private void Interstitial_OnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log($"Quảng cáo interstitial không thể hiển thị. Lỗi: {errorInfo.Message}");
        // Bạn có thể xử lý lại hoặc thêm logic dự phòng ở đây
    }

    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        Debug.Log($"Quảng cáo interstitial đã bị hủy. ID quảng cáo: {adUnitId}");
        // Bạn có thể thêm các hành động dọn dẹp hoặc quản lý trạng thái ở đây
    }

    public void RequestInter()
    {
        MaxSdk.LoadInterstitial(interId);
        Debug.Log("Yêu cầu tải quảng cáo interstitial.");
    }

    public void CheckToShowInter()
    {
        if (MaxSdk.IsInterstitialReady(interId))
        {
            MaxSdk.ShowInterstitial(interId);
            Showing_applovin_ads = true;
            Debug.Log("Hiển thị quảng cáo interstitial.");
        }
        else
        {
            Debug.LogWarning("Quảng cáo interstitial chưa sẵn sàng, yêu cầu tải quảng cáo...");
            RequestInter();
        }
    }

    public void ShowInter()
    {
        if (DataManager.instance.saveData.removeAds)
        {
            Debug.Log("Quảng cáo đã bị tắt bởi người dùng.");
            return;
        }

        DataParam.lastShowInter = DateTime.Now;

        // Đảm bảo đủ thời gian đã trôi qua kể từ lần quảng cáo trước
        if ((DataParam.lastShowInter - DataParam.beginShowInter).TotalSeconds > DataParam.timeDelayShowAds)
        {
            CheckToShowInter();
            AudioManager.Instance.PlaySound("ShowAds");
        }
        else
        {
            Debug.Log("Chưa đủ thời gian kể từ lần quảng cáo trước.");
        }
    }

    #endregion


    #region reward
    int retryAttemptVideo;
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardDisplayedEvent;

        // Load the first rewarded ad
        RequestReward();
    }

    private void OnRewardDisplayedEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        throw new NotImplementedException();
    }

    private void OnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo info)
    {
        doneWatchAds = true;
    }

    private void OnRewardedAdHiddenEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        Showing_applovin_ads = false;
        ChangeSettingAudio();
        StartCoroutine(delayAction());
        // RequestVideo();
    }

    private void OnRewardedAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo info1, MaxSdkBase.AdInfo info2)
    {
        // display failed
    }

    private void OnRewardedAdFailedEvent(string arg1, MaxSdkBase.ErrorInfo info)
    {
        // load failed
    }

    private void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        Debug.Log("========= video load sucess");
    }

    public void RequestReward()
    {
        //   IronSource.Agent.init(appIdTemp, IronSourceAdUnits.REWARDED_VIDEO);
        MaxSdk.LoadRewardedAd(videoId);
    }

    IEnumerator delayAction()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (doneWatchAds)
        {
            acreward();
            EventController.SUM_VIDEO_SHOW_NAME(nameEventVideo);
            DataParam.countShowVideo++;
        }
        RequestReward();
        // Debug.LogError("====== close video");
        acreward = null;

        doneWatchAds = false;
        Time.timeScale = 1;
    }
    Action acreward;
    string nameEventVideo;
    public void ShowReward(Action _ac, string name)
    {
        if (MaxSdk.IsRewardedAdReady(videoId))
        {
            AudioManager.Instance.PlaySound("ShowAds");
            acreward = _ac;
            doneWatchAds = false;
            nameEventVideo = name;
            MaxSdk.ShowRewardedAd(videoId);
            Time.timeScale = 0;
            audioManager.StopMusic();
            audioManager.StopSound();

            //DataParam.afterShowAds = true;
            Showing_applovin_ads = true;
            // Debug.LogError("------ video show video");
        }
        else
        {
            //   Debug.LogError("------ video chua load");
            RequestReward();
        }
    }
    #endregion


    void ChangeSettingAudio()
    {
        //save audio setting
        //soundController.ChangeSettingMusic();
        //soundController.ChangeSettingSound();
        audioManager.LoadVolumeSettings();
    }
    void OnAdRevenuePaidForSolarEngine(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        // Solar engine
        AppImpressionAttributes impressionAttributes = new AppImpressionAttributes();
        impressionAttributes.ad_platform = impressionData.NetworkName;
        impressionAttributes.ad_appid = appIdTemp;
        impressionAttributes.ad_id = impressionData.AdUnitIdentifier;
        impressionAttributes.ad_type = GetAdType(impressionData.AdFormat);
        impressionAttributes.ad_ecpm = impressionData.Revenue * 1000.00;
        impressionAttributes.currency_type = "USD";
        impressionAttributes.mediation_platform = "MAX";//Please input the mediation platform you're using.
        impressionAttributes.is_rendered = true;
        // Please do not report custom properties starting with "_", otherwise SDK will abandon its value by default.
        SolarEngine.Analytics.trackIAI(impressionAttributes);
    }
    public static int GetAdType(string format)
    {
        if (format == "BANNER")
        {
            return 5;
        }
        else if (format == "INTER")
        {
            return 3;
        }
        else if (format == "APP_OPEN")
        {
            return 2;
        }
        else if (format == "REWARDED")
        {
            return 1;
        }
        else if (format == "MREC")
        {
            return 10;
        }
        return 0;
    }
}
