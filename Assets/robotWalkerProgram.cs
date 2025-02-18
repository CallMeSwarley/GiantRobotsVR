using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotWalkerProgram : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool o_pressed = Input.GetKey("o");
        bool isRobotWalking = animator.GetBool("isRobotWalking");
        bool right_shift_pressed = Input.GetKey("right shift");
        bool isRobotRunning = animator.GetBool("isRobotRunning");

        if (!isRobotWalking && o_pressed)
        {
            animator.SetBool("isRobotWalking", true);
        }

        if (isRobotWalking && !o_pressed)
        {
            animator.SetBool("isRobotWalking", false);
        }

        if (!isRobotRunning && (o_pressed && right_shift_pressed))
        {
            animator.SetBool("isRobotRunning", true);
        }

        if (isRobotRunning && (!o_pressed || !right_shift_pressed))
        {
            animator.SetBool("isRobotRunning", false);
        }
    }
}
