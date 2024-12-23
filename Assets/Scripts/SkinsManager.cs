using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsManager : MonoBehaviour
{
    private const string OwnedKey = "CharacterOwned_";  // Key cho trạng thái sở hữu
    private const string EquippedKey = "CharacterEquipped";  // Key cho trạng thái trang bị
    public static SkinsManager instance;

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
        foreach (var character in Shop.instance.Skins)
        {
            character.isOwned = PlayerPrefs.GetInt(OwnedKey + character.ID, 0) == 1;
            
        }

        // Lấy trạng thái trang bị từ PlayerPrefs
        int equippedID = PlayerPrefs.GetInt(EquippedKey, 6); // -1 nếu không có nhân vật nào được trang bị
        PlayerPrefs.SetInt(EquippedKey, equippedID);
        Shop.instance.equipedSkinID = equippedID;
        Shop.instance.LoadModel(equippedID);
    }

    public void EquipCharacter(int id)
    {
        PlayerPrefs.SetInt(EquippedKey, id);
    }
    public int GetEquippedCharacter()
    {
        return PlayerPrefs.GetInt(EquippedKey, 6);
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
}
