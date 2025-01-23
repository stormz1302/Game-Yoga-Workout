using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadObject : MonoBehaviour
{
    public GameObject[] objects;

    void Start()
    {
        int level = GameManager.Instance.Level; 
        LoadModelBegin(level);
    }
    private void LoadModelBegin(int level)
    {
        if (level < 5)
        {
            for (int i = 1; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }
        }
       
    }
}
