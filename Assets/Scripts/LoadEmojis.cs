using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEmojis : MonoBehaviour
{
    [SerializeField] private GameObject[] male_Emojis;
    [SerializeField] private GameObject[] female_Emojis;
    [SerializeField] private GameObject emojiHeart;

    private void Start()
    {
        if (male_Emojis != null) EndGame.Instance.male_Emojis = male_Emojis;
        if (female_Emojis != null) EndGame.Instance.female_Emojis = female_Emojis;
        if (emojiHeart != null) EndGame.Instance.emojiHeart = emojiHeart;
    }
}
