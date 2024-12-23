using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    private int money;
    private int level;
    private List<int> skinOwnedIndex;

    public void LoadMoney()
    {
        money = PlayerPrefs.GetInt("Money", 0);
        GameManager.Instance.money = money;
        CanvasLv1.Instance.UpdateMoney(money);
    }
    public void LoadLevel()
    {
        level = PlayerPrefs.GetInt("Level", 0);
        GameManager.Instance.Level = level;
    }

    public void Save()
    {
        level = GameManager.Instance.Level;
        money = GameManager.Instance.money;
        money += GameManager.Instance.bonusMoney;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.Save();
    }

    public void SetLevelBonus(bool levelBonus)
    {
        PlayerPrefs.SetInt("LevelBonus", levelBonus ? 1 : 0);
    }

    public bool GetLevelBonus()
    { 
        return PlayerPrefs.GetInt("LevelBonus", 0) == 1;
    }
}
