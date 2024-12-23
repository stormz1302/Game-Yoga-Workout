using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusMission", menuName = "ScriptableObjects/BonusMission", order = 1)]
public class BonusMission : ScriptableObject
{
    public List<GameObject> missions = new List<GameObject>();
    public int missionCount;
    public float missionTime;
}
