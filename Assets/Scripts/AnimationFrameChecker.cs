using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFrameChecker : MonoBehaviour
{
    public Animator playerAnimator;
    public GameObject bot;
    public Animator modelAnimator;
    [SerializeField] private float totalFrames = 60;
    [SerializeField] float tolerance = 1;
    PlayerController playerController;  
    public static AnimationFrameChecker Instance;
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
    }

    private void OnEnable()
    {
        playerController = FindObjectOfType<PlayerController>();
    }
    void Update()
    {
        if (bot != null)
        {
            if (bot.activeSelf && AreFramesMatching(playerAnimator, modelAnimator, totalFrames))
            {
                Debug.Log("Matching");
                playerController.isMatching = true;
            }
            else playerController.isMatching = false;
        }
    }

    bool AreFramesMatching(Animator playerAnimator, Animator modelAnimator, float totalFrames)
    {
        Debug.Log("Bot is not null" + bot.activeSelf);
        float playerFrame = Mathf.Floor(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrames);
        float modelFrame = Mathf.Floor(modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrames);
        Debug.Log("Player Frame: " + playerFrame + " Model Frame: " + modelFrame);
        Debug.Log(Mathf.Abs(playerFrame - modelFrame) + " / " + tolerance);
        return Mathf.Abs(playerFrame - modelFrame) < tolerance;
             
    }  

}
