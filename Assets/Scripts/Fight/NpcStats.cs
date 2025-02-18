using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStats : Stats
{
    /// <summary>
    /// this name has to be the same as the variable in Promptlibrary, see FightBehaviour for clarification
    /// </summary>
    public string npcName;

    [HideInInspector]
    public string FightBehaviour
    {
        get
        {
            return PromptLibrary.GetBehaviour(npcName);
        }
    }
}
