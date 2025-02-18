using Meta.WitAi.TTS.Utilities;
using Newtonsoft.Json;
using Oculus.Interaction;
using Oculus.Voice.Dictation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class ShopItem
{
    public string name;
    public string description;
    public int price;
}
[System.Serializable]
public class NPCShopAnswer
{
    public string answer;
    public List<string> boughtItems;
    public int price;
}
public class Shop : MonoBehaviour
{
    public GameObject parent;
    public AppDictationExperience dictationExperience;
    public TTSSpeaker tts;
    public TMP_Text textField;
    public TMP_Text answerTextField;
    public TMP_Text buttonText;
    public GameObject canvas;
    public GameObject recordingHint;
    public GameObject player;
    private PlayerStats playerStats;
    public float displayDistance = 10.0f;
    private List<ShopItem> shopItems;
    APIKeys keys = APIKeys.Load();
    private string apiUrl = "https://api.openai.com/v1/chat/completions";
    private Message prompt;
    private List<Message> chatHistory = new List<Message>();
    [SerializeField]
    private ActiveStateSelector[] sendMessagePose;
    [SerializeField]
    private ActiveStateSelector[] activateMicPose;
    private bool locked = false;
    // Start is called before the first frame update
    void Start()
    {
        if (canvas != null)
            canvas.SetActive(false);
        playerStats = player.GetComponent<PlayerStats>();
        playerStats.LoadGameSettings();
        int itemPrice = playerStats.itemPrice();
        shopItems = new List<ShopItem>()
        {
            new ShopItem() { name = "Chainreaction", description = "Chainweapon does 10 more damage", price = itemPrice},
            new ShopItem() { name = "Shield", description = "One more shield per fight", price = itemPrice },
            new ShopItem() { name = "Dodgedrink", description = "Increases your dodgingstrength by 10%", price = itemPrice },
            new ShopItem() { name = "Disarmpotion", description = "Increases your disarmstrength by 10%", price = itemPrice }
        };
    }

    private string inventoryString()
    {
        string inventory = "";
        foreach (ShopItem item in shopItems)
        {
            inventory += "Item: \"" + item.name + "\" Description: " + item.description + " Price: " + item.price + " coins\n";
        }
        return inventory;
    }

    private IEnumerator CallOpenAI(string message)
    {
        buttonText.text = "Thinking...";
        chatHistory.Add(new Message { role = "user", content = $"Prisoners balance {PlayerPrefs.GetInt("Money", 0)} coins." });
        chatHistory.Add(new Message { role = "user", content = message });
        var requestData = new OpenAIRequest
        {
            model = "gpt-4o",
            messages = chatHistory
        };
        string jsonData = JsonConvert.SerializeObject(requestData);
        StartCoroutine(UpdateKarma(message));
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
                    Debug.Log(reply);
                    NPCShopAnswer npcShopAnswer;
                    try
                    {
                        GameObject goToDestroy = new GameObject();
                        npcShopAnswer = JsonConvert.DeserializeObject<NPCShopAnswer>(reply);
                        chatHistory.Add(new Message { role = "assistant", content = reply });
                        answerTextField.text = npcShopAnswer.answer;
                        if (npcShopAnswer.boughtItems != null && npcShopAnswer.boughtItems.Count > 0)
                        {
                            goToDestroy = parent;
                            foreach (string item in npcShopAnswer.boughtItems)
                            {
                                switch (item)
                                {
                                    case "Chainreaction":
                                        PlayerPrefs.SetInt("shootDamage", PlayerPrefs.GetInt("shootDamage", 10) + 10);
                                        shopItems.RemoveAll(x => x.name == "Chainreaction");
                                        break;
                                    case "Shield":
                                        PlayerPrefs.SetInt("shieldCount", PlayerPrefs.GetInt("shieldCount", 3) + 1);
                                        shopItems.RemoveAll(x => x.name == "Shield");
                                        break;
                                    case "Dodgedrink":
                                        PlayerPrefs.SetFloat("dodgeSuccessRate", PlayerPrefs.GetFloat("dodgeSuccessRate", 0.5f) + 0.1f);
                                        shopItems.RemoveAll(x => x.name == "Dodgedrink");
                                        break;
                                    case "Disarmpotion":
                                        PlayerPrefs.SetFloat("disarmSuccessRate", PlayerPrefs.GetFloat("disarmSuccessRate", 0.5f) + 0.1f);
                                        shopItems.RemoveAll(x => x.name == "Disarmpotion");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            // wo dont check price, if user manages to convince LLM, then its what we want
                            PlayerPrefs.SetInt("Money", Mathf.Max(0, PlayerPrefs.GetInt("Money", 0) - npcShopAnswer.price));
                            PlayerPrefs.Save();
                            playerStats.LoadGameSettings();
                        }
                        buttonText.text = PlayerPrefs.GetInt("Money", -1) + " Coins";
                        if (npcShopAnswer.answer.Length > 280)
                        {
                            List<string> textChunks = PromptLibrary.SplitTextIntoChunks(npcShopAnswer.answer, 275);
                            StartCoroutine(WaitForTTSToFinishAndDestroy(goToDestroy));
                            StartCoroutine(PromptLibrary.PlayChunksSequentially(textChunks, tts, textField));
                        }
                        else
                        {
                            StartCoroutine(WaitForTTSToFinishAndDestroy(goToDestroy));
                            tts.Speak(npcShopAnswer.answer);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error: " + e.Message);
                        Debug.LogError("Error: " + reply);
                        textField.text = "Transaction failed";
                        tts.Speak("Transaction Failed!");
                    }
                }
                else
                {
                    Debug.LogError("Error: " + request.error);
                }
            }
            else
            {
                answerTextField.text = "Sorry, I cannot talk right now. Please try again later.";
                tts.Speak(answerTextField.text);
            }
        }
        locked = false;
    }

    private IEnumerator WaitForTTSToFinishAndDestroy(GameObject gameObjectToDestroy)
    {
        while (!tts.IsSpeaking)
        {
            yield return null; // Wait until the next frame.
        }
        while (tts.IsSpeaking)
        {
            yield return null; // Wait until the next frame.
        }
        Destroy(gameObjectToDestroy);
    }
    private void updatePrompt()
    {
        string negotiationLvl = PromptLibrary.NegotiationMid;
        if (playerStats.KarmaScore < 33)
            negotiationLvl = PromptLibrary.NegotiationLow;
        else if (playerStats.KarmaScore > 66)
            negotiationLvl = PromptLibrary.NegotiationHigh;
        string inventory = inventoryString();
        prompt = new Message
        {
            role = "system",
            content = string.Format(PromptLibrary.shop, inventory.Length > 0 ? inventory : "Inventory is empty!", negotiationLvl)
        };
        Debug.Log(prompt);
        if (chatHistory.Count > 0)
        {
            // first element is always the prompt
            chatHistory[0] = prompt;
        }
        else
            chatHistory.Add(prompt);
    }
    private IEnumerator UpdateKarma(string lastMessage)
    {
        Message prompt = new Message { role = "system", content = PromptLibrary.EvalShopKarma };
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player.transform)
        {
            for (int i = 0; i < activateMicPose.Length; i++)
            {
                activateMicPose[i].WhenSelected += () => TalkingWithHand();
            }
            //for (int i = 0; i < sendMessagePose.Length; i++)
            //{
            //    sendMessagePose[i].WhenSelected += () => CallOpenAIWithHand();
            //}
            updatePrompt();
            transform.LookAt(player.transform);
            ShowCanvasInFrontOfPlayer();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player.transform)
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
            canvas.SetActive(false);
            stopRecording();
        }
    }

    private void ShowCanvasInFrontOfPlayer()
    {
        if (canvas != null && player != null)
        {
            canvas.transform.LookAt(player.transform); // Make the canvas face the player
            canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - player.transform.position); // Adjust for proper facing

            // Activate the canvas
            canvas.SetActive(true);
        }
    }
    private void startRecording()
    {
        //buttonText.text = "Listening...";
        //recordingHint.SetActive(true);
        //dictationExperience.Activate();
    }

    /// <summary>
    /// This is called by the hand gesture detector for thumbs up to confirm the selected Button
    /// </summary>
    public void CallOpenAIWithHand()
    {
        if (canvas.activeSelf && !dictationExperience.Active && textField.text?.Length > 0 && !tts.IsSpeaking)
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

    private void stopRecording()
    {
        dictationExperience.Deactivate();
        buttonText.text = PlayerPrefs.GetInt("Money", -1) + " Coins";
        recordingHint.SetActive(false);
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
    //    // check if "W" is pressed and start mic
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        TalkingWithHand();
    //    }
    //}
}
