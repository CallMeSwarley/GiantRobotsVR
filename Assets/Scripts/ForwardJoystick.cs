using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class ForwardJoystick : MonoBehaviour
{
    public Action<float> forwardAction;
    public Action<float> backwardAction;
    public UnityEvent<float> actionEvent;
    public Transform JoystickRod;
    public OneGrabRotateTransformer oneGrabRotateTransformer;
    private Vector3 initialEulerRotation;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = JoystickRod.rotation;
        initialEulerRotation = JoystickRod.eulerAngles;
        initialPosition = JoystickRod.position;
    }

    public void ResetEverything()
    {
        JoystickRod.rotation = initialRotation;
        JoystickRod.eulerAngles = initialEulerRotation;
        JoystickRod.position = initialPosition;
        actionEvent.Invoke(0);
    }

    private void Update()
    {
        MakeMoveRod();
    }

    public void MakeMoveRod()
    {
        float eulerAnglesX = JoystickRod.eulerAngles.x;

        if (eulerAnglesX > 5 && eulerAnglesX <= oneGrabRotateTransformer.Constraints.MaxAngle.Value)
        {
            actionEvent.Invoke(eulerAnglesX / oneGrabRotateTransformer.Constraints.MaxAngle.Value);
        }
        else if (eulerAnglesX < 355 && eulerAnglesX >= 360+oneGrabRotateTransformer.Constraints.MinAngle.Value)
        {
            actionEvent.Invoke((360 - eulerAnglesX) / (oneGrabRotateTransformer.Constraints.MinAngle.Value));
        }
    }
}