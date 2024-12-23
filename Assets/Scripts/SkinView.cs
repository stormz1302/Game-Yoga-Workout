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
        bool isSelect = Shop.instance.CheckSkin(skinID);
        if (isSelect)
        {
            gameObject.GetComponent<Outline>().enabled = true;
        }
    }


    private void Update()
    {
        isOwned = Shop.instance.SkinOwned(skinID);
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
        bool isSelect = Shop.instance.CheckSkin(skinID);
        gameObject.GetComponent<Outline>().enabled = isSelect;
        
    }
    //public void OnClick()
    //{
    //    gameObject.GetComponent<Outline> ().enabled = true;
    //}
    
}
