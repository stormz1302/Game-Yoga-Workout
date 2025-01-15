using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class CharacterLoad : MonoBehaviour
{
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer dress;
    public SkinnedMeshRenderer pants;
    public SkinnedMeshRenderer shoes;
    public SkinnedMeshRenderer hands;
    private Animator animator;
    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (body != null) GameManager.Instance.body = body;
        if (dress != null) GameManager.Instance.dress = dress;
        if (pants != null) GameManager.Instance.pants = pants;
        if (shoes != null) GameManager.Instance.shoes = shoes;
        if (hands != null) GameManager.Instance.hands = hands;

        if (scene.name == "Level01")
        {
            GameManager.Instance.SetBlendShape(100);
        }
    }
}
