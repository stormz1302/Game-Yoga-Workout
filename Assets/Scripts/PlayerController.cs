﻿using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Animator animator; 
    public List<string> animationName = new List<string>();
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

    private float lastShowTime = -25f;  // Lưu trữ thời gian gọi ShowCotinue() trước đó
    private float coolDownTime = 25f;   // Thời gian cooldown, 25 giây

    private void Awake()
    {
        GameManager.Instance.player = gameObject.transform;
        EndGame.Instance.Player = gameObject;
        animationName = animatorController.animationName;
    }
    private void Start()
    {
        AnimationFrameChecker.Instance.playerAnimator = animator;
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        animIndex = GameManager.Instance.animIndex;
        animator.Play(animationName[animIndex]);
    }

    public void LoadAnim()
    {
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

        Debug.Log("Bot: " + botScorePerObject);
        Debug.Log("Trap: " + trapScorePerObject);
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
                // Kiểm tra xem con trỏ chuột có nằm trong nửa dưới màn hình không
                if (Input.mousePosition.y <= Screen.height / 2)
                {
                    startDragPosition = Input.mousePosition;
                    isDragging = true;
                }
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentDragPosition = Input.mousePosition;
                float dragDistance = currentDragPosition.x - startDragPosition.x;

                float dragProgress = Mathf.Clamp(currentProgress + (dragDistance / dragThreshold), 0f, 1f);

                animator.Play(animationName[animIndex], 0, dragProgress);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    Vector3 currentDragPosition = Input.mousePosition;
                    float dragDistance = currentDragPosition.x - startDragPosition.x;
                    currentProgress = Mathf.Clamp(currentProgress + (dragDistance / dragThreshold), 0f, 1f);

                    isDragging = false;
                }
            }
        }

    }

    public void StartDragging()
    {
        startDragPosition = Input.mousePosition;
        isDragging = true;
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
            Objectspawner objectSpawner = FindObjectOfType<Objectspawner>();
            int value = objectSpawner.moneyValue;
            GameManager.Instance.AddMoney(value);
            AudioManager.Instance.PlaySound("MoneyPickup");
            PlayerHit(hitMoneyEfect, other);
            stage = other;
        }
        else if (other != null && other.gameObject.CompareTag("Things") && other != stage)
        {
            float currentTime = Time.time;
            AudioManager.Instance.PlaySound("Hit");
            canDrag = false;
            stage = other;
            Handheld.Vibrate();
            //show ads inter
            //AdsController.instance.ShowInter();
            // Kiểm tra xem đã đủ 25 giây kể từ lần gọi ShowCotinue trước đó chưa
            if (currentTime - lastShowTime >= coolDownTime)
            {
                GameManager.Instance.ShowCotinue();
                lastShowTime = currentTime; 
            }else if (currentTime - lastShowTime <= 1.5f)
            {
                return;
            }
            else
            {
                // Nếu chưa đủ 25 giây, bạn có thể thông báo hoặc làm gì đó ở đây
                GameManager.Instance.ShowPopupEndgame(false);
            } 
        }
        if (other != null && other.gameObject.CompareTag("Clean") && other != stage)
        {
            GameManager.Instance.AddScore(true, goodFoodScorePerObject);
            AudioManager.Instance.PlaySound("GoodEffect");
            foodClean.SetActive(true);
            foodClean.transform.position = other.transform.position;
            Invoke("Deactivate", 0.5f);
            other.transform.GetChild(0).gameObject.SetActive(false);
            stage = other;
        }
        if (other != null && other.gameObject.CompareTag("notClean") && other != stage)
        {
            GameManager.Instance.AddScore(false, badFoodPenaltyPerObject);
            AudioManager.Instance.PlaySound("BadEffect");
            BadEfect.SetActive(true);
            BadEfect.transform.position = other.transform.position;
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
            GoodEffect();
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
