using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Sample;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image progressBar; 
    public List<Sprite> backGrounds = new List<Sprite>();
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
        Image loadingBG = loadingScreen.GetComponent<Image>();
        loadingBG.sprite = backGrounds[Random.Range(0, backGrounds.Count)];
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
                fakeProgress += Time.deltaTime * 0.5f; 
            }
            else
            {
                fakeProgress += Time.deltaTime * 0.3f;
            }

            float displayedProgress = Mathf.Min(fakeProgress, realProgress);
            progressBar.fillAmount = displayedProgress;

            if (realProgress >= 0.9f && fakeProgress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                // Chờ App Open Ad sẵn sàng
                while (!AppOpenAdController.instance.IsAppOpenAdAvailable)
                {
                    Debug.Log("Waiting for App Open Ad to be ready...");
                    yield return null; // Đợi cho đến khi quảng cáo sẵn sàng
                }
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        AudioManager.Instance.LoadVolumeSettings();
        loadingScreen.SetActive(false);
    }
}
