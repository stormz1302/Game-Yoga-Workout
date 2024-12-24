using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Animator animator; 
    private List<string> animationName = new List<string>();
    public float dragThreshold = 500f; 
    private Vector3 startDragPosition;
    private bool isDragging = false;
    private float currentProgress = 0f;
    int animIndex;
    [SerializeField] private GameObject hitMoneyEfect;
    [SerializeField] private GameObject foodClean;
    [SerializeField] private GameObject GoodEfect;
    [SerializeField] private GameObject BadEfect;
    [SerializeField] private GameObject EndPoint;
    public bool isMatching = false;
    public bool isHit = false;
    bool canDrag;
    bool endGame = false;
    [SerializeField] private AnimatorController animatorController;
    Collider stage;

    [SerializeField] int botScorePerObject;
    [SerializeField] int goodFoodScorePerObject;
    [SerializeField] int trapScorePerObject;
    [SerializeField] int badFoodPenaltyPerObject;
    [SerializeField] int notMatchingPenaltyPerObject;


 
    private void Awake()
    {
        GameManager.Instance.player = gameObject.transform;
        EndGame.Instance.Player = gameObject;
    }
    private void Start()
    {
        AnimationFrameChecker.Instance.playerAnimator = animator;
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        animationName = animatorController.animationName;
        animIndex = GameManager.Instance.animIndex;
        animator.Play(animationName[animIndex]);

    }


    public void LoadScore()
    {
        SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
        setObjectInLevel.AssignScoresToObjects();
        botScorePerObject = setObjectInLevel.botScorePerObject;
        goodFoodScorePerObject = setObjectInLevel.goodFoodScorePerObject;
        trapScorePerObject = setObjectInLevel.trapScorePerObject;
        badFoodPenaltyPerObject = setObjectInLevel.badFoodPenaltyPerObject;
        notMatchingPenaltyPerObject = setObjectInLevel.notMatchingPenaltyPerObject;
    }

    void Update()
    {
        HoldAnimation();
        canDrag = GameManager.Instance.canDrag;
        endGame = GameManager.Instance.endGame;
        if (canDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDragPosition = Input.mousePosition;
                isDragging = true;
            }


            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentDragPosition = Input.mousePosition;
                float dragDistance = currentDragPosition.x - startDragPosition.x;


                float dragProgress = Mathf.Clamp(currentProgress + (dragDistance / dragThreshold), 0f, 1f);


                animator.Play(animationName[animIndex], 0, dragProgress);
                //animator.CrossFade(animationName[LevelIndex], 0.0f, 0, dragProgress);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 currentDragPosition = Input.mousePosition;
                float dragDistance = currentDragPosition.x - startDragPosition.x;
                currentProgress = Mathf.Clamp(currentProgress + (dragDistance / dragThreshold), 0f, 1f);

                isDragging = false;
            }
        }

        
    }

    

    //private void SetTranform()
    //{
    //    Debug.Log("SetTransform");
    //    animator.transform.position = new Vector3(0, 0, 0);
    //    animator.transform.rotation = Quaternion.Euler(0, 0, 0);
    //}

    private void HoldAnimation()
    {
        if (!isDragging) animator.speed = 0f;
        else animator.speed = 1f;
        if (endGame) animator.speed = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other != null && other.gameObject.CompareTag("Bot"))
        {
            isHit = true;
            if (isMatching)
            {
                GoodEfect.SetActive(true);
                AudioManager.Instance.PlaySound("GoodEffect");
                GameManager.Instance.AddScore(true, botScorePerObject);
                Invoke("Deactivate", 0.5f);
            }
            else
            {
                BadEfect.SetActive(true);
                AudioManager.Instance.PlaySound("BadEffect");
                GameManager.Instance.AddScore(false, notMatchingPenaltyPerObject);
                Invoke("Deactivate", 0.5f);
            }
            isHit = false;
            GameManager.Instance.UnActiveBot();
        }
        if (other != null && other.gameObject.CompareTag("Money") && other != stage)
        {
            GameManager.Instance.AddMoney();
            AudioManager.Instance.PlaySound("MoneyPickup");
            PlayerHit(hitMoneyEfect, other);
            stage = other;
        }
        else if (other != null && other.gameObject.CompareTag("Things"))
        {
            GameManager.Instance.ShowPopupEndgame(false);
        }
        if (other != null && other.gameObject.CompareTag("Clean") && other != stage)
        {
            GameManager.Instance.AddScore(true, goodFoodScorePerObject);
            AudioManager.Instance.PlaySound("GoodEffect");
            foodClean.SetActive(true);
            Invoke("Deactivate", 0.5f);
            other.transform.GetChild(0).gameObject.SetActive(false);
            stage = other;
        }
        if (other != null && other.gameObject.CompareTag("notClean") && other != stage)
        {
            GameManager.Instance.AddScore(false, badFoodPenaltyPerObject);
            AudioManager.Instance.PlaySound("BadEffect");
            BadEfect.SetActive(true);
            Invoke("Deactivate", 0.5f);
            other.transform.GetChild(0).gameObject.SetActive(false);
            stage = other;
        }
        if (other != null && other.gameObject.CompareTag("AdsPoint") && other != stage)
        {
            Debug.Log("=======Ads========");
            CanvasLv1.Instance.ShowAdsPopup();
            AudioManager.Instance.PlaySound("UnlockGift");
            stage = other;
        }
    }
    public void GoodEffect()
    {
        GoodEfect.SetActive(true);
        Invoke("Deactivate", 0.5f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("Stage") && other != stage)
        {
            GameManager.Instance.AddScore(true, trapScorePerObject);
            AudioManager.Instance.PlaySound("GoodEffect");
            Destroy(other.gameObject);
            stage = other;
        }
    }
    private void Deactivate() 
    {
        hitMoneyEfect.SetActive(false);
        foodClean.SetActive(false );
        GoodEfect.SetActive(false);
        BadEfect.SetActive(false);
    }

    private void PlayerHit(GameObject gameOb, Collider other)
    {
        gameOb.SetActive(true);
        gameOb.transform.position = other.transform.position;
        
        Invoke("Deactivate", 0.5f);
        Destroy(other.gameObject);
    }

}
