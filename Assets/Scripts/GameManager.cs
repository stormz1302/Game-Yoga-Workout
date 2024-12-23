using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Gameplay")]
    public float score = 0;
    public int maxScore;
    public int maxObject;
    public MissionListSO MissionListSO;
    public BonusMission bonusMission;
    public List<GameObject> missions = new List<GameObject>();
    public int bonusMoney = 0;
    public int Level = 0;
    public int animIndex = 0;
    public GameObject bot;
    public bool endGame = false;
    AnimationFrameChecker animationFrameChecker;
    bool levelBonus = false;
    public int bonusCount;
    public float bonusTime;

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
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
            
            LoadAnim();
            Ready();
        }
        if (scene.name == "Home")
        {
            LoadHomeScene();
        }
        AchievementReward achievementReward = FindObjectOfType<AchievementReward>();
        achievementReward.UpdateRewardBar();

    }

    private void LoadModelInPlay()
    {
        GameObject character = Instantiate(Shop.instance.Skins[selectedSkinID].CharacterPrf);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.animator = character.GetComponent<Animator>();
        character.transform.SetParent(player);
        character.transform.SetSiblingIndex(0);
    }
    private void SetBlendShape(float value)
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
        if (!levelBonus) 
        {
            if (Level <= 6)
            {
                SetDifficultyLevel(DifficultyLevel.Beginner);
                animIndex = Level;
            }
            else if (6 < Level && Level <= 15)
            {
                SetDifficultyLevel(DifficultyLevel.Novice);
            }
            else if (15 < Level && Level <= 25)
            {
                SetDifficultyLevel(DifficultyLevel.Intermediate);
            }
            else if (25 < Level && Level <= 40)
            {
                SetDifficultyLevel(DifficultyLevel.Advanced);
            }
            else if (40 < Level && Level <= 60)
            {
                SetDifficultyLevel(DifficultyLevel.Expert);
            }
            else if (60 < Level)
            {
                SetDifficultyLevel(DifficultyLevel.Master);
            }
            missions = MissionListSO.missionLevels[animIndex].missions;
        }
        else
        {
            missions = bonusMission.missions;
            bonusCount = bonusMission.missionCount;
            bonusTime = bonusMission.missionTime;
        }
        
    }

    private void SetDifficultyLevel(DifficultyLevel difficultyLevel)
    {
        animIndex = Random.Range(0, 6);
        MissionListSO.missionLevels[animIndex].difficultyLevel = difficultyLevel;
        MissionListSO.missionLevels[animIndex].SetDifficulty();
        maxObject = MissionListSO.missionLevels[animIndex].maxObject;
    }

    public void AddScore(bool good, int value)
    {
        Debug.Log("AddScore");
        if (!levelBonus && good)
        {
            score += value;
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
        LoadingScreen loadingScreen = FindObjectOfType<LoadingScreen>();
        loadingScreen.LoadScene("Level01");
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
        LoadModelInPlay();
        EndGame.enabled = true;
        animationFrameChecker.enabled = false;
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.animator.speed = 0f;
        playerController.enabled = false;
        CanvasLv1.Instance.UpdateMoneyInPlay(bonusMoney);
        CanvasLv1.Instance.LoadLevelUI(levelBonus);
        SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
        setObjectInLevel.SetupLevel(levelBonus);
        setObjectInLevel.AssignScoresToObjects();
        playerController.LoadScore();
        
    }
    public void StartGame()
    {
        scoreUI = FindObjectOfType<ScoreUI>();
        if (scoreUI != null)
            scoreUI.SetMaxScore(maxScore);
        canDrag = true;
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.animator.speed = 1f;
        playerController.enabled = true;
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
        SetBlendShape(currentBlendShapeValue);
    }

    public void home()
    {
        Time.timeScale = 1;
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        objectSpawner.StopSpawning();
        LoadingScreen loadingScreen = FindObjectOfType<LoadingScreen>();
        loadingScreen.LoadScene("Home");
    } 

    public void ReStart()
    {
        Time.timeScale = 1;
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        objectSpawner.StopSpawning();
        LoadingScreen loadingScreen = FindObjectOfType<LoadingScreen>();
        loadingScreen.LoadScene("Level01");
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

    public void AddMoney()
    {
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        int value = objectSpawner.moneyValue;
        if (comboIndex > 0) value += comboIndex;
        bonusMoney += value;
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
        if (win && (Level - animIndex) < 1)
        {
            if (!levelBonus && (Level +1)% 5 == 0)
            {
                levelBonus = true;
            }
            else
            {
                Level++;
                levelBonus = false;
            }
            saveData.SetLevelBonus(levelBonus);
        }
        saveData.Save();
    }
}
