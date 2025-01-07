using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Samples.Purchasing.Core.BuyingConsumables;

public class CanvasLv1 : MonoBehaviour
{
    [Header("Menu:")]
    [SerializeField] GameObject playScreen;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] private Image fadeImage;
    [SerializeField] GameObject Menu;
    Animator playButtonAnimator;
    [SerializeField] private GameObject rewardSlider;

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
    //[SerializeField] private GameObject CameraShop;

    [Header("Pause:")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button soundButtonInPause;
    [SerializeField] private Button musicButtonInPause;

    [Header("PlayScreen:")]
    [SerializeField] private TMP_Text bonusMoneyText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject scoreBar;
    bool lvelBonus = false;

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

    [Header("Ads Popup:")]
    [SerializeField] private GameObject adsPopup;
    [SerializeField] private GameObject CotinuePopp;

    [Header("Lucky Spin:")]
    [SerializeField] private GameObject luckySpinPop;
    [Header("No Ads:")]
    [SerializeField] private GameObject noAdsPopp;
    [SerializeField] private Button noAdsButton;

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
        isOnMusic = true;
        isOnSound = true;
        endGamePopup.SetActive(false);
        CheckRemoveAds();
        StartCoroutine(FadeIn());
    }

    public void LoadLevelUI(bool levelBonus)
    {
        lvelBonus = levelBonus;
    }
    public void UpdateLevel(int level, bool levelBonus)
    {
        if (levelBonus)
        {
            levelText.text = "Level Bonus";
            rewardSlider.SetActive(false);
        }
        else
        {
            levelText.text = "Level " + (level + 1).ToString();
            rewardSlider.SetActive(true);
        }
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
        scoreBar.SetActive(!lvelBonus);
        playButton.SetActive(false);
        AudioManager.Instance.PlaySound("PlayButton");
        GameManager.Instance.StartGame();
    }

    public void OpenSetting()
    {
        AudioManager.Instance.PlaySound("MenuOpen");
        setting.SetActive(true);
        AdsController.instance.ShowMrecCentered();
    }
    public void CloseSetting()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        setting.SetActive(false);
        AdsController.instance.HideMrec();
    }

    public void OnClickSoundButton(bool Music)
    {
        if (!Music)
        {
            if (isOnSound)
            {
                SoundButton.image.sprite = soundIcon[1];
                soundButtonInPause.image.sprite = soundIcon[1];
                isOnSound = false;
                AudioManager.Instance.SetVolume("Sounds", false);
            }
            else
            {
                SoundButton.image.sprite = soundIcon[0];
                soundButtonInPause.image.sprite = soundIcon[0];
                isOnSound = true;
                AudioManager.Instance.SetVolume("Sounds", true);
            }
        }
        else
        {
            if (isOnMusic)
            {
                MusicButton.image.sprite = soundIcon[3];
                musicButtonInPause.image.sprite = soundIcon[3];
                isOnMusic = false;
                AudioManager.Instance.SetVolume("Music", false);
            }
            else
            {
                MusicButton.image.sprite = soundIcon[2];
                musicButtonInPause.image.sprite = soundIcon[2];
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
        //CameraShop.SetActive(true);
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
        //CameraShop.SetActive(false);
        settingButton.SetActive(true);
        SkinsManager.instance.LoadCharacterData();
        StartCoroutine(FadeIn());
        AdsController.instance.ShowInter();
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
        AdsController.instance.ShowMrecCentered();
        pauseButton.SetActive(false);
    }

    public void Continue()
    {
        AudioManager.Instance.PlaySound("MenuClose");
        GameManager.Instance.ContinueGame();
        AdsController.instance.HideMrec();
        pauseScreen.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void Home()
    {
        AdsController.instance.HideMrec();
        AudioManager.Instance.PlaySound("MenuClose");
        pauseButton.SetActive(true);
        pauseScreen.SetActive(false);
        Menu.SetActive(true);
        playScreen.SetActive(false);
        GameManager.Instance.home();
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
        AdsController.instance.ShowMrecBottomCenter();
        endGamePopup.SetActive(true);
        if (isWin) levelState.text = "Level complete";
        else
        {
            levelState.text = "Level failed";
            //nextLevel.gameObject.SetActive(false);
        }
        levelEndGameText.text = "Level " + (level + 1f).ToString();
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

    private IEnumerator FadeIn()
    {
        if (fadeImage == null)
        {
            Debug.LogError("fadeImage null.");
            yield break;
        }
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        Color originalColor = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        fadeImage.gameObject.SetActive(false);
    }

    public void ShowAdsPopup()
    {
        adsPopup.SetActive(true);
        AdsController.instance.ShowMrecBottomCenter();
    }
    public void CloseAdsPopup()
    {
        Time.timeScale = 1;
        AdsController.instance.HideMrec();
        adsPopup.SetActive(false);
    }

    public void ShowNoAdsPopp()
    {
        noAdsPopp.SetActive(true);
    }
    public void CloseNoAdsPopp()
    {
        noAdsPopp.SetActive(false);
    }

    public void ShowLuckySpinPop()
    {
        luckySpinPop.SetActive(true);
        AudioManager.Instance.PlaySound("MenuOpen");
    }
    public void CloseLuckySpinPop()
    {
        luckySpinPop.SetActive(false);
        AudioManager.Instance.PlaySound("MenuClose");
    }

    public void ShowContinuePop()
    {
        CotinuePopp.SetActive(true);
        AudioManager.Instance.PlaySound("MenuOpen");
        AdsController.instance.ShowMrecBottomCenter();
    }

    public void BuyRemoveAds()
    {
        ShopPurchase.instance.BuyRemoveAds();
        CloseNoAdsPopp();
        noAdsButton.interactable = false;
    }

    private void CheckRemoveAds()
    {
        if (DataManager.instance.saveData.removeAds)
        {
            noAdsButton.interactable = false;
        }
    }
}

