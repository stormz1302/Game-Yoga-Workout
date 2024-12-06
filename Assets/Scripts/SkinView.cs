using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinView : MonoBehaviour
{
    public Image SkinIcon;
    public Sprite skinSprite;
    public GameObject panel;
    public GameObject blockIcon;
    public bool isOwned;
    public int skinID;

    private void Start()
    {
        gameObject.GetComponent<Outline>().enabled = false;
        if (isOwned)
        {
            panel.SetActive(false);
            blockIcon.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
            blockIcon.SetActive(true);
        }
    }

    private void Update()
    {
        if (skinSprite != null && SkinIcon.sprite != skinSprite)
        {
            SkinIcon.sprite = skinSprite;
        }
        if (isOwned)
        {
            panel.SetActive(false);
            blockIcon.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
            blockIcon.SetActive(true);
        }
    }

    public void OnClick()
    {
        gameObject.GetComponent<Outline> ().enabled = true;
    }
    
}
