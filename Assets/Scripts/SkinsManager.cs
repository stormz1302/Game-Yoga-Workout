using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsManager : MonoBehaviour
{
    private const string OwnedKey = "CharacterOwned_";  // Key cho trạng thái sở hữu
    private const string EquippedKey = "CharacterEquipped";  // Key cho trạng thái trang bị
    private const string CurrentAdsKey = "CurrentAds_";
    public static SkinsManager instance;
    public int defaultSkinID = 9;
    bool isTrialAcitve = false;
    int trialID = -1;
    int equippedBeforeTrial = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SaveSkinOwned()
    {
        foreach (var character in Shop.instance.Skins)
        {
            PlayerPrefs.SetInt(OwnedKey + character.ID, character.isOwned ? 1 : 0);
        }
        
    }

    public void LoadCharacterData()
    {
        PlayerPrefs.SetInt(OwnedKey + defaultSkinID, 1);
        foreach (var character in Shop.instance.Skins)
        {
            character.isOwned = PlayerPrefs.GetInt(OwnedKey + character.ID, 0) == 1;
        }
        // Lấy trạng thái trang bị từ PlayerPrefs
        int equippedID = PlayerPrefs.GetInt(EquippedKey, defaultSkinID); // -1 nếu không có nhân vật nào được trang bị
        PlayerPrefs.SetInt(EquippedKey, equippedID);
        Shop.instance.equipedSkinID = equippedID;
        Shop.instance.LoadModel(equippedID);
    }

    //gọi sau khi chạy quảng cáo
    public void WatchAd(int skinID)
    {
        int currentAds = LoadCurrentAds(skinID);
        currentAds++;
        PlayerPrefs.SetInt(CurrentAdsKey + skinID, currentAds);
    }

    public bool CheckOwnedCharacter(int id)
    {
        return PlayerPrefs.GetInt(OwnedKey + id, 0) == 1;
    }

    public int LoadCurrentAds(int skinID)
    {
        return PlayerPrefs.GetInt(CurrentAdsKey + skinID, 0);
    }

    public void ResetCurrentAds(int skinID)
    {
        PlayerPrefs.SetInt(CurrentAdsKey + skinID, 0);
    }

    public void EquipCharacter(int id)
    {
        PlayerPrefs.SetInt(EquippedKey, id);
    }

    public int GetEquippedCharacter()
    {
        return PlayerPrefs.GetInt(EquippedKey, defaultSkinID);
    }

    //Buy skin
    public void UnlockCharacter(int id)
    {
        // Mở khóa nhân vật (sở hữu)
        Character character = Shop.instance.Skins.Find(c => c.ID == id);
        if (character != null)
        {
            character.isOwned = true;
            SaveSkinOwned();
        }
    }

    public void EquipTrialCharacter(int id)
    {
        ResetCurrentAds(id);
        equippedBeforeTrial = GetEquippedCharacter();
        EquipCharacter(id);
        trialID = id;
        isTrialAcitve = true;
    }

    public void EndTrial()
    {
        if (isTrialAcitve)
        {
            EquipCharacter(equippedBeforeTrial);
            trialID = -1;
            equippedBeforeTrial = -1;
            isTrialAcitve = false;
        }
    }

    public bool IsTrialActive()
    {
        return isTrialAcitve;
    }
}
