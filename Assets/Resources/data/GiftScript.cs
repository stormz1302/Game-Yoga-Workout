using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GiftScript
{
    public List<Character> skins;
    public float ratio;
    public int value;
    public GiftType giftType;
    public enum GiftType
    {
        Money,
        Skin,
    }

    public int GetSkinID()
    {
        return skins[Random.Range(0, skins.Count)].ID;
    }
}
