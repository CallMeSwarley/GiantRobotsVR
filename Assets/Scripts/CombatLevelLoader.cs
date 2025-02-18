using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatLevelLoader : MonoBehaviour
{
    public GameObject easyOpponent, mediumOpponent, hardOpponent;
    
    public PlayerStats fpsPlayerStats; 
    
    
    //private void Awake()
    //{
    //    PlayerPrefs.SetInt("wonCount", 2);
    //}

    void Start()
    {
        easyOpponent.SetActive(false);
        mediumOpponent.SetActive(false);
        hardOpponent.SetActive(false);
        
        switch (PlayerPrefs.GetInt("wonCount",0))
        {
            case 0:
                easyOpponent.SetActive(true); 
                fpsPlayerStats.opponent = easyOpponent.transform;
                break;
            case 1:
                mediumOpponent.SetActive(true);
                fpsPlayerStats.opponent = mediumOpponent.transform;
                break;
            case 2:
                hardOpponent.SetActive(true);
                fpsPlayerStats.opponent = hardOpponent.transform;
                break;
            default:
                easyOpponent.SetActive(true);
                fpsPlayerStats.opponent = easyOpponent.transform;
                break; 
        }
    }

}
