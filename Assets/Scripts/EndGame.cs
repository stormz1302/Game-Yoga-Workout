using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class EndGame : MonoBehaviour
{
    public GameObject Male;
    public GameObject Player;
    private NavMeshAgent navMeshAgent;
    private GameObject Female;
    public Transform FinishPoint;
    private Animator femaleAnimator;
    private Animator maleAnimator;
    [SerializeField] float maxHight = 40.6f;
    public GameObject finishBG;
    public GameObject[] male_Emojis;
    public GameObject[] female_Emojis;  
    public float pushForce = 10f;     

    [SerializeField] bool endGame = false;
    public float speed = 5f;
    public static EndGame Instance;
    [SerializeField] private Transform moneyFinish;
    int i = 0;
    bool _win = false;
    float maxY = 0;
    GameObject moneyEffect;
    GameObject endGameEffect;

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
        if (scene.name == "Level01")
        {
            i = 0;
            endGame = false;
            i = 0;
            endGame = false;
            Male = GameObject.Find("male00");
            moneyFinish = GameObject.Find("MoneyFinish").transform;
            FinishPoint = GameObject.Find("FinishPoint").transform;
            
        }
    }

    private void Update()
    {
        if (!endGame) endGame = GameManager.Instance.endGame;
        if (endGame && i == 0)
        {
            Female = Player.transform.GetChild(0).gameObject;
            moneyEffect = moneyFinish.GetChild(0).gameObject;
            endGameEffect = Male.transform.GetChild(4).gameObject;
            if (endGameEffect.activeSelf) endGameEffect.SetActive(false);
            femaleAnimator = Female.GetComponent<Animator>();
            maleAnimator = Male.GetComponent<Animator>();
            Invoke("RunState", 1f);
            SetTransForm();
            StopSpawning();
            CameraFollow( 0f, 3.5f, -4.5f );
            femaleAnimator.SetTrigger("idle");
            i++;
        }
    }

    private IEnumerator MoveFemaleToFinish()
    {
        femaleAnimator.SetTrigger("run");
        femaleAnimator.applyRootMotion = false;
        Vector3 finishPoint = FinishPoint.position;
        finishPoint.y += 0.15f;
        Vector3 targetPosition = finishPoint;
        navMeshAgent = Player.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(targetPosition);
        while (Vector3.Distance(Player.transform.position, finishPoint) > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        CameraFollow(5.5f, 4f, -0.5f);
        femaleAnimator.applyRootMotion = true;
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        StartCoroutine(PlayEmojiThenCharacterAnim(_win));
        
    }
    
    private void RunState()
    {
        
        StartCoroutine(MoveFemaleToFinish());
    }

    private void StopSpawning()
    {
        Objectspawner objectspawner = FindObjectOfType<Objectspawner>();
        objectspawner.StopSpawning();
    }

    private void CameraFollow(float x, float y, float z)
    {
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow.OnCamera(x, y, z);
    }

    private void SetTransForm()
    {
        Female.transform.position = new Vector3(0,0,0);
        Female.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    public void EndGameState(bool win, float score)
    {
        _win = win;
        maxY = score >= 100 ? maxHight : (score >= 75 ? (float)(0.75f * maxHight) : (float)(0.5f * maxHight));
    }

    private IEnumerator PlayEmojiThenCharacterAnim(bool isWin)
    {
        int state = isWin ? 0 : 1; // 0: win, 1: lose

        if (male_Emojis[state] != null)
        {
            male_Emojis[state].SetActive(true); 
            female_Emojis[state].SetActive(true);
            if (!isWin)
            {
                maleAnimator.SetTrigger("angry");
                femaleAnimator.SetTrigger("sad");
            }
            else 
            {
                endGameEffect.SetActive(true);
                AudioManager.Instance.PlaySound("Cheers");
                maleAnimator.SetTrigger("Surprised");
                femaleAnimator.SetTrigger("happy");
            }
            yield return new WaitForSeconds(3f); 
            male_Emojis[state].SetActive(false); 
            female_Emojis[state].SetActive(false);
        }

        // Bước 2: Thực hiện hoạt ảnh của nhân vật
        
        if (isWin)
        {
            femaleAnimator.SetTrigger("kiss"); 
            maleAnimator.SetTrigger("kiss");
            yield return new WaitForSeconds(6f);
            femaleAnimator.SetTrigger("Dance");
        }
        else
        {
            GameManager.Instance.ShowPopupEndgame(false);
            yield break;
        }

        finishBG.SetActive(true);
        Female.transform.rotation = Quaternion.Euler(0, 180f, 0);

        CameraFollow(0f, 2.8f, -5f);
        while (Player.transform.position.y < maxY)
        {
            PushUp();
            yield return null;
        }
        if (isWin) moneyEffect.SetActive(true);
        yield return new WaitForSeconds(3f);
        // enable Popup end game(win)
        GameManager.Instance.ShowPopupEndgame(true);
    }

    public void PushUp()
    {
        if (Player.transform != null)
        {
            // Tạo chuyển động đẩy lên bằng cách dùng Translate
            Vector3 upwardMovement = Vector3.up * pushForce * Time.deltaTime;
            Player.transform.Translate(upwardMovement, Space.World);
            moneyFinish.transform.Translate(upwardMovement, Space.World);
        }
        else
        {
            Debug.LogError("Transform không được gán!");
        }

    }


}
