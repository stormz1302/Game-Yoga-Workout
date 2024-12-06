using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (other != null && other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
