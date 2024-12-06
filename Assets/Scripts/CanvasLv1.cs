using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CanvasLv1 : MonoBehaviour
{
    [Header("Menu:")]
    [SerializeField] GameObject playScreen;
    [SerializeField] TMP_Text moneyText;

    [SerializeField] GameObject Menu;
    Animator playButtonAnimator;

    [Header("Setting:")]
    [SerializeField] GameObject settingButton;
    [SerializeField] GameObject setting;
    [SerializeField] Sprite[] soundOn;
    [SerializeField] Button MusicButton;
    [SerializeField] Button SoundButton;
    bool isOnMusic = true;
    bool isOnSound = true;

    [Header("Shop:")]
    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject shop;
    [HideInInspector] public bool shopOpening = false;

    [Header("Pause:")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseScreen;

    [Header("PlayScreen:")]
    [SerializeField] private TMP_Text bonusMoneyText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject playButton;

    public static CanvasLv1 Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playScreen.SetActive(false);
        Menu.SetActive(true);
        playButtonAnimator = playButton.GetComponent<Animator>();
        playButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        shopButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        isOnMusic = true;
        isOnSound = true;

    }
    public void UpdateLevel(int level)
    {
        levelText.text = "Level " + (level + 1);
    }

    public void ReadyScreen()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        Menu.SetActive(false);
        playScreen.SetActive(true);
        playButton.SetActive(true);
    }
    public void StartGame()
    {
        AudioManager.Instance.PlaySound("PlayButton");
        GameManager.Instance.StartGame(); 
        playButton.SetActive(false);
    }

    public void OpenSetting()
    {
        AudioManager.Instance.PlaySound("MenuOpen");
        setting.SetActive(true);
    }
    public void CloseSetting()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        setting.SetActive(false);
    }

    public void OnClickSoundButton(bool Music)
    {
        if (!Music)
        {
            if (isOnSound)
            {
                SoundButton.image.sprite = soundOn[1];
                isOnSound = false;
                AudioManager.Instance.SetVolume("Sounds", false);
            }
            else
            {
                SoundButton.image.sprite = soundOn[0];
                isOnSound = true;
                AudioManager.Instance.SetVolume("Sounds", true);
            }
        }
        else
        {
            if (isOnMusic)
            {
                MusicButton.image.sprite = soundOn[1];
                isOnMusic = false;
                AudioManager.Instance.SetVolume("Music", false);
            }
            else
            {
                MusicButton.image.sprite = soundOn[0];
                isOnMusic = true;
                AudioManager.Instance.SetVolume("Music", true);
            }
        }
    }

    public void OpenShop()
    {
        shopOpening = true;
        AudioManager.Instance.PlaySound("MenuOpen");
        shop.SetActive(true);
        playButton.SetActive(false);
        settingButton.SetActive(false);
        playScreen.SetActive(false);
    }

    public void CloseShop()
    {
        shopOpening = false;
        AudioManager.Instance.PlaySound("MenuClose");
        shop.SetActive(false);
        playButton.SetActive(true);
        settingButton.SetActive(true);
        SkinsManager.instance.LoadCharacterData();
    }

    public void OnClickReady()
    {
        AudioManager.Instance.PlaySound("PlayButton");
        GameManager.Instance.LoadPlayScene();
    }

    public void PauseGame()
    {
        AudioManager.Instance.PlaySound("MenuOpen");
        GameManager.Instance.PauseGame();
        pauseScreen.SetActive(true);
        pauseButton.SetActive(false);
    }
    
    public void Continue()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        GameManager.Instance.ContinueGame();
        pauseScreen.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void Home()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        pauseButton.SetActive(true);
        GameManager.Instance.home();
        pauseScreen.SetActive(false);
        Menu.SetActive(true);
        playScreen.SetActive(false);
    }

    public void ReStart()
    {
        GameManager.Instance.ReStart();
        pauseScreen.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void UpdateMoneyInPlay(int value)
    {
        bonusMoneyText.text = value.ToString();
    }

    public void UpdateMoney(int value)
    {
        moneyText.text = value.ToString();
    }
}
