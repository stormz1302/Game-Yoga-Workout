using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContinueOutLine : MonoBehaviour
{
    [SerializeField] Image OutLine;
    [SerializeField] float CooldownTime;
    [SerializeField] Button ContinueButton;

    private void Start()
    {
        StartCoroutine(StartCooldown());
    }
    IEnumerator StartCooldown()
    {
        float time = CooldownTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            OutLine.fillAmount = time / CooldownTime;
            yield return null;
        }
        GameManager.Instance.ShowPopupEndgame(false);
        gameObject.SetActive(false);
    }
}
