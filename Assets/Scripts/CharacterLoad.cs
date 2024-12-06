using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoad : MonoBehaviour
{
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer dress;
    public SkinnedMeshRenderer pants;
    public SkinnedMeshRenderer shoes;
    public SkinnedMeshRenderer hands;

    private void Start()
    {
        if (body != null) GameManager.Instance.body = body;
        if (dress != null) GameManager.Instance.dress = dress;
        if (pants != null) GameManager.Instance.pants = pants;
        if (shoes != null) GameManager.Instance.shoes = shoes;
        if (hands != null) GameManager.Instance.hands = hands;
    }
}
