using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] Slider scoreBar;          
    [SerializeField] float increaseDuration = 1.0f;

    
    public void IncreaseScore(float amount)
    {
        StartCoroutine(IncreaseScoreBar(amount));
    }

    IEnumerator IncreaseScoreBar(float targetValue)
    {
        float startValue = scoreBar.value;         
        float elapsedTime = 0f;

        while (elapsedTime < increaseDuration)
        {
            scoreBar.value = Mathf.Lerp(startValue, targetValue, elapsedTime / increaseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scoreBar.value = targetValue; 
    }

    public void SetMaxScore(float maxScore)
    {
        scoreBar.maxValue = maxScore;

        scoreBar.value = 0; 
    }
}
