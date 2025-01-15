using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingEffect : MonoBehaviour
{
    public TMP_Text loadingText; 
    private string baseText = "Loading"; 
    private int dotCount = 0; 

    void Start()
    {
        StartCoroutine(LoadingAnimation());
    }

    IEnumerator LoadingAnimation()
    {
        while (true) // Lặp vô tận
        {
            dotCount = (dotCount + 1) % 4; // Xoay vòng từ 0 đến 3
            loadingText.text = baseText + new string('.', dotCount); // Cập nhật dấu chấm
            yield return new WaitForSeconds(0.5f); // Chờ 0.5 giây trước khi lặp
        }
    }
}
