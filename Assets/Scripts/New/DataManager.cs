using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class SaveDatas
{
    public bool offmusic, offsound, offvibra, removeAds, showIntro, rated, requestLoadLevel, FirstTimeInGame, showTutorial, showStory, showButtonHint;
    public int currentLevel, session, highLevel, currentChapter, highChapter, Coin;

}

public class DataManager : MonoBehaviour
{
    public SaveDatas saveData;
    public static DataManager instance;
    public string urlLevelAndroid, urlLevelIOS;

    string urlLevel;

    UnityWebRequest wwwLevel;
    private void Start()
    {

#if UNITY_IOS
        urlLevel = urlLevelIOS;
#else
        urlLevel = urlLevelAndroid;
#endif
        wwwLevel = UnityWebRequest.Get(urlLevel);
        StartCoroutine(WaitForRequestLevel(wwwLevel));
        //StartCoroutine(GetRequest(urlLevel));
    }

    IEnumerator WaitForRequestLevel(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            DataParam.wwwLevel = www.downloadHandler.text;
            Debug.LogError("=====WWW Level!: " + www.downloadHandler.text);
        }


    }
    /*IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("==== "+ pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("==== " + pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("==== " + pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    DataParam.wwwLevel = webRequest.downloadHandler.text;
                    break;
            }
        }
    }*/


    private void Awake()
    {
        if (instance == null)
        {
            Application.targetFrameRate = 300;
            //Input.multiTouchEnabled = false;
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    void LoadAllData()
    {
        saveData = new SaveDatas();
        string jsonSaved = string.Empty;
        jsonSaved = PlayerPrefs.GetString(DataParam.SAVEDATA);
        if (!string.IsNullOrEmpty(jsonSaved) && jsonSaved != "" && jsonSaved != "[]")
        {
            var jData = JsonMapper.ToObject(jsonSaved);
            if (jData != null)
            {
                saveData = JsonMapper.ToObject<SaveDatas>(jData.ToJson());
            }
        }
        DataParam.beginShowInter = DataParam.lastShowInter = System.DateTime.Now;

        saveData.session++;
    }

    void SaveData()
    {
        var tempsaveData = JsonMapper.ToJson(saveData);
        PlayerPrefs.SetString(DataParam.SAVEDATA, tempsaveData);
        PlayerPrefs.Save();
        //Debug.LogError("save");
    }
    void OnApplicationQuit()
    {
        SaveData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }
    public void RemoveAdsFunc()
    {
        saveData.removeAds = true;
        //GameController.gameController.settingPopUp.DisplayBtn();
        AdsController.instance.HideBanner();
        AdsController.instance.HideMrec();
    }
    
}
