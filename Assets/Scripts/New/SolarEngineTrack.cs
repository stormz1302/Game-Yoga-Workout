using SolarEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarEngineTrack : MonoBehaviour
{
    public static SolarEngineTrack Instance;
    public string appKeyAndroid;
    public string appKeyiOS;
    string appKey;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(WaitInit());
    }
    public void InitSDK()
    {

#if UNITY_ANDROID
        appKey = appKeyAndroid;
#elif UNITY_IOS
        appKey = appKeyiOS;
#endif


        //Perform initialization, taking (not integrating online parameter SDK) as an example. (Integrating Online Parameter SDK) callback method is the same as this method.
        SolarEngine.Analytics.preInitSeSdk(appKey);    //Pre-Init must be performed.
        SEConfig seConfig = new SEConfig();
        seConfig.logEnabled = true;
        seConfig.initCompletedCallback = onInitCallback;     //Put initialization callback in the SEConfig.
        SolarEngine.Analytics.initSeSdk(appKey, seConfig);

        //Initialization callback received
        


    }
    private void onInitCallback(int code)
    {
        ///Please refer to the callback codes table below.

        Debug.Log("Solar Engine init successed with appkey: "+ appKey);
    }
    private void attSuccessCallback(int errorCode, Dictionary<string, object> attribution)
    {

        if (errorCode != 0)
        {
            Debug.Log("SEUnity: errorCode : " + errorCode);

        }
        else
        {

            Debug.Log("SEUnity: attSuccessCallback : " + attribution);

        }
    }
    private void initSuccessCallback(int code)
    {
        Debug.Log("SEUnity:initSuccessCallback  code : " + code);

    }
    public void TestEvent()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("K1", "V1");
        dict.Add("K2", "V2");
        dict.Add("K3", 2);
        SolarEngine.Analytics.setSuperProperties(dict);
        Debug.LogWarning("set event solar");
    }
    IEnumerator WaitInit()
    {
        yield return new WaitForSeconds(0.5f);
        InitSDK();
    }
}
