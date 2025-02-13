using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestButton : MonoBehaviour
{
    public List<GameObject> uiObjects;
    private List<GameObject> uiObjects2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClickHide()
    {

        foreach (var uiObject in uiObjects)
        {
            uiObject.SetActive(false);
        }
    }
    public void OnClickShow()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        foreach (var uiObject in uiObjects)
        {
            if (uiObject.name == "LevelText") uiObject.SetActive(true);
            if (sceneName == "Home" && uiObject.name == "Menu")
            {
                uiObject.SetActive(true);
            }
            else if(sceneName == "Level01" && uiObject.name == "PlayScreen")
            {
                uiObject.SetActive(true);
            }
        }
    }

    public void OnClickMoney()
    {
        int money = PlayerPrefs.GetInt("Money", 0);
        if (money <= 1000000)
        {
            PlayerPrefs.SetInt("Money", 1000000);
        }
        SaveData saveData = new SaveData();
        saveData.LoadMoney();
    }
}
