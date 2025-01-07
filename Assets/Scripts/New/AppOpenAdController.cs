using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System.Collections.Generic;
using SolarEngine;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads app open ad format.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/AppOpenAdController")]
    public class AppOpenAdController : MonoBehaviour
    {
        [SerializeField] private bool UseTestDevice;
        [SerializeField] private string testDeviceIds;
        bool firstShow;
        private AppOpenAd appOpenAd;
        private DateTime appOpenExpireTime;
        private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
        public static AppOpenAdController instance;

        private int tierIndex = 1;
        [SerializeField] private string ID_TIER_1_Android = "";
        [SerializeField] private string ID_TIER_2_Android = "";
        [SerializeField] private string ID_TIER_3_Android = "";

        //[SerializeField] private string ID_TIER_1_IOS = "";
        //[SerializeField] private string ID_TIER_2_IOS = "";
        //[SerializeField] private string ID_TIER_3_IOS = "";

        private string ID_TIER1;
        private string ID_TIER2;
        private string ID_TIER3;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            // Use the AppStateEventNotifier to listen to application open/close events.
            // This is used to launch the loaded ad when we open the app.
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        private void OnDestroy()
        {
            // Always unlisten to events when complete.
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }
        public bool IsAppOpenAdAvailable
        {
            get
            {
                return (appOpenAd != null
                        && appOpenAd.CanShowAd()
                        && DateTime.Now < appOpenExpireTime);
            }
        }
        private void OnAppStateChanged(AppState state)
        {
            Debug.Log("App State changed to : " + state);

            // if the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                if (IsAppOpenAdAvailable && firstShow)
                {
                    ShowAppOpenAd();
                }
            }
        }
        public void ShowAppOpenFirstLoadingFinish()
        {
            if (!firstShow)
            {
                Debug.Log("show app open first time");
                ShowAppOpenAd();
                firstShow = true;
            }
        }

        public void Start()
        {
            // Initialize the Google Mobile Ads SDK.

#if UNITY_ANDROID
            ID_TIER1 = ID_TIER_1_Android;
            ID_TIER2 = ID_TIER_2_Android;
            ID_TIER3 = ID_TIER_3_Android;
#else
            ID_TIER1 = ID_TIER_1_IOS;
            ID_TIER2 = ID_TIER_2_IOS;
            ID_TIER3 = ID_TIER_3_IOS;
#endif

            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                LoadAppOpenAd();
            });
        }

        /// <summary>
        /// Loads the app open ad.
        /// </summary>
        public void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }


            string id = ID_TIER1;
            if (tierIndex == 2)
                id = ID_TIER2;
            else if (tierIndex == 3)
                id = ID_TIER3;

            if (id == "")
                return;

            Debug.Log("Loading the app open ad.");

            if (UseTestDevice)
            {
                RequestConfiguration requestConfiguration = new RequestConfiguration();
                requestConfiguration.TestDeviceIds.Add(testDeviceIds);
                MobileAds.SetRequestConfiguration(requestConfiguration);
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            AppOpenAd.Load(id, adRequest, (AppOpenAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        tierIndex++;
                        Debug.LogError("app open ad failed to load an ad " +
                                       "with error : " + error + "load id ads tier " + tierIndex);

                        if (tierIndex <= 3)
                            LoadAppOpenAd();
                        else
                            tierIndex = 1;

                        return;
                    }

                    Debug.Log("App open ad loaded with response : "
                              + ad.GetResponseInfo());

                    tierIndex = 1;

                    // App open ads can be preloaded for up to 4 hours.
                    this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;

                    appOpenAd = ad;
                    RegisterEventHandlers(ad);
                });
        }

        /// <summary>
        /// Shows the app open ad.
        /// </summary>
        public void ShowAppOpenAd()
        {
            if (appOpenAd != null && appOpenAd.CanShowAd() )
            {
                if (!DataManager.instance.saveData.removeAds && AdsController.instance.Showing_applovin_ads == false)
                {
                    Debug.Log("Showing app open ad.");
                    appOpenAd.Show();
                }
                else
                {
                    Debug.Log("cant show because some reason");
                }


            }
            else
            {
                Debug.LogError("App open ad is not ready yet.");
            }
        }
        private void RegisterReloadHandler(AppOpenAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
        }
        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));


                OnAdRevenuePaid(adValue);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("App open ad full screen content opened.");
                Time.timeScale = 0;

                // show ads
                try
                {
                    AudioManager.Instance.StopMusic();
                    AudioManager.Instance.StopSound();
                }
                catch
                {
                    Debug.LogError("cần tắt âm thanh game khi show app open ở đây !");
                }
                

                EventController.SUM_OPENADS_ALL_GAME();
                /*EventController.SHOW_APP_OPEN_ADJUST();*/
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");
                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
                Time.timeScale = 1;

                // show ads
                //save audio setting
                //SoundController.instance.ChangeSettingMusic();
                //SoundController.instance.ChangeSettingSound();
                AudioManager.Instance.LoadVolumeSettings();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);
                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
        }
        void OnAdRevenuePaid(AdValue adValue)
        {
            AdapterResponseInfo loadedAdapterResponseInfo = appOpenAd.GetResponseInfo().
                      GetLoadedAdapterResponseInfo();

            // Solar engine
            AppImpressionAttributes impressionAttributes = new AppImpressionAttributes();
            impressionAttributes.ad_platform = loadedAdapterResponseInfo.AdSourceName;
            impressionAttributes.ad_appid = loadedAdapterResponseInfo.AdSourceId;
            impressionAttributes.ad_id = appOpenAd.GetAdUnitID();
            impressionAttributes.ad_type = 2;
            impressionAttributes.ad_ecpm = (double)adValue.Value / 1000;
            impressionAttributes.currency_type = adValue.CurrencyCode;
            impressionAttributes.mediation_platform = "Admob";//Please input the mediation platform you're using.
            impressionAttributes.is_rendered = true;
            // Please do not report custom properties starting with "_", otherwise SDK will abandon its value by default.
            SolarEngine.Analytics.trackIAI(impressionAttributes);
        }
    }
}
