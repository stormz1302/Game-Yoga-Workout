using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public TMP_Text fpsText; // Tham chiếu đến Text UI

    private float deltaTime = 0.0f;

    void Update()
    {
        // Tính thời gian giữa các frame
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Tính FPS
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {fps:0.}";
    }
}
