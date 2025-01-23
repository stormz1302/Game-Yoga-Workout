using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Sample;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image progressBar; 
    public GameObject loadingScreen;
    public static LoadingScreen Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        loadingScreen.SetActive(true);
        LoadScene("Home");
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.StopSound();
        float fakeProgress = 0f;

        while (!operation.isDone)
        {
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (fakeProgress < realProgress)
            {
                fakeProgress += Time.unscaledDeltaTime * 0.5f; 
            }
            else
            {
                fakeProgress += Time.unscaledDeltaTime * 0.3f;
            }

            float displayedProgress = Mathf.Min(fakeProgress, realProgress);
            progressBar.fillAmount = displayedProgress;

            if (realProgress >= 0.9f && fakeProgress >= 1f)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                // Chờ App Open Ad sẵn sàng 
                float timeOut = 0f;
                while (!AppOpenAdController.instance.IsAppOpenAdAvailable && timeOut < 3f)
                {
                    timeOut += Time.unscaledDeltaTime;
                    yield return null;
                }
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        AudioManager.Instance.LoadVolumeSettings();
        loadingScreen.SetActive(false);
    }
}
