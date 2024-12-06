using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkinData", menuName = "ScriptableObjects/CharacterSkinData")]
public class CharacterSkinData : ScriptableObject
{
    public List<Character> characterSkinData = new List<Character>();
}
