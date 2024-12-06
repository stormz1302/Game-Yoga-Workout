using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimatorController", menuName = "ScriptableObjects/AnimatorController")]
public class AnimatorController : ScriptableObject
{
    public List<string> animationName = new List<string>();  
}
