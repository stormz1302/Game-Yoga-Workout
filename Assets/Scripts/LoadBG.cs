using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBG : MonoBehaviour
{
    [SerializeField] List<Sprite> backGrounds = new List<Sprite>();
    [SerializeField] GameObject backGround;
    [SerializeField] int Level;
    [SerializeField] Background background;

    void Start()
    {
        backGrounds = background.backGrounds;
        Level = GameManager.Instance.Level;
        LoadBGround(Level);
    }

    private void LoadBGround(int level)
    {
        int backGroundIndex = (level + 1) / 5;
        if (backGroundIndex >= backGrounds.Count)
        {
            backGroundIndex = Random.Range(0, backGrounds.Count);
        }
        backGround.GetComponent<SpriteRenderer>().sprite = backGrounds[backGroundIndex];
        Debug.Log("Load BG: " + backGrounds[backGroundIndex]);
    }
}
