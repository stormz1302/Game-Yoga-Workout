using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts : Editor
{
    [MenuItem("Tools/Cleanup/Remove Missing Scripts From Prefabs")]
    public static void RemoveMissingScriptss()
    {
        // Tìm tất cả Prefab trong thư mục Assets
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        int prefabCount = 0;
        int missingScriptCount = 0;

        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                int removedScripts = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);

                if (removedScripts > 0)
                {
                    Debug.Log($"Removed {removedScripts} missing scripts from Prefab: {prefab.name} at {prefabPath}");
                    missingScriptCount += removedScripts;

                    // Đánh dấu Prefab đã chỉnh sửa
                    EditorUtility.SetDirty(prefab);
                    prefabCount++;
                }
            }
        }

        // Lưu thay đổi và làm mới
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Removed {missingScriptCount} missing scripts from {prefabCount} Prefabs.");
    }
}
