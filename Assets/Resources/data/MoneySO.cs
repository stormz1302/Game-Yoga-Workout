using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoneySO", menuName = "ScriptableObjects/MoneySO")]
public class MoneySO : ScriptableObject
{
    public GameObject Money;    
    public int moneyValue;
}
