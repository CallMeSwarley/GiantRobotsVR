using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Oculus.Voice;
using Oculus.Voice.Dictation;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS.Integrations;
using System;
using Unity.VisualScripting;
using System.Linq;
using Oculus.Interaction;

[System.Serializable]
public class OpenAIResponse
{
    public List<Choice> choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class OpenAIRequest
{
    public string model = "gpt-4o";
    public List<Message> messages;
}
[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

/// <summary>
/// Set the voce ID in the child TTS object of the gameobject
/// </summary>
public class VoiceScript : MonoBehaviour
{
    public AppDictationExperience dictationExperience;
    public TTSSpeaker tts;
    public Button ttsButton;
    public TMP_Text textField;
    public TMP_Text answerTextField;
    public TMP_Text buttonText;
    public GameObject canvas;
    public GameObject recordingHint;
    public string npcName;
    public RandomWander wanderScript;
    /// <summary>
    /// The name of the opponent this one has information about
    /// </summary>
    public string hasInfoAbout;
    APIKeys keys = APIKeys.Load();
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    private List<Message> chatHistory = new List<Message>();
    public Transform player;
    public PlayerStats playerStats;
    private string instructions = "";
    [SerializeField]
    private ActiveStateSelector[] sendMessagePose;
    [SerializeField]
    private ActiveStateSelector[] activateMicPose;
    private bool locked = false;
    private void Start()
    {
        if (canvas != null)
            canvas.SetActive(false);
    }
    void OnTTSButtonClick()
    {
        if (textField.text?.Length > 0)
            StartCoroutine(CallOpenAI(textField.text));
    }

    private IEnumerator CallOpenAI(string message)
    {
        buttonText.text = "Thinking...";
        // Construct the message payload
        chatHistory.Add(new Message { role = "user", content = message });

        var requestData = new OpenAIRequest
        {
            model = "gpt-4o",
            messages = chatHistory
        };
        string jsonData = JsonConvert.SerializeObject(requestData);

        // Set up the UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (keys != null)
            {
                request.SetRequestHeader("Authorization", "Bearer " + keys.OpenAIKey);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OpenAIResponse response = JsonConvert.DeserializeObject<OpenAIResponse>(request.downloadHandler.text);
                string reply = response.choices[0].message.content.Replace("```", "");
                reply = reply.Replace("json", "");
                if (reply != null && reply.Length > 0)
                {
                    buttonText.text = "Button";
                    answerTextField.text = reply;
                    chatHistory.Add(new Message { role = "assistant", content = reply });
                    if (reply.Length > 280)
                    {
                        List<string> textChunks = PromptLibrary.SplitTextIntoChunks(answerTextField.text, 275);
                        StartCoroutine(PromptLibrary.PlayChunksSequentially(textChunks, tts, textField));
                    }
                    else
                    {
                        tts.Speak(reply);
                    }
                    StartCoroutine(UpdateKarma(message));
                }
                else
                {
                    Debug.LogError("Error: " + request.error);
                    answerTextField.text = "Error: " + request.error;
                }
                //ttsButton.enabled = true;
            }
            else
            {
                answerTextField.text = "Sorry, I cannot talk right now. Please try again later.";
                tts.Speak(answerTextField.text);
            }
        }
        locked = false;
    }
    private IEnumerator UpdateKarma(string lastMessage)
    { // Construct the message payload
        Message prompt = new Message { role = "system", content = PromptLibrary.EvalConversationKarma };
        Message userMessage = new Message { role = "user", content = lastMessage };
        var requestData = new OpenAIRequest
        {
            model = "gpt-4o",
            messages = new List<Message> { prompt, userMessage }
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // Set up the UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (keys != null)
            {
                request.SetRequestHeader("Authorization", "Bearer " + keys.OpenAIKey);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OpenAIResponse response = JsonConvert.DeserializeObject<OpenAIResponse>(request.downloadHandler.text);
                string reply = response.choices[0].message.content;
                if (reply != null && reply.Length > 0)
                {
                    if (reply.Contains("+"))
                    {
                        playerStats.KarmaScore += 5;
                    }
                    else if (reply.Contains("-"))
                    {
                        playerStats.KarmaScore -= 5;
                    }
                    updatePrompt();
                }
                else
                {
                    Debug.LogError("Error: " + request.error);
                }
            }
        }
    }
    // TODO refactor and use the TTSScript

    private string chatHistoryAsString(List<Message> messages)
    {
        string output = "";
        foreach (Message message in messages)
        {
            output += message.role + ": " + message.content + "\n";
        }
        return output;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            for (int i = 0; i < activateMicPose.Length; i++)
            {
                activateMicPose[i].WhenSelected += () => TalkingWithHand();
            }
            //for (int i = 0; i < sendMessagePose.Length; i++)
            //{
            //    sendMessagePose[i].WhenSelected += () => CallOpenAIWithHand();
            //}
            wanderScript.canWander = false;
            playerStats.LoadGameSettings();
            updatePrompt();
            ttsButton.onClick.AddListener(OnTTSButtonClick);
            transform.LookAt(player);
            ShowCanvasInFrontOfPlayer();
        }
    }

    private void updatePrompt()
    {
        string helpfulness = PromptLibrary.HelpfulnessMid;
        if (playerStats.KarmaScore < 33)
            helpfulness = PromptLibrary.HelpfulnessLow;
        else if (playerStats.KarmaScore > 66)
            helpfulness = PromptLibrary.HelpfulnessHigh;
        instructions = string.Format(PromptLibrary.NPCConversation, npcName, helpfulness, PromptLibrary.GetBehaviour(hasInfoAbout));
        Debug.Log(instructions);
        if (chatHistory.Count == 0)
            chatHistory.Add(new Message { role = "system", content = instructions });
        else
            chatHistory[0] = new Message { role = "system", content = instructions };
    }

    private void ShowCanvasInFrontOfPlayer()
    {
        if (canvas != null && player != null)
        {
            canvas.transform.LookAt(player); // Make the canvas face the player
            canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - player.position); // Adjust for proper facing

            // Activate the canvas
            canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            locked = false;
            tts.Stop();
            for (int i = 0; i < activateMicPose.Length; i++)
            {
                activateMicPose[i].WhenSelected -= () => TalkingWithHand();
            }
            for (int i = 0; i < sendMessagePose.Length; i++)
            {
                sendMessagePose[i].WhenSelected -= () => CallOpenAIWithHand();
            }
            wanderScript.canWander = true;
            ttsButton.onClick.RemoveListener(OnTTSButtonClick);
            canvas.SetActive(false);
            stopRecording();
            textField.text = "";
            answerTextField.text = "";
        }
    }

    private void stopRecording()
    {
        buttonText.text = "Button";
        recordingHint.SetActive(false);
        dictationExperience.Deactivate();
    }

    /// <summary>
    /// This is called by the hand gesture detector for thumbs up to confirm the selected Button
    /// </summary>
    public void CallOpenAIWithHand()
    {
        if (canvas.activeSelf && !dictationExperience.Active && textField.text.Length > 0 && !tts.IsSpeaking)
        {
            stopRecording();
            StartCoroutine(CallOpenAI(textField.text));
        }
    }

    /// <summary>
    /// This is called by the hand gesture detector for fist to confirm the selected Button
    /// </summary>
    public void TalkingWithHand()
    {
        if (canvas.activeSelf && !dictationExperience.Active && !tts.IsSpeaking)
        {
            dictationExperience.Activate();
        }
    }
    public void AutomaticGPTAnswer()
    {
        if (!locked)
        {
            locked = true;
            Debug.Log("AutomaticGPTAnswer");
            StartCoroutine(CallOpenAI(textField.text));
        }

    }
    //private void Update()
    //{
    //    // check if "Space" is pressed and start mic
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        TalkingWithHand();
    //    }
    //}
}
