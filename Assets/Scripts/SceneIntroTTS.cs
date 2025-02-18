using Meta.WitAi.TTS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneIntroTTS : MonoBehaviour
{
    [Tooltip("This tts text has a limit of 280 characters.")]
    public string text;
    private TTSSpeaker speaker;
    string baseMessage = "Gather info from wandering npcs, shop for upgrades, or negotiate deals.";
    string nextOpponentMessage = "When you're ready, step into the arena to face your next opponent!";
    string welcomeBackMessage = "Welcome back! You've made it through the {0} round.";

    // Start is called before the first frame update
    void Start()
    {
        speaker = GetComponentInChildren<TTSSpeaker>();
        if (speaker != null)
        {
            if (text == null || text == "")
            {
                int wonCount = PlayerPrefs.GetInt("wonCount");
                text = wonCount switch
                {
                    1 => string.Format(welcomeBackMessage, "first") + " " + baseMessage + " " + nextOpponentMessage,
                    2 => string.Format(welcomeBackMessage, "second") + " One more to go! " + baseMessage + " " + nextOpponentMessage,
                    _ => "Welcome to Mars! " + baseMessage + " Once you're prepared, step into the arena and face your first opponent!"
                };
                // Limit text to 280 characters otherwise TTS would fail
                Debug.Log("Text length: " + text.Length);
                text = text.Length > 280 ? text.Substring(0, 280) : text;
            }
            speaker.Speak(text);
        }
    }
}
