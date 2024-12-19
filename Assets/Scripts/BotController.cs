using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private float totalFrames = 60;
    [SerializeField] private Transform Player;
    [SerializeField] private GameObject outline;

    private List<string> animationName = new List<string>();
    private int animIndex;
    private PlayerController playerController;
    [SerializeField] private AnimatorController animatorController;

    bool isMatching;

    private void Awake()
    {
        // Lấy tham chiếu tới PlayerController
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }

        // Gán danh sách tên animation từ AnimatorController
        if (animatorController != null)
        {
            animationName = animatorController.animationName;
        }
        else
        {
            Debug.LogError("AnimatorController is not assigned.");
        }

        // Lưu bot và animIndex vào GameManager
        if (GameManager.Instance != null)
        {
            animIndex = GameManager.Instance.animIndex;
            GameManager.Instance.bot = gameObject;
        }
        else
        {
            Debug.LogError("GameManager.Instance is null.");
        }

        // Gán modelAnimator và bot vào AnimationFrameChecker
        if (AnimationFrameChecker.Instance != null)
        {
            AnimationFrameChecker.Instance.modelAnimator = modelAnimator;
            AnimationFrameChecker.Instance.bot = gameObject;
        }
        else
        {
            Debug.LogError("AnimationFrameChecker.Instance is null.");
        }
    }

    private void OnEnable()
    {
        // Random một frame và dừng animation tại frame đó
        if (modelAnimator == null || animationName.Count == 0 || animIndex < 0 || animIndex >= animationName.Count)
        {
            Debug.LogError("Animator or animation data is invalid.");
            return;
        }

        float randomFrame = Random.Range(0, totalFrames);
        StopAtFrame(modelAnimator, randomFrame, totalFrames);
    }

    private void Update()
    {
        // Kiểm tra các tham chiếu trước khi sử dụng
        if (playerController == null || AnimationFrameChecker.Instance == null) return;

        bool isHit = playerController.isHit;
        CheckHitPlayer(isHit);

        bool isMatching = AnimationFrameChecker.Instance.isMatching;

        if (outline != null)
        {
            outline.SetActive(isMatching);
        }
        else
        {
            Debug.LogWarning("Outline GameObject is not assigned.");
        }
    }

    private void CheckHitPlayer(bool isHit)
    {
        if (Player == null)
        {
            Debug.LogError("Player transform is not assigned.");
            return;
        }

        if (gameObject.transform.position.z <= Player.position.z && !isHit)
        {
            if (GameManager.Instance != null)
            {
                SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
                if (setObjectInLevel != null) {
                    GameManager.Instance.AddScore(false, setObjectInLevel.notMatchingPenaltyPerObject); 
                }
            }
            else
            {
                Debug.LogError("GameManager.Instance is null.");
            }
        }
    }

    private void StopAtFrame(Animator animator, float frame, float totalFrames)
    {
        if (animIndex < 0 || animIndex >= animationName.Count)
        {
            Debug.LogError("Invalid animation index: " + animIndex);
            return;
        }
        float targetNormalizedTime = frame / totalFrames;
        animator.Play(animationName[animIndex], 0, targetNormalizedTime);
        animator.speed = 0; // Dừng animation
    }
}
