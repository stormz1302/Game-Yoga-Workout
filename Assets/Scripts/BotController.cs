using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class BotController : MonoBehaviour
{
    
    [SerializeField] private Animator modelAnimator;
    private List<string> animationName = new List<string>();
    [SerializeField] private float totalFrames = 60;
    //[SerializeField] GameObject Root_M;
    [SerializeField] private Transform Player;
    int animIndex;


    PlayerController playerController;

    [SerializeField] private AnimatorController animatorController;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        animationName = animatorController.animationName;
        animIndex = GameManager.Instance.animIndex;
        GameManager.Instance.bot = gameObject;
        AnimationFrameChecker.Instance.modelAnimator = modelAnimator;
        AnimationFrameChecker.Instance.bot = gameObject;
    }

    
    void OnEnable()
    {
        //Root_M.SetActive(true);
        //Root_M.transform.position = gameObject.transform.position;
        float randomFrame = Random.Range(0, totalFrames);
        StopAtFrame(modelAnimator, randomFrame, totalFrames);
        Debug.Log("Anim index: " + animIndex);

    }
    private void Update()
    {
        bool isHit = playerController.isHit;
        CheckHitPlayer(isHit);
    }
    void CheckHitPlayer(bool isHit)
    {
        if (gameObject.transform.position.z <= Player.position.z && !isHit)
        {
            GameManager.Instance.AddScore(false);
        }
    }
    void StopAtFrame(Animator animator, float frame, float totalFrames)
    {
        float targetNormalizedTime = frame / totalFrames;
        animator.Play(animationName[animIndex], 0, targetNormalizedTime);
        animator.speed = 0; 
    }

    //Update get total frames and animation name
}
