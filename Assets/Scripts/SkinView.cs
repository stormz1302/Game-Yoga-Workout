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
    public GameObject selectIcon;

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
        int skinIdEquiped = SkinsManager.instance.GetEquippedCharacter();
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
        if (skinIdEquiped == skinID)
        {
            selectIcon.SetActive(true);
        }
        else
        {
            selectIcon.SetActive(false);
        }
    }
    //public void OnClick()
    //{
    //    gameObject.GetComponent<Outline> ().enabled = true;
    //}
    
}
