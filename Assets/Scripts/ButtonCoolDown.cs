using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonCoolDown : MonoBehaviour
{
    [SerializeField] private Button targetButton;
    public Image targetImage;
    [SerializeField] float cooldownTime = 5f;
    public TMP_Text cooldownText;

    private float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            if(targetImage != null)
            {
                targetImage.fillAmount = cooldownTimer / cooldownTime;
            }

            if (cooldownText != null)
            {
                cooldownText.text = FormatTime(cooldownTimer);
            }
            if (cooldownTimer <= 0)
            {
                ResetButton();
            }
        }
    }

    public void StartCooldown(Button button, int remainingTime, int cldownTime)
    {
        targetButton = button;
        targetButton.interactable = false;
        cooldownTime = cldownTime;
        cooldownTimer = remainingTime;
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            targetImage.fillAmount = 1;
        }
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(true);
        }
    }

    private void ResetButton()
    {
        targetButton = GetComponent<Button>();
        if (targetButton == null)
        {
            Debug.LogError("ButtonCoolDown: targetButton is null");
            return;
        }
        targetButton.interactable = true;
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(false);
        }
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false);
        }
    }
    private string FormatTime(float time)
    {
        if (time < 0) time = 0; // Đảm bảo không hiển thị giá trị âm

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
