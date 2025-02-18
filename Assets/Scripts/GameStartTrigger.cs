using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChild : MonoBehaviour
{
    public GameManager gameManagerEasyOpponent;
    public GameManager gameManagerMediumOpponent;
    public GameManager gameManagerHardOpponent;

    private GameManager selectedGameManager;

    private void Awake()
    {
        switch (PlayerPrefs.GetInt("wonCount",0))
        {
            case 0:
                selectedGameManager = gameManagerEasyOpponent;
                break;
            case 1:
                selectedGameManager = gameManagerMediumOpponent;
                break;
            case 2:
                selectedGameManager = gameManagerHardOpponent;
                break;
            default:
                selectedGameManager = gameManagerEasyOpponent;
                break; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        selectedGameManager.EnterGame(other);
        gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        selectedGameManager.ExitGame(other);
    }
}
