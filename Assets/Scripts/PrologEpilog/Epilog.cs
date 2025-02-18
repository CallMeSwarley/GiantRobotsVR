using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Epilog : PrologEpilogHandler
{
    private string epilogTextWon = "Against all odds, you emerged victorious, earning your place back on Earth. " +
        "The forbidden mask's burden is lifted, and your story becomes a testament to perseverance and triumph in the face of despair. " +
        "Welcome home champion. Please take off the mask";
    private string epilogTextLost = "Defeated, the weight of the forbidden mask crushes your spirit. " +
        "Mars becomes your eternal prison, a desolate monument to failure. Yet in your loss, whispers of " +
        "your struggle inspire others to defy the odds you could not";
    public override void Start()
    {
        base.Start();
        TypeTextWithBlinkAsync(PlayerPrefs.GetInt("wonCount") == 3 ? epilogTextWon : epilogTextLost, uiText, () =>
        {
            PlayerPrefs.SetInt("TutorialCompleted", -1);
            PlayerPrefs.Save();
            // TODO: wait until user takes off the mask
            OVRManager.HMDUnmounted += UnmountHandler;
        });
    }
    void UnmountHandler()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        OVRManager.HMDUnmounted -= UnmountHandler;  // Unsubscribe
    }
}
