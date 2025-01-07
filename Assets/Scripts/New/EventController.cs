using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;

[System.Serializable]
public class RemoteDefaultInfo
{
    public string key, value;
}
public class EventController : MonoBehaviour
{
    Firebase.FirebaseApp app;
    static bool fireBaseInitDone;
    public RemoteDefaultInfo[] remoteDefault;
    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            Debug.Log("ContinueWith");
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                InitRemoteConfig();
                fireBaseInitDone = true;
                //   StartCoroutine(GetIDToken());
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

    }
    //private IEnumerator GetIDToken()
    //{
    //    System.Threading.Tasks.Task<string> t = Firebase.InstanceId.FirebaseInstanceId.DefaultInstance.GetTokenAsync();
    //    while (!t.IsCompleted) yield return new WaitForEndOfFrame();
    //    Debug.LogError("============ FirebaseID is " + t.Result);
    //}
    void InitRemoteConfig()
    {
        //Debug.Log("init remove config firebase");
        System.Collections.Generic.Dictionary<string, object> defaults =
  new System.Collections.Generic.Dictionary<string, object>();

        for (int i = 0; i < remoteDefault.Length; i++)
        {
            defaults.Add(remoteDefault[i].key, remoteDefault[i].value);
        }

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task =>
          {
              FetchDataAsync();
          });
    }
    public Task FetchDataAsync()
    {
        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            

    }
    void FetchComplete(Task fetchTask)
    {
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();

                Debug.LogError("======= fetch success:" + Firebase.RemoteConfig.FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[0].key).StringValue);
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.LogError("======= fetch error");

                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.LogError("======= fetch fail");

                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.LogError("======= fetch peding");

                break;
        }
        if (fetchTask.IsCanceled)
        {

            Debug.LogError("======= fetch cancel");
        }
        else if (fetchTask.IsFaulted)
        {

            Debug.LogError("======= fetch fault");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.LogError("======= fetch complete:" + Firebase.RemoteConfig.FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[0].key).StringValue);
        }
        DataParam.timeDelayShowAds = float.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[0].key).StringValue);
        DataParam.ShowOpenAds = Firebase.RemoteConfig.FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[1].key).StringValue == "0" ? false : true;
    }
    public static void REMOVE_ADS_FUNTION_BY_VIDEOREWARD()
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("remove_ads_para", "remove_ads_done");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("remove_ads_funtion", param);
        }
    }
    public static void SUM_VIDEO_SHOW_NAME(string value)
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("video_show_all_game_para", "video_showed_" + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "video_show_all_game", param);
        }
    }
    static string nameTempParam;
    public static void SKIP_LEVEL_NAME(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("skip_level_para", "skip_level_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("skip_level_name", param);
        }
    }
    public static void HINT_LEVEL_NAME(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("hint_level_para", "hint_level_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("hint_level_name", param);
        }
    }
    public static void SUM_INTER_ALL_GAME()
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("inter_show_all_game_para", "inter_show_all_game_value");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "inter_show_all_game", param);
        }
    }
    public static void SUM_NATIVE_ADS()
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("sum_native_ads_para", "sum_native_ads_value");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "sum_native_ads", param);
        }
    }
    public static void SUM_OPENADS_ALL_GAME()
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("openads_show_all_game_para", "openads_show_all_game_value");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "sum_app_open", param);
        }
    }
    public static void PLAY_LEVEL_EVENT(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("play_level_para", "play_level_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "play_level_event", param);
            //Debug.Log("DataParam.createLevel.version " + DataParam.createLevel.version);
            //print("play_level_" + nameTempParam + value);
            //DataParam.createLevel.version 04
        }
    }
    public static void WIN_LEVEL_EVENT(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("win_level_para", "win_level_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "win_level_event", param);
        }
    }
    public static void DIE_LEVEL_EVENT(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("die_level_para", "die_level_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "die_level_event", param);
        }
    }

    public static void DIE_LEVEL_CHECKPOINT_EVENT(int level, int CheckPoint)
    {
        if (fireBaseInitDone)
        {
            if (level < 10)
            {
                nameTempParam = "00";
            }
            else if (level >= 10 && level < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("die_lv_checkpoint_para", "die_lv_checkpoint_" + nameTempParam + level + "_" + CheckPoint);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "die_level_checkpoint_event", param);
        }
    }


    public static void CHECKPOINT_LEVEL_EVENT(int level, int CheckPoint)
    {
        if (fireBaseInitDone)
        {
            if (level < 10)
            {
                nameTempParam = "00";
            }
            else if (level >= 10 && level < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("play_lv_checkpoint_para", "play_lv_checkpoint_" + nameTempParam + level + "_" + CheckPoint);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "player_level_checkpoint_event", param);
        }
    }

    public static void TUTORIAL_EVENT(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("tutorial_para", "tutorial_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "tutorial_event", param);
        }
    }

    public static void LOSE_LEVEL_EVENT(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("lose_level_para", "lose_level_" + nameTempParam + value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(DataParam.createLevel.version + "_" + "lose_level_event", param);
        }
    }
    public static void REMOVE_ADS(int value)
    {
        if (fireBaseInitDone)
        {
            if (value < 10)
            {
                nameTempParam = "00";
            }
            else if (value >= 10 && value < 100)
            {
                nameTempParam = "0";
            }
            else
            {
                nameTempParam = "";
            }
            Parameter param = new Parameter("remove_ads_para", "removeads");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("remove_ads", param);
        }
    }
    public static void FLOW_FIRST_OPEN(string value, string nameScene)//chua
    {
        if (fireBaseInitDone)
        {
            if (nameScene == "Home")
            {
                Parameter param = new Parameter("flow_first_open_para", "flow_menu_" + value);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("flow_first_open", param);
                Debug.LogError("flow_menu_" + value);
            }
            else if (nameScene == "Map")
            {
                Parameter param = new Parameter("flow_first_open_para", "flow_map_" + value);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("flow_first_open", param);
                Debug.LogError("flow_map_" + value);
            }
            else if (nameScene == "Gameplay")
            {
                Parameter param = new Parameter("flow_first_open_para", "flow_gameplay_" + value);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("flow_first_open", param);
                Debug.LogError("flow_gameplay_" + value);
            }


        }
    }
    public static void FOLLOW_BUTTON(string value, string nameScene)
    {
        if (fireBaseInitDone)
        {
            if (nameScene == "Home")
            {
                Parameter param = new Parameter("mainmenu_para", "mainmenu_" + value + "_click");
                Firebase.Analytics.FirebaseAnalytics.LogEvent("mainmenu_click", param);
                Debug.LogError("Menu_" + "mainmenu_" + value + "_click");
            }
            else if (nameScene == "Map")
            {
                Parameter param = new Parameter("map_para", "map_" + value + "_click");
                Firebase.Analytics.FirebaseAnalytics.LogEvent("map_click", param);
                Debug.LogError("Map_" + "map_" + value + "_click");
            }
            else if (nameScene == "Gameplay")
            {
                Parameter param = new Parameter("gameplay_para", "gameplay_" + value + "_click");
                Firebase.Analytics.FirebaseAnalytics.LogEvent("game_play", param);
                Debug.LogError("Play_" + "gameplay_" + value + "_click");
            }
        }
    }
    public static void MORE_GAME()
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("more_game_para", "more_game_click");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("more_game", param);
        }
    }
    public static void INTRO_EVENT(string value)
    {
        if (fireBaseInitDone)
        {
            Parameter param = new Parameter("intro_para", value);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("intro_event", param);
        }
    }

}
