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
    [SerializeField] private GameObject hitThingsEfect;
    [SerializeField] private GameObject GoodEfect;
    [SerializeField] private GameObject BadEfect;
    public bool isMatching = false;
    public bool isHit = false;
    bool hitThings = false;
    bool canDrag;
    bool endGame = false;
    [SerializeField] private AnimatorController animatorController;

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
            Debug.Log("hit Bot: " + isMatching);
            if (isMatching)
            {
                Debug.Log("hit Bot matching");
                GoodEfect.SetActive(true);
                AudioManager.Instance.PlaySound("GoodEffect");
                GameManager.Instance.AddScore(true);
                Invoke("Deactivate", 1f);
            }
            else
            {
                Debug.Log("hit Bot not matching");
                BadEfect.SetActive(true);
                AudioManager.Instance.PlaySound("BadEffect");
                GameManager.Instance.AddScore(false);
                Invoke("Deactivate", 1f);
            }
            isHit = false;
            GameManager.Instance.UnActiveBot();
        }
        if (other != null && other.gameObject.CompareTag("Money"))
        {
            //Event bonus money
            Debug.Log("hit Money");
            GameManager.Instance.AddMoney(Random.Range(5, 8));
            AudioManager.Instance.PlaySound("MoneyPickup");
            PlayerHit(hitMoneyEfect, other);
        }
        else if (other != null && other.gameObject.CompareTag("Things"))
        {
            //event hit things
            Debug.Log("hit Things");
            GameManager.Instance.LoadEndScreen();
        }
    }
    public void GoodEffect()
    {
        GoodEfect.SetActive(true);
        Invoke("Deactivate", 1f);
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("Stage"))
        {
            if (!hitThings)
            {
                GameManager.Instance.AddScore(true);
                AudioManager.Instance.PlaySound("GoodEffect");
                Destroy(other.gameObject);
            }else hitThings = false;
        }
    }
    private void Deactivate() 
    {
        hitMoneyEfect.SetActive(false);
        hitThingsEfect.SetActive(false );
        GoodEfect.SetActive(false);
        BadEfect.SetActive(false);
    }

    private void PlayerHit(GameObject gameOb, Collider other)
    {
        gameOb.SetActive(true);
        gameOb.transform.position = other.transform.position;
        
        Invoke("Deactivate", 1f);
        Destroy(other.gameObject);
    }



}
