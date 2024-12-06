using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Image FillBar;
    private void Start()
    {
        FillBar.fillAmount = 0;
    }
    public void UpdateScoreBar(float fillAmount)
    {
        Debug.Log("Fill amount: " + fillAmount);
        FillBar.fillAmount = fillAmount;
    }
}
