using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Background", menuName = "ScriptableObjects/Background", order = 1)]
public class Background : ScriptableObject
{
    public List<Sprite> backGrounds = new List<Sprite>();
}
