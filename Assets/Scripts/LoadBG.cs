using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBG : MonoBehaviour
{
    [SerializeField] List<GameObject> environments = new List<GameObject>();
    [SerializeField] Material skyboxNight;
    [SerializeField] Material skyboxDay;
    public static LoadBG Instance;

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
    }

    public void LoadMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= environments.Count)
        {
            mapIndex = Random.Range(0, environments.Count);
        }
        for (int i = 0; i < environments.Count; i++)
        {
            if (i != mapIndex)
            {
                environments[i].SetActive(false);
            }
        }
        environments[mapIndex].SetActive(true);

        if (mapIndex == 0 || mapIndex == 2)
        {
            Debug.Log("Night");
            RenderSettings.skybox = skyboxNight;
        }
        else if (mapIndex == 1 || mapIndex == 3)
        {
            RenderSettings.skybox = skyboxDay;
        }
    }
}
