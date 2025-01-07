using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DataParam
{
    public static string packBuyIAP;
    public static string SAVEDATA = "savedata";
    public static DateTime beginShowInter, lastShowInter;
    public static float timeDelayShowAds = 25;                                        // test
    public static bool ShowOpenAds;

    public static int countShowInter;
    public static int levelHintOrSkip;
    public static int countShowVideo;
    
    
    static string path;
    static TextAsset textAsset;
    public static CreateLevel createLevel = new CreateLevel();
    public static string wwwLevel;
    static JsonData json;
    static bool jsonError = false;

    public static string HomeScene = "Home";
    public static string SelectLevelScene = "SelectLevel";
    public static string Gameplay = "Gameplay";

    public static void LoadInfoLevel()
    {
        if (!string.IsNullOrEmpty(wwwLevel) && wwwLevel != "" && wwwLevel != "[]")
        {
            try
            {
                json = JsonMapper.ToObject(wwwLevel.ToString());
                Debug.Log("json: " + json);
            }
            catch
            {
                jsonError = true;
            }

            if (jsonError)
            {
                Debug.LogError("loi");
                ReadFromLocal();
            }
            else
            {
                Debug.LogError("ko  loi");
                createLevel = JsonMapper.ToObject<CreateLevel>(json.ToJson());
                loaddonelevel = true;
            }
        }
        else
        {
            //Debug.LogError("=======thieu text asset");
            ReadFromLocal();
        }
    }

    public static bool loaddonelevel;
    static void ReadFromLocal()
    {
        path = "Level";
        textAsset = Resources.Load<TextAsset>("TextAsset/" + path);
        if (textAsset == null)
            return;
        path = textAsset.ToString();

        if (!string.IsNullOrEmpty(path) && path != "" && path != "[]")
        {

            createLevel = JsonMapper.ToObject<CreateLevel>(path);
        }

        //createLevel.version = "ver" + Application.version;
        Debug.LogError("=======Load info level from local");
        Debug.LogError("======= json local:  " + path);
        loaddonelevel = true;
    }
    public static void FirstInitData()
    {
        if (DataManager.instance.saveData.currentLevel == 0)
        {
            DataManager.instance.saveData.currentLevel = 1;
        }
        if (DataManager.instance.saveData.highLevel == 0)
        {
            DataManager.instance.saveData.highLevel = 1;
        }
        if (DataManager.instance.saveData.highChapter == 0)
        {
            DataManager.instance.saveData.highChapter = 1;
        }
        if (DataManager.instance.saveData.currentChapter == 0)
        {
            DataManager.instance.saveData.currentChapter = 1;
        }

    }
    /*private int GetIndexLoadLevelFromJson(int indexDisplay)
    {
        int SaveCount = 0;
        while (SaveCount < 1000)
        {
            var LoadIndex = DataParam.createLevel.info[indexDisplay - 1].prefab;
            if (IsResourceAvailable("Level/level" + LoadIndex))
            {
                // Debug.Log("display: " + indexDisplay + "   loadIndex: " + LoadIndex);
                return LoadIndex;
            }
            else
            {
                indexDisplay++;
                if (indexDisplay > DataParam.createLevel.info.Count)
                {
                    Debug.LogError("tim prefab level khong thay => quay ve 1");
                    indexDisplay = 1;
                }
                Debug.LogError("khong thay prefab: " + LoadIndex + "  , tiếp tục next để tìm");
                SaveCount++;
            }

        }
        //  không tìm thấy resource sau 1000 lần thử => cho load level 1
        Debug.LogError("không tìm thấy resource sau 1000 lần thử => cho load level 1");
        return 1;
    }*/
    /*private bool IsResourceAvailable(string path)
    {
        var Object_ = Resources.Load<LevelController>(path);
        return Object_ != null;
    }*/
}

