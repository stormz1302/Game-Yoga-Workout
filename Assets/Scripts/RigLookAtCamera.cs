using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class RigLookAtCamera : MonoBehaviour
{
    public Transform target;
    VRMLookAtHead lookAtHead;

    void Start()
    {
        lookAtHead = GetComponent<VRMLookAtHead>();
        Animator animator = GetComponent<Animator>();
        target = Camera.main.transform;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Home")
        {
            
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            lookAtHead.Target = target;
        }
        if (sceneName == "Level01")
        {
            animator.updateMode = AnimatorUpdateMode.Normal;
        }
    }

}
