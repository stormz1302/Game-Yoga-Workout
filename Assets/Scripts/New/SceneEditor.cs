#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneEditor : EditorWindow
{
    public bool runWhenClick;
    [MenuItem("Window/SceneEditor")]
    public static void ShowWindow()
    {
        GetWindow<SceneEditor>("SceneEditor");
    }
    private void OnGUI()
    {
        //GUILayout.Label("All Scene.", EditorStyles.boldLabel);
        //if (GUILayout.Button("Menu"))
        //{
        //    EditorSceneManager.OpenScene("Bac_UI/Scenes/Menu");
        //}
        //if (GUILayout.Button("TestScene"))
        //{
        //    EditorSceneManager.OpenScene("DungTP/Scenes/SampleScene");
        //}
        //if (GUILayout.Button("Gameplay"))
        //{
        //    EditorSceneManager.OpenScene("DungTP/Scenes/GamePlay");
        //}
        runWhenClick = GUILayout.Toggle(runWhenClick, "Run When Click");

        var scenes = EditorBuildSettings.scenes;

        for (int i = 0; i < scenes.Length; i++)
        {
            if (GUILayout.Button(scenes[i].path))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(scenes[i].path);

                if (runWhenClick)
                {
                    EditorApplication.isPlaying = true;
                }
            }
        }
    }
}
#endif
