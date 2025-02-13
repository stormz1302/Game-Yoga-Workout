using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static CreateLevel;

[System.Serializable]
public class CreateLevel
{
    public string version = "v03";
    public List<InfoCreateLevel> info = new List<InfoCreateLevel>();
    [System.Serializable]
    public class InfoCreateLevel
    {
        public int level;
        public int prefab;
    }
}
public class CreateJsonForLevel : MonoBehaviour
{
    TextAsset textAsset;
    public int totalLevel;
    public CreateLevel createLevel = new CreateLevel();
    JsonData jData;
    //private void Start()
    //{
    //    Load();
    //}
    string path;
    void Load()
    {
        path = "Level";
        /*#if UNITY_ANDROID
                path = "LevelAndroid";
        #elif UNITY_IOS
             path = "LevelIOS";
        #endif*/
        textAsset = Resources.Load<TextAsset>("TextAsset/" + path);
        if (textAsset == null)
            return;
        tempCreateLevel = textAsset.ToString();
        if (!string.IsNullOrEmpty(tempCreateLevel) && tempCreateLevel != "" && tempCreateLevel != "[]")
        {
            createLevel = JsonMapper.ToObject<CreateLevel>(tempCreateLevel);
        }
    }
    string tempCreateLevel;
    public void CreateLevel()
    {
        Load();
        //createLevel.info.Clear();
        Debug.Log("total lv: " + totalLevel);
        for (int i = createLevel.info.Count; i < totalLevel; i++)
        {
            InfoCreateLevel _infoCreateLevel = new InfoCreateLevel();
            _infoCreateLevel.level = (i + 1);
            _infoCreateLevel.prefab = (i + 1);
            createLevel.info.Add(_infoCreateLevel);
        }
        tempCreateLevel = JsonMapper.ToJson(createLevel);
        Debug.LogError(tempCreateLevel);
        Debug.LogError("create lv++++");
    }
    public void ClearLevel()
    {
        createLevel.info.Clear();
    }


}
#if UNITY_EDITOR
[CustomEditor(typeof(CreateJsonForLevel))]
public class CreateLevelEditor : Editor
{
    public bool isCheck;
    private CreateJsonForLevel myScript;

    private void OnSceneGUI()
    {
        myScript = (CreateJsonForLevel)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (myScript == null)
            myScript = (CreateJsonForLevel)target;
        GUIStyle SectionNameStyle = new GUIStyle();
        SectionNameStyle.fontSize = 16;
        SectionNameStyle.normal.textColor = Color.blue;

        if (myScript == null) return;

        EditorGUILayout.LabelField("----------\t----------\t----------\t----------\t----------", SectionNameStyle);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            if (GUILayout.Button("Create", GUILayout.Height(50)))
            {
                isCheck = true;
                myScript.CreateLevel();
            }
        }
        EditorGUILayout.EndVertical();
        if (isCheck)
        {
            EditorUtility.SetDirty(myScript);
            isCheck = false;
        }
        EditorGUILayout.LabelField("----------\t----------\t----------\t----------\t----------", SectionNameStyle);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            if (GUILayout.Button("Clear", GUILayout.Height(50)))
            {
                isCheck = true;
                myScript.ClearLevel();
            }
        }
        EditorGUILayout.EndVertical();
        if (isCheck)
        {
            EditorUtility.SetDirty(myScript);
            isCheck = false;
        }
        //
    }
}
#endif

