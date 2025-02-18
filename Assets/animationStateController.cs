using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
//# This is for the walking guy "Bryce".
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
    bool w_pressed = Input.GetKey("w");
    bool isBryceWalking = animator.GetBool("isBryceWalking");
    if (!isBryceWalking && w_pressed)
        {
            animator.SetBool("isBryceWalking", true);
        }

     if (isBryceWalking && !w_pressed)
        {
            animator.SetBool("isBryceWalking", false);
        }
    }
}
