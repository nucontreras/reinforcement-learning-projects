using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPresses = Input.GetKey("w");
        if (!isWalking && forwardPresses)
        {
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !forwardPresses)
        {
            animator.SetBool(isWalkingHash, false);
        }
    }
}
