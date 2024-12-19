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
    public bool isMatching;

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
            isMatching = AreFramesMatching(playerAnimator, modelAnimator, totalFrames);
            if (bot.activeSelf && isMatching)
            {
               // Debug.Log("Matching");
                playerController.isMatching = true;
            }
            else playerController.isMatching = false;
        }
    }

    private bool AreFramesMatching(Animator playerAnimator, Animator modelAnimator, float totalFrames)
    {
        float playerFrame = Mathf.Floor(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrames);
        float modelFrame = Mathf.Floor(modelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrames);
        //Debug.Log("Player Frame: " + playerFrame + " Model Frame: " + modelFrame);
        //Debug.Log(Mathf.Abs(playerFrame - modelFrame) + " / " + tolerance);
        return Mathf.Abs(playerFrame - modelFrame) < tolerance;
             
    }  

}
