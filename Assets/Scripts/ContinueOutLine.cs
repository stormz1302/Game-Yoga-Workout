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
    bool isContinue = false;

    private void Start()
    {
        isContinue = false;
        StartCoroutine(StartCooldown());
        continueButton.onClick.AddListener(ContinueButtonClicked);
    }
    IEnumerator StartCooldown()
    {
        float time = CooldownTime;
        while (time > 0)
        {
            if (isContinue)
            {
                StopCoroutine(StartCooldown());
            }
            time -= Time.unscaledDeltaTime;
            OutLine.fillAmount = time / CooldownTime;
            yield return null;
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
        }, "Continue-Game-After-Die");
    }
}
