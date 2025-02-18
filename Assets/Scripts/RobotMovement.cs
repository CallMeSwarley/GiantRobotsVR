using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public CharacterController CharacterController;
    public ForwardJoystick ForwardJoystick;
    public ForwardJoystick RotateJoystick;
    public Animator characterAnimator;
    
    public float Speed = 2f;

    private static readonly int SpeedAnimationKey = Animator.StringToHash("Speed");
    private void Start()
    {
        ForwardJoystick.forwardAction += OnForwardEvent;
        ForwardJoystick.backwardAction += OnBackwardEvent;
        
        RotateJoystick.forwardAction += OnRotateClockwiseEvent;
        RotateJoystick.backwardAction += OnRotateCounterClockwiseEvent;
    }
    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W))
            OnForwardEvent(0.2f);
        else if(Input.GetKey(KeyCode.E))
            OnForwardEvent(0.7f);
        else if(Input.GetKey(KeyCode.S))
            OnBackwardEvent(-0.2f);
        else if(Input.GetKey(KeyCode.D))
            OnBackwardEvent(-0.7f);
    }

    public void OnForwardEvent(float value)
    {
        Debug.Log("forwarddevent " + value);

        CharacterController.Move(Vector3.forward * ((1 + value) * Speed));
        characterAnimator.SetFloat(SpeedAnimationKey,value);
    }
    
    public void OnBackwardEvent(float value)
    {
        Debug.Log("backwardevent " + value);
        CharacterController.Move(Vector3.back * ((1 + value) * Speed));
        characterAnimator.SetFloat(SpeedAnimationKey,value);

    }
    
    public void OnRotateClockwiseEvent(float value)
    {
        CharacterController.transform.Rotate(Vector3.up, -value);
    }
    
    public void OnRotateCounterClockwiseEvent(float value)
    {
        CharacterController.transform.Rotate(Vector3.up, -value);
    }

    public void ResetLocation()
    {
        transform.position = new Vector3(0, 0, 4);
    }
}
