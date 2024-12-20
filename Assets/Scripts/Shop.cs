﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using System.Linq;


public class Shop : MonoBehaviour
{
    [SerializeField] private CharacterSkinData characterData;
    [SerializeField] private GameObject skinPrefab;

    [SerializeField] private Transform contentView;
    public Transform SpawnCharacter;
    public RawImage character;
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private GameObject BuyButton;
    [SerializeField] private GameObject SelectButton;
    [SerializeField] private RawImage characterIcon;
    public List<Character> Skins = new List<Character>();
    [HideInInspector] public int selectedSkinID;
    [SerializeField] private List<GameObject> skinButtons = new List<GameObject>();
    public GameObject currentModel;
    private int currentSkinID;
    int currentClickedID = -1;
    public float delayBeforeShow = 0.2f;


    public static Shop instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "Home")
        {
            SpawnCharacter = GameObject.Find("CharacterSpwn").transform;
            if (Skins.Count != 0)
            {
                SkinsManager.instance.LoadCharacterData();
            }
        }
    }

    private void Start()
    {
        Skins = characterData.characterSkinData;
        SkinsManager.instance.LoadCharacterData();
        LoadSkins();
        
        BuyButton.GetComponent<Button>().onClick.AddListener(() => BuySkin());
        SelectButton.GetComponent<Button>().onClick.AddListener(() => SelectSkin());
        BuyButton.SetActive(false);
    }
    

    private void BuySkin()
    {
        if (GameManager.Instance.money >= Skins[currentSkinID].Price)
        {
            SkinsManager.instance.UnlockCharacter(currentSkinID);
            GameManager.Instance.money -= Skins[currentSkinID].Price;
            SaveData saveData = new SaveData();
            saveData.Save();
            skinButtons[currentSkinID].GetComponent<SkinView>().isOwned = true;
            AudioManager.Instance.PlaySound("BuySkin");
            BuyButton.SetActive(false);
            SelectButton.SetActive(true);
        }
    }

    private void SelectSkin()
    {
        SkinsManager.instance.EquipCharacter(currentSkinID);
        SelectButton.SetActive(false);
    }

    public void LoadSkins()
    {
        foreach (Transform child in contentView)
        {
            Destroy(child.gameObject);
        }

        if (Skins == null || Skins.Count == 0)
        {
            return;
        }

        List<Character> sortedSkins = Skins.OrderByDescending(skin => skin.isOwned).ToList();


        foreach (Character skin in sortedSkins)
        {
            GameObject skinButton = Instantiate(skinPrefab, contentView);
            skinButton.GetComponent<SkinView>().skinSprite = skin.CharacterIcon;
            skinButton.GetComponent<SkinView>().skinID = skin.ID;
            skinButton.GetComponent<SkinView>().isOwned = skin.isOwned;
            skinButton.GetComponent<Button>().onClick.AddListener(() => LoadModel(skin.ID));
            skinButtons.Add(skinButton);
        }
    }

    public void LoadModel(int skinID)
    {
        if (currentSkinID != skinID)
        {
            if (currentModel != null)
                Destroy(currentModel);

            Character skin = Skins[skinID];
            foreach (Transform child in SpawnCharacter)
            {
                Destroy(child.gameObject);
            }
            StartCoroutine(LoadPrefabModel(skin));
            Physics.SyncTransforms();
            currentModel.transform.localPosition = Vector3.zero;
            currentSkinID = skinID;
            AudioManager.Instance.PlaySound("SwicthSkin");
            checkOwnedSkin(skinID);
            //PlayerPrefs.SetInt("CurrentSkin", currentSkinID);
        }
    }

    IEnumerator LoadPrefabModel(Character skin)
    {
        currentModel = Instantiate(skin.CharacterPrf, SpawnCharacter);
        characterIcon.gameObject.SetActive(false);
        yield return null;
        yield return new WaitForSeconds(delayBeforeShow);
        characterIcon.gameObject.SetActive(true);
    }

    private void checkOwnedSkin(int skinID)
    {
        if (currentClickedID != skinID)
        {
            //if (currentID != -1) skinButtons[currentID].GetComponent<Outline>().enabled = false;
            currentClickedID = skinID;
        }
        if (!Skins[skinID].isOwned)
        {
            BuyButton.SetActive(true);
            SelectButton.SetActive(false);
            PriceText.text = Skins[skinID].Price.ToString() + "$";
        }
        else
        {
            CheckSelectSkin(); 
            BuyButton.SetActive(false);
        }
    }
    public bool CheckSkin(int skinID)
    {
        return currentClickedID == skinID;
    }

    private void CheckSelectSkin()
    {
        if (currentSkinID == selectedSkinID)
        {
            SelectButton.SetActive(false);
        }
        else
        {
            SelectButton.SetActive(true);
        }
    }
}
