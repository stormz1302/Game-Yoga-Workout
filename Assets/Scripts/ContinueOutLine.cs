using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContinueOutLine : MonoBehaviour
{
    [SerializeField] Image OutLine;
    [SerializeField] float CooldownTime;
    [SerializeField] Button continueButton;
    [SerializeField] GameObject skipButton;
    bool isContinue = false;

    private void Start()
    {
        isContinue = false;
        continueButton.onClick.AddListener(ContinueButtonClicked);
        skipButton.GetComponent<Button>().onClick.AddListener(OnClickShipButton);
    }

    private void OnEnable()
    {
        isContinue = false;
        OutLine.fillAmount = 1f;
        continueButton.gameObject.GetComponent<Animation>().Play();
        Animator animator = skipButton.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
    private void Update()
    {
        bool isWatching = AdsController.instance.Showing_applovin_ads;
        if (!isWatching)
        {
            Time.timeScale = 0;
            StartCoroutine(StartCooldown());
        }
    }

    IEnumerator StartCooldown()
    {
        float startTime = Time.realtimeSinceStartup; 
        float endTime = startTime + CooldownTime;  
        OutLine.fillAmount = 1f;
        while (Time.realtimeSinceStartup < endTime)
        {
            // Nếu isContinue được kích hoạt, thoát khỏi coroutine
            if (isContinue)
            {
                yield break; 
            }
            // Tính thời gian còn lại dựa trên thời gian thực
            float remainingTime = endTime - Time.realtimeSinceStartup;
            // Cập nhật UI hiển thị tiến trình cooldown
            OutLine.fillAmount = remainingTime / CooldownTime;
            yield return null; // Đợi frame tiếp theo
        }
        Time.timeScale = 1; 
        GameManager.Instance.ShowPopupEndgame(false);
        gameObject.SetActive(false);
    }


    public void ContinueButtonClicked()
    {
        isContinue = true;
        AdsController.instance.ShowReward(() =>
        {
            AdsController.instance.HideMrec();
            gameObject.SetActive(false);
            Time.timeScale = 1;
            GameManager.Instance.canDrag = true;
        }, "Continue-Game-After-Die");
    }

    private void OnClickShipButton()
    {
        isContinue = true;
        gameObject.SetActive(false);
        Time.timeScale = 1;
        //AdsController.instance.ShowInter();
        GameManager.Instance.ShowPopupEndgame(false);
    }
}
