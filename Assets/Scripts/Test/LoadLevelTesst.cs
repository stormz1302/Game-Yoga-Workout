using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelTesst : MonoBehaviour
{
    public Button button;
    int level = 0;
    public void LoadLevelTest()
    {
        GameManager.Instance.Level = level;
        SaveData saveData = new SaveData();
        saveData.Save();
        level += 5;
    }
}
