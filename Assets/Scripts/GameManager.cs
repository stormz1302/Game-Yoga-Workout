using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Sample;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Gameplay")]
    public float score = 0;
    public int maxScore;
    public int maxObject;
    public MissionListSO MissionListSO;
    public MissionListSO MissionListSOBegin;
    public BonusMission bonusMission;
    public List<GameObject> missions = new List<GameObject>();
    public Mission missionLevels ;
    public int bonusMoney = 0;
    public int Level = 0;
    public int animIndex = 0;
    public GameObject bot;
    public bool endGame = false;
    AnimationFrameChecker animationFrameChecker;
    bool levelBonus = false;
    public int bonusCount;
    public float bonusTime;
    List<string> animName = new List<string>();

    [Header("Main")]
    public int money;
   
    [Header("Player")]
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer dress;
    public SkinnedMeshRenderer pants;
    public SkinnedMeshRenderer shoes;
    public SkinnedMeshRenderer hands;
    private float currentBlendShapeValue = 60f;
    public Transform player;
    Vector3 characterTrf;
    Quaternion characterRot;
    ScoreUI scoreUI;

    public bool canDrag;
    int _score;
    int selectedSkinID;
    SaveData saveData;
    EndGame EndGame;
    [SerializeField] int combo = 0;
    [SerializeField] List<int> comboList = new List<int>();
    int comboIndex = 0;
    public static GameManager Instance;
    bool openGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
            
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        openGame = true;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //==========Set full money (test shop)===============================================================
        money = PlayerPrefs.GetInt("Money");
        if (money <= 100000)
            PlayerPrefs.SetInt("Money", 200000);
        //===================================================================================================
        AdsController.instance.StartCoroutineBanner();
        AppOpenAdController.instance.ShowAppOpenFirstLoadingFinish();
        animationFrameChecker = GetComponent<AnimationFrameChecker>();
        saveData = new SaveData();
        levelBonus = saveData.GetLevelBonus();
        if (scene.name == "Level01")
        {
            selectedSkinID = Shop.instance.equipedSkinID;
            endGame = false;
            bonusMoney = 0;
            _score = 0;
            saveData.LoadMoney();
            CanvasLv1.Instance.ReadyScreen();
            //UpdateScore(0);
            animName = FindObjectOfType<PlayerController>().animationName;
            LoadAnim();
            Ready();
        }
        if (scene.name == "Home")
        {
            LoadHomeScene();
        }
        AchievementReward achievementReward = FindObjectOfType<AchievementReward>();
        if (achievementReward != null) achievementReward.UpdateRewardBar();

    }

    private void LoadModelInPlay()
    {
        GameObject character = Instantiate(Shop.instance.Skins[selectedSkinID].CharacterPrf);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.animator = character.GetComponent<Animator>();
        character.transform.SetParent(player);
        character.transform.SetSiblingIndex(0);
        characterTrf = character.transform.position;
        characterRot = character.transform.rotation;
        character.GetComponent<Animator>().Play(animName[animIndex],0);
        //character.transform.position = player.position;
        //character.transform.rotation = Quaternion.Euler(0, 0, 0);

    }
    public void SetBlendShape(float value)
    {
        if (body != null) body.SetBlendShapeWeight(0, value);
        if (dress != null) dress.SetBlendShapeWeight(0, value);
        if (pants != null) pants.SetBlendShapeWeight(0, value);
        if (shoes != null) shoes.SetBlendShapeWeight(0, value);
        if (hands != null) hands.SetBlendShapeWeight(0, value);
    }

    private void LoadAnim()
    {
        animIndex = Level;
        int level = Level + 1;
        if (!levelBonus)
        {
            if (level <= 5)
            {
                SetDifficultyLevel(DifficultyLevel.Beginner);
                missions = MissionListSOBegin.missionLevels[animIndex].missionsHome;
                LoadBG.Instance.LoadMap(0);
            }
            else if (5 < level && level <= 15)
            {
                SetDifficultyLevel(DifficultyLevel.Novice);
                missions = missionLevels.missionsIce;
                LoadBG.Instance.LoadMap(1);
            }
            else if (15 < level && level <= 25)
            {
                SetDifficultyLevel(DifficultyLevel.Intermediate);
                missions = missionLevels.missionsHLW;
                LoadBG.Instance.LoadMap(2);
            }
            else if (25 < level && level <= 35)
            {
                SetDifficultyLevel(DifficultyLevel.Advanced);
                missions = missionLevels.missionsJapan;
                LoadBG.Instance.LoadMap(3);
            }
            else if (35 < level && level <= 45)
            {
                SetDifficultyLevel(DifficultyLevel.Expert);
                missions = missionLevels.missionsHome;
                LoadBG.Instance.LoadMap(0);
            }
            else if (45 < level)
            {
                SetDifficultyLevel(DifficultyLevel.Master);
                missions = RandomMissions();
                LoadBG.Instance.LoadMap(4);
            }
        }
        else
        {
            missions = bonusMission.missions;
            bonusCount = bonusMission.missionCount;
            bonusTime = bonusMission.missionTime;
            animIndex = 1;
        }
        
    }

    private void SetDifficultyLevel(DifficultyLevel difficultyLevel)
    {
        animIndex = Random.Range(0, 5);
        if(difficultyLevel == DifficultyLevel.Beginner) animIndex = Level;
        missionLevels = MissionListSO.missionLevels[animIndex];
        missionLevels.SetDifficulty(difficultyLevel);
        maxObject = missionLevels.maxObject;
        //missions = missionLevels.missions;
    }

    private List<GameObject> RandomMissions()
    {
        int type = Random.Range(0, 4);
        switch (type)
        {
            case 0:
                return missionLevels.missionsHome;
            case 1:
                return missionLevels.missionsIce;
            case 2:
                return missionLevels.missionsHLW;
            case 3:
                return missionLevels.missionsJapan;
            default:
                return missionLevels.missionsHome;
        }
    }

    public void AddScore(bool good, int value)
    {
        Debug.Log("AddScore");
        if (!levelBonus && good)
        {
            score += value;
            Debug.Log("value: " + value);
            scoreUI.IncreaseScore(score);
            AddCombo(true);
            _score = Mathf.FloorToInt(maxScore * 0.6f);
            if (score >= _score)
            {
                if (_score > 0)
                {
                    PlayerController playerController = FindObjectOfType<PlayerController>();
                    if (playerController != null) playerController.GoodEffect();
                    AudioManager.Instance.PlaySound("Transform");
                    currentBlendShapeValue = Mathf.Clamp(currentBlendShapeValue - 50f, 0f, 100f);
                }
                _score = maxScore;
            }
            SetBlendShape(currentBlendShapeValue);
            //UpdateScore(score);
            if (score >= maxScore)
            {
                score = maxScore;
                LoadEndScreen();
            }
        }
        else if (!levelBonus && !good && score > 0) 
        {
            AddCombo(false);
            score -= value;
            scoreUI.IncreaseScore(score);
            if (score < 0) score = 0;
            if (score < maxScore * 0.6f) _score = Mathf.FloorToInt(maxScore * 0.6f);
            currentBlendShapeValue = Mathf.Clamp(currentBlendShapeValue + 50f, 0f, 100f);
            SetBlendShape(currentBlendShapeValue);
            if (score < 0)
            {
                score = 0;
                LoadEndScreen();
            }
            //UpdateScore(score);
        }
    }

    private void LoadHomeScene()
    {
        if (!openGame) AdsController.instance.ShowInter();
        openGame = false;
        EndGame = GetComponent<EndGame>();
        EndGame.enabled = false;
        animationFrameChecker.enabled = false;
        score = 0;
        SaveData savedata = new SaveData();
        savedata.LoadLevel();
        savedata.LoadMoney();
        CanvasLv1.Instance.UpdateMoney(money);
        CanvasLv1.Instance.UpdateLevel(Level, levelBonus);
    }

    public void LoadPlayScene()
    {
        Time.timeScale = 1;
        saveData.Save();
        LoadingScreen.Instance.LoadScene("Level01");
    }

    public void LoadEndScreen()
    {
        bool win = score >= maxScore * 0.6f; 
        endGame = true;
        canDrag = false;
        EndGame.Instance.EndGameState(win, score);
        
    }
    
    public void Ready()
    {
        score = 0;
        LoadModelInPlay();
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.OnCamera(0f, 2.5f, -4f);
            cameraFollow.SetAim(0.5f, 0.5f);
        }
        EndGame.enabled = true;
        animationFrameChecker.enabled = false;
        SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
        if (setObjectInLevel != null)
        {
            setObjectInLevel.SetupLevel(levelBonus);
        }
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log("Ready");
            //playerController.LoadAnim();
            playerController.LoadScore();
            playerController.enabled = false;
        }
        CanvasLv1.Instance.UpdateMoneyInPlay(bonusMoney);
        CanvasLv1.Instance.LoadLevelUI(levelBonus);
        CanvasLv1.Instance.UpdateLevel(Level, levelBonus);
    }
    public void StartGame()
    {
        scoreUI = FindObjectOfType<ScoreUI>();
        if (scoreUI != null)
            scoreUI.SetMaxScore(maxScore);
        canDrag = true;
        //SetBlendShape(currentBlendShapeValue);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.gameObject.transform.GetChild(0).transform.position = characterTrf;
            playerController.gameObject.transform.GetChild(0).transform.rotation = characterRot;
            playerController.animator.speed = 1f;
            playerController.enabled = true;
        }
        animationFrameChecker.enabled = true;
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        objectSpawner.StartSpawning();
        if (!levelBonus)
            currentBlendShapeValue = 100f;
        else
        {
            currentBlendShapeValue = 0f;
            score = maxScore;
        }
    }

    public void home()
    {
        Time.timeScale = 1;
        saveData.Save();
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        objectSpawner.StopSpawning();
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.StopSound();
        LoadingScreen.Instance.LoadScene("Home");
    } 

    public void ReStart()
    {
        Time.timeScale = 1;
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        objectSpawner.StopSpawning();
        LoadingScreen.Instance.LoadScene("Level01");
    }

    public void PauseGame()
    {
        canDrag = false;
        Time.timeScale = 0;
    }
    public void ContinueGame()
    {
        if(!endGame) canDrag = true;
        Time.timeScale = 1;
    }

    public void UnActiveBot()
    {
        bot.SetActive(false);
        SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
        setObjectInLevel.SpawnedBot();
    }

    public void AddMoney(int moneyValue)
    {
        //Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        //int value = objectSpawner.moneyValue;
        if (comboIndex > 0) moneyValue += comboIndex;
        bonusMoney += moneyValue;
        CanvasLv1.Instance.UpdateMoneyInPlay(bonusMoney);
    }

    private void AddCombo(bool addCombo)
    {
        if (!addCombo)
        {
            combo = 0;
            comboIndex = 0;
        }
        else
        {
            combo++;
            if (comboIndex < comboList.Count && combo >= comboList[comboIndex])
            {
                CanvasLv1.Instance.UpdateComboPopupUI(comboIndex);
                comboIndex++;
                if (comboIndex >= comboList.Count)
                {
                    comboIndex = comboList.Count - 1;
                    combo = comboList[comboIndex - 1];
                }
            }
        }    
    }

    public void ShowPopupEndgame(bool win)
    {
        CanvasLv1.Instance.ShowPopup(Level, bonusMoney, win);
        endGame = (!endGame) ? true : endGame;
        //Save level, bonus money
        //if (win && (Level - animIndex) < 1)
        if (win)
        {
            if (!levelBonus && (Level +1)% 5 == 0)
            {
                levelBonus = true;
            }
            else
            {
                Debug.Log("Level: " + Level);
                Level++;
                levelBonus = false;
            }
            saveData.SetLevelBonus(levelBonus);
        }
        //saveData.Save();
    }

    public void ShowCotinue()
    {
        Time.timeScale = 0;
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.animator.speed = 0f;
        }
        CanvasLv1.Instance.ShowContinuePop();
    }


}
