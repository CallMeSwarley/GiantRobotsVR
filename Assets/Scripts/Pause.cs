using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

/// <summary>
/// Add this to one of your game objects to pause the game when the time scale is set to 0. Not needed if GameManager is used!
/// </summary>
public class Pause : MonoBehaviour
{
    //private bool paused = false;
    ////public FPSController fpsController;
    //void TogglePauseGame()
    //{
    //    paused = !paused;
    //    if (paused)
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //    }
    //    else if (!paused)
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //        Cursor.visible = false;
    //    }
    //    //fpsController.canMove = !paused ;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Time.timeScale == 0f && !paused)
    //    {
    //        TogglePauseGame();
    //    }
    //    else if (Time.timeScale == 1f && paused)
    //    {
    //        TogglePauseGame();
    //    }
    //}
}
