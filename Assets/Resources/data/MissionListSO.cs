using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionList", menuName = "ScriptableObjects/MissionList", order = 1)]
public class MissionListSO : ScriptableObject
{
    public List<Mission> missionLevels = new List<Mission>();
}

