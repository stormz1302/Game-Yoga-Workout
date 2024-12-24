using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GiftScript
{
    public int skinID;
    public Sprite SkinSprite;
    public float ratio;
    public int value;
    public GiftType giftType;
    public enum GiftType
    {
        Money,
        Skin,
    }
}
