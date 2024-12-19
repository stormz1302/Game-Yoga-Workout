using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupEndGame : MonoBehaviour
{
    [SerializeField] private Image  OutlineAds;
    public float cooldownTime = 5f;
    public float reductionFactor = 1f;
    
    void Start()
    {
        StartCoroutine(TimeLine());
    }

    IEnumerator TimeLine()
    {
        float currentCooldown = cooldownTime;  

        OutlineAds.fillAmount = currentCooldown / cooldownTime; 

        while (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime * reductionFactor; 
            OutlineAds.fillAmount = currentCooldown / cooldownTime; 
            yield return null;  
        }
        OutlineAds.fillAmount = 0;  
    }
}
