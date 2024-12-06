using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBG : MonoBehaviour
{
    [SerializeField] private GameObject road;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private Material skybox;
    private MissionListSO missionListSO;
    int mapIndex = 0;

    private void Start()
    {
        mapIndex = GameManager.Instance.animIndex;
        missionListSO = GameManager.Instance.MissionListSO;
        LoadMaterial();
    }

    private void LoadMaterial()
    {
        roadMaterial = missionListSO.missionLevels[mapIndex].roadMaterial;
        skybox = missionListSO.missionLevels[mapIndex].skybox;
        road.GetComponent<MeshRenderer>().material = roadMaterial;
        RenderSettings.skybox = skybox;
    }
}
