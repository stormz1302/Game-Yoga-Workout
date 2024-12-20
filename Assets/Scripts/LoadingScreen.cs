using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image progressBar; 
    public List<Sprite> backGrounds = new List<Sprite>();
    public GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(false);
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
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
