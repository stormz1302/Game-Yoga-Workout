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
    [SerializeField] Sprite[] soundIcon;
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
    [SerializeField] private GameObject scoreBar;

    [Header("ComboPopupUI:")]
    [SerializeField] private List<GameObject> comboPopupUI;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 1.5f;

    [Header("EndGame Popup:")]
    [SerializeField] private GameObject endGamePopup;
    [SerializeField] private TMP_Text bonusMoneyRewardText;
    [SerializeField] private TMP_Text levelEndGameText;
    [SerializeField] private TMP_Text levelState;
    //[SerializeField] private Button nextLevel;
    //[SerializeField] private Button Ads;
    [SerializeField] private Button homeButton;


    private Coroutine currentCoroutine;

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
        //playButtonAnimator = playButton.GetComponent<Animator>();
        //playButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        shopButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        isOnMusic = true;
        isOnSound = true;
        endGamePopup.SetActive(false);

    }
    public void UpdateLevel(int level)
    {
        levelText.text = (level + 1).ToString();
    }

    public void ReadyScreen()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        Menu.SetActive(false);
        playScreen.SetActive(true);
        playButton.SetActive(true);
        scoreBar.SetActive(false);
    }

    public void LoadNextLevel()
    {
        AudioManager.Instance.PlaySound("PlayButton");
        GameManager.Instance.ReStart();
    }

    public void StartGame()
    {
        scoreBar.SetActive(true);
        playButton.SetActive(false);
        AudioManager.Instance.PlaySound("PlayButton");
        GameManager.Instance.StartGame(); 
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
                SoundButton.image.sprite = soundIcon[1];
                isOnSound = false;
                AudioManager.Instance.SetVolume("Sounds", false);
            }
            else
            {
                SoundButton.image.sprite = soundIcon[0];
                isOnSound = true;
                AudioManager.Instance.SetVolume("Sounds", true);
            }
        }
        else
        {
            if (isOnMusic)
            {
                MusicButton.image.sprite = soundIcon[3];
                isOnMusic = false;
                AudioManager.Instance.SetVolume("Music", false);
            }
            else
            {
                MusicButton.image.sprite = soundIcon[2];
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
        bonusMoneyText.text = value.ToString("N0");
    }

    public void UpdateMoney(int value)
    {
        moneyText.text = value.ToString("N0");
    }

    public void UpdateComboPopupUI(int comboIndex)
    {
        if (comboIndex < 0 || comboIndex >= comboPopupUI.Count)
        {
            Debug.LogWarning("Invalid combo index!");
            return;
        }
        GameObject popup = comboPopupUI[comboIndex];

        if (popup == null)
        {
            Debug.LogWarning("Popup at this index is missing!");
            return;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        AudioManager.Instance.PlaySound("Combo");
        currentCoroutine = StartCoroutine(ShowPopupEffect(popup));
    }

    private IEnumerator ShowPopupEffect(GameObject popup)
    {
        Image image = popup.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Popup does not have an Image component!");
            yield break;
        }

        Color originalColor = image.color;
        popup.SetActive(true);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

        yield return new WaitForSeconds(displayDuration);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = 1 - (t / fadeDuration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        popup.SetActive(false);
    }

    public void ShowPopup(int level, int rewardAmount, bool isWin)
    {
        endGamePopup.SetActive(true);
        if (isWin) levelState.text = "Level complete";
        else
        {
            levelState.text = "Level failed";
            //nextLevel.gameObject.SetActive(false);
        }
        levelEndGameText.text = "Level " + level;
        GachaUIController gachaUIController = FindObjectOfType<GachaUIController>();
        gachaUIController.OnLoadEndGamePopup();
        StartCoroutine(UpdateBonusMoney(rewardAmount));

    }

    private IEnumerator UpdateBonusMoney(int targetAmount)
    {
        int currentAmount = 0;
        float duration = 1.5f; 
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentAmount = Mathf.RoundToInt(Mathf.Lerp(0, targetAmount, elapsed / duration));
            bonusMoneyRewardText.text = currentAmount.ToString("N0");
            yield return null;
        }
        bonusMoneyRewardText.text = targetAmount.ToString("N0");
    }

    public void PauseButtonUnActive()
    {
        pauseButton.SetActive(false);
    }
}

