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
    public List<GameObject> missions = new List<GameObject>();
    public int bonusMoney;
    public int Level = 0;
    public int animIndex = 0;
    public GameObject bot;
    public bool endGame = false;
    AnimationFrameChecker animationFrameChecker;

    [Header("Main")]
    public int money;

    [Header("Player")]
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer dress;
    public SkinnedMeshRenderer pants;
    public SkinnedMeshRenderer shoes;
    public SkinnedMeshRenderer hands;
    private float currentBlendShapeValue = 60f;
    [SerializeField] public Transform player;
    [SerializeField] int currentObject;
    

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
        if (scene.name == "Level01")
        {
            selectedSkinID = Shop.instance.selectedSkinID;
            Time.timeScale = 1;
            endGame = false;
            currentObject = 0;
            bonusMoney = 0;
            _score = 0;
            saveData = new SaveData();
            saveData.LoadMoney();
            CanvasLv1.Instance.ReadyScreen();
            //UpdateScore(0);
            //Level = PlayerPrefs.GetInt("Level", 0);
            LoadAnim();
            Ready();
        }
        if (scene.name == "Home")
        {
            LoadHomeScene();
            Debug.Log("Home Scene Loaded: " + Level);
        }
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
        Debug.Log("Anim Index: " + animIndex);
        if (Level <= 6) maxObject = MissionListSO.missionLevels[animIndex].maxObject;
        else
        {
            animIndex = Random.Range(0, 6);
            maxObject = 100;
        }
        missions = MissionListSO.missionLevels[animIndex].missions;   
    }
    public void AddScore(bool good)
    {
        if (good)
        {
            currentObject++;

            score = maxScore * currentObject / maxObject;
            AddCombo(true);
            if (score >= _score)
            {
                if (_score > 0)
                {
                    PlayerController playerController = FindObjectOfType<PlayerController>();
                    if (playerController != null) playerController.GoodEffect();
                    AudioManager.Instance.PlaySound("Transform");
                    currentBlendShapeValue = Mathf.Clamp(currentBlendShapeValue - 50f, 0f, 100f);
                }
                _score += maxScore / 2;
            }
            SetBlendShape(currentBlendShapeValue);
            //UpdateScore(score);
            if (score >= maxScore)
            {
                score = maxScore;
                LoadEndScreen();
            }
        }
        else if (!good && score > 0) 
        {
            AddCombo(false);
            currentObject -= maxObject / 4;
            if (currentObject < 0) currentObject = 0;
            score = maxScore * currentObject / maxObject;
            if (currentObject < maxObject / 2) _score -= maxScore / 2;
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
        //SaveData saveData = new SaveData();
        //saveData.LoadMoney();
        //saveData.LoadLevel();
        CanvasLv1.Instance.UpdateMoney(money);
        CanvasLv1.Instance.UpdateLevel(Level);

        Time.timeScale = 1;
    }

    public void LoadPlayScene()
    {
        LoadingScreen loadingScreen = FindObjectOfType<LoadingScreen>();
        loadingScreen.LoadScene("Level01");
    }

    public void LoadEndScreen()
    {
        bool win = score >= maxScore/2; 
        endGame = true;
        canDrag = false;
        EndGame.Instance.EndGameState(win, score);
        //Save level, bonus money
        if (win && (Level - animIndex) < 1)
        {
            Debug.Log("You Win");
            //SaveData saveData = new SaveData();
            //saveData.Save();
            //load end screen win
        }
        else
        {
            Debug.Log("You Lose");
            //load end screen lose
        }
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
    }
    public void StartGame()
    {
        currentBlendShapeValue = 100f;
        canDrag = true;
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.animator.speed = 1f;
        Debug.Log("Game Started: " + playerController.animator.speed);
        playerController.enabled = true;
        animationFrameChecker.enabled = true;
        Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
        objectSpawner.StartSpawning();
        SetBlendShape(currentBlendShapeValue);
    }

    public void home()
    {
        LoadingScreen loadingScreen = FindObjectOfType<LoadingScreen>();
        loadingScreen.LoadScene("Home");
    } 

    public void ReStart()
    {
        SceneManager.LoadScene("Level01");
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
    }

    public void AddMoney(int value)
    {
        if (comboIndex > 0) value += comboIndex;
        bonusMoney += value;
        CanvasLv1.Instance.UpdateMoneyInPlay(bonusMoney);
    }

    private void AddCombo(bool addCombo)
    {
        Debug.Log("Add Combo: " + combo);
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
        if (win)
        {
            Level ++;
        }
    }
}
