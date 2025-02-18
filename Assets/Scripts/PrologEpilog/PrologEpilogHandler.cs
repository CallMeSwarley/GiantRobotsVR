using Meta.WitAi.TTS.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologEpilogHandler : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    public TTSSpeaker speaker;
    public Canvas canvas;
    private TTSScript ttsScript;
    private bool addBlink = false;

    public virtual void Start()
    {
        ttsScript = GetComponent<TTSScript>();
        canvas.gameObject.SetActive(true);
        uiText.text = $"Karmascore: {PlayerPrefs.GetInt("karmaScore")}/100\n";
    }

    /// <summary>
    /// This doesnt work in build, use TypeTextWithBlinkAsync instead
    /// </summary>
    protected IEnumerator TypeTextWithBlink(string prologText, TextMeshProUGUI uiText, System.Action callback)
    {
        //StartCoroutine(ttsScript.SpeakAsync(prologText.Replace("\n", " "), speaker));
        ttsScript.Speak(prologText.Replace("\n", " "), speaker);
        prologText += "...";
        uiText.text = "|";
        //yield return Blink(uiText, 6, 0.3f);

        foreach (char c in prologText)
        {
            string baseText = uiText.text.TrimEnd('|');
            uiText.text = baseText + c;

            if (addBlink)
            {
                uiText.text += "|";
            }
            Canvas.ForceUpdateCanvases();
            addBlink = !addBlink;
            yield return new WaitForSeconds(0.045f);
        }

        //yield return Blink(uiText, 11, 0.3f);
        //canvas.gameObject.SetActive(false);
        callback?.Invoke();
        yield return Blink(uiText, 11, 0.3f);
    }

    private IEnumerator Blink(TextMeshProUGUI uiText, int blinkCount, float blinkInterval)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            string baseText = uiText.text.TrimEnd('|');
            uiText.text = addBlink ? baseText + "|" : baseText;

            addBlink = !addBlink;
            yield return new WaitForSeconds(blinkInterval);
        }
    }


    protected async void TypeTextWithBlinkAsync(string prologText, TextMeshProUGUI uiText, System.Action callback)
    {
        ttsScript.Speak(prologText.Replace("\n", " "), speaker);
        prologText += "...";
        uiText.text += "|";
        await BlinkAsync(uiText, 6, 300);
        foreach (char c in prologText)
        {
            string baseText = uiText.text.TrimEnd('|');
            uiText.text = baseText + c;

            if (addBlink)
            {
                uiText.text += "|";
            }
            Canvas.ForceUpdateCanvases();
            addBlink = !addBlink;

            await Task.Delay(45);  // Delay in milliseconds (0.045 seconds)
        }
        await BlinkAsync(uiText, 11, 300);
        callback?.Invoke();
    }

    private async Task BlinkAsync(TextMeshProUGUI uiText, int blinkCount, int blinkInterval)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            string baseText = uiText.text.TrimEnd('|');
            uiText.text = addBlink ? baseText + "|" : baseText;

            addBlink = !addBlink;
            await Task.Delay(blinkInterval);
        }
        uiText.enabled = true;
    }
}
