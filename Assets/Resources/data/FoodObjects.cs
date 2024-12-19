using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodPrefabList", menuName = "ScriptableObjects/FoodPrefabList", order = 1)]
public class FoodObjects : ScriptableObject
{
    public List<GameObject> goodFoodPrefabs;
    public List<GameObject> badFoodPrefabs;
}
