using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Prolog : PrologEpilogHandler
{
    //private string prologText = "YOU FOOL! You put on the forbidden mask of unbearable truths." +
    //    " As punishment for your disobedience you are teleported to the prison planet Mars." +
    //    " To earn one of the few tickets back to earth you will have to prove yourself " +
    //    "in a series of fights. Good luck";
    private string prologText = "YOU FOOL! You put on the forbidden mask of unbearable truths. " +
        "As punishment, you’re sent to the prison planet Mars. The mask triggered a chainreaction, each fight fuels chaos," +
        " each choice ripples outward. Prove yourself in battle, or be crushed by the chain. Good luck.";

    public override void Start()
    {
        base.Start();
        //StartCoroutine(TypeTextWithBlink(prologText, uiText, () => { SceneManager.LoadScene(2); }));
        TypeTextWithBlinkAsync(prologText, uiText, () => { SceneManager.LoadScene("WanderingScene"); }); 
    }
}

