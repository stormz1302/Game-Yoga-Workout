using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public List<GameObject> missions = new List<GameObject>();
    public int maxObject;
    public Material roadMaterial;
    public Material skybox;
}
