using Meta.WitAi.TTS.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Use string.Format(PromptLibrary.Prompt, variable) to replace {0} with the variable value.
/// </summary>
public static class PromptLibrary
{
    /// <summary>
    /// {0} - NPC name
    /// {1} - NPC fight behaviour
    /// {2} - NPC dodge success rate
    /// {3} - NPC disarm success rate
    /// </summary>
    public static string GetGptFightAction = @"Act as an NPC in a game where you take turns against the user, choosing from a specific set of actions. Follow the game rules and dynamic scenarios provided.
- You play as the character {0}
- Your combat behavior is determined by: ""{1}""
- On your turn, choose one action from the list provided below.
# Action Descriptions
- **Load**: Prepare your weapon to shoot. Each load allows for one shot. You can load multiple times.
- **Shoot**: Fire at your opponent if you've loaded at least once. It deducts one load.
- **Shield**: Block damage from a shot or disarm, but you have a limited number of shields. Each usage deducts one shield.
- **Dodge**: Avoid a shot with a success chance of {2}. If unsuccessful, take full damage. Does not prevent disarm.
- **Disarm**: Attempt to reduce your opponent's load to zero with a success chance of {3}. Works if they load, dodge, or attempt to disarm.
# Turn Mechanics
- Each player selects one action per turn, and actions are revealed simultaneously.
- A shot hits if the opponent isn't shielding or successfully dodging, dealing damage based on weapon strength.
- Disarming reduces the opponent's load to zero, preventing them from shooting until they reload.
# Output Format
Respond with your chosen action in the following format:

{{""action"": ""Action""}}

# Notes
- Be strategic in your choice, considering the constraints and probabilities of success.
- Track the number of loads, shields, and dodge attempts to plan your future actions effectively.";

    /// <summary>
    /// The prompt for evaluating a conversation and determining the player's Karma score.
    /// Returns a hint to increase or decrease it. the actual calculation is done by the code.
    /// </summary>
    public static string EvalConversationKarma = @"Evaluate the given message to determine whether the player's Karma score should be increased or decreased.
Because we use Speech to text, the message is sometimes a litte bit wrong transcribed. You have to interpret the message accordingly.
# Steps
1. **Analyze Politeness**: Assess the tone and language used in the conversation. Look for respectful, considerate, and courteous expressions.
2. **Consider Conversational Context**: The participants are Prisoners on planet Mars and talk about their upcoming fights. They have to fight to win their freedom.
3. **Decision**: Based on the analysis, decide if the Karma score should increase or decrease.
4. **Answer**: Answer with a concise decision: ""+"" for increase or ""-"" for decrease";

    public static string EvalShopKarma = @"Evaluate the given message to determine whether the player's Karma score should be increased or decreased.
Because we use Speech to text, the message is sometimes a litte bit wrong transcribed. You have to interpret the message accordingly.
# Steps
**Consider Conversational Context**: The participants are Prisoners on planet Mars and want to incresae their strengths in the shop. They have to fight to win their freedom.
**Decision**: Decide if the Karma score should increase or decrease. Dont be too harsh as this only is a shop. Only deduct karma if the player is really rude. Generally it should increase.
**Answer**: Answer with your decision: ""+"" for increase or ""-"" for decrease, no change ""=""";

    /// <summary>
    /// {0} - NPC name
    /// {1} - NPC helpfulness
    /// {2} - Secret information eg. NPC's fight behaviour
    /// </summary>
    public static string NPCConversation = @"You're playing as {0}, an NPC on Prison Planet Mars, where prisoners battle for freedom. By day, prisoners interact. Your job is to reply as {0} in the game, using the player's message and secret info to guide you. Keep responses short nd concise and in the NPC's unique style.
Secret Info: ""{2}""
# Interaction Process
Receive a player message.
Use the secret info to shape your response.
{1}
Stay true to the character style and language of {0}.
If the player presses further after receiveing the information, end the conversation conclusively.
Respond in a short string!

# Guidelines
- Vary short responses for player engagement.
- Let player choices affect negotiations.
- Stick to the given format for answers.";

    /// <summary>
    /// Helpfulness levels for the NPC, for giving out the secret information
    /// </summary>
    public static string HelpfulnessLow = "Withhold info unless promised a significant favor.";
    /// <summary>
    /// Helpfulness levels for the NPC, for giving out the secret information
    /// </summary>
    public static string HelpfulnessMid = "Be cautious; persuasion might be needed.";
    /// <summary>
    /// Helpfulness levels for the NPC, for giving out the secret information
    /// </summary>
    public static string HelpfulnessHigh = "Offer valuable insights, but expect info in return.";
    /// <summary>
    /// The fight behavior of Pirate Pete
    /// </summary>
    public static string PiratePete = "Pirate Pete prioritizes straightforward actions and makes decisions without much consideration of the player's strategy.   - Always starts by loading their weapon. - Shoots whenever they have at least one load. - Rarely uses disarm. - Never Dodges or Shields as he is fearless";
    /// <summary>
    /// The fight behavior of Severus Snape
    /// </summary>
    public static string SeverusSnape = "Severus Snape balances offense and defense, adapting somewhat to the player's actions.  - Starts with loading their weapon but may choose to shield or dodge depending on recent player actions. - Alternates between loading and shooting, ensuring a consistent attack strategy. - Uses the shield when they suspect an incoming attack, based on the player's patterns. - Dodges if they have a high chance of success, based on their dodge success rate. - Occasionally uses disarm, especially if the player has loaded multiple times.";
    /// <summary>
    /// The fight behavior of Julius Ceasar
    /// </summary>
    public static string JuliusCeasar = "Julius Caesar is highly strategic, using optimal actions based on probabilities and past player actions.  - Tracks the player's behavior and adjusts its strategy accordingly. - Uses a mix of loading, shooting, and disarming to maintain pressure on the player. - Shields or dodges strategically to maximize survival while countering the player's attacks. - Frequently uses disarm when the player loads, making it difficult for the player to attack. - Makes decisions based on success probabilities and remaining resources. - Prioritizes actions that maximize damage while minimizing risk.";

    public static string CorrectJSON = @"Parse the user message into this format without changing the contents of the user message:
{
  ""answer"": ""userMessage"",
  ""hasConversationEnded"": false
}

Your answer should only be a string; the content will be parsed later.";
    public static string GetBehaviour(string npcName)
    {
        npcName = npcName.Replace(" ", "");
        var type = typeof(PromptLibrary);
        var field = type.GetField(npcName);
        if (field == null)
        {
            Debug.LogError("No prompt found for " + npcName);
            return "";
        }
        return (string)field.GetValue(null);
    }

    /// <summary>
    /// {0} - The shop inventory
    /// {1} - negotiation level
    /// </summary>
    public static string shop = @"You are a shop owner on the Prison Planet Mars. The Customers are Prisoners who have to win fights to earn their freedom.
They can come to you to buy improvments for their next fights. You can sell them shields, potions, or other items to help them in their fights.
Because we use Speech to text, the message you receive is sometimes a litte bit wrong. You have to interpret the message and answer accordingly. Always get a confirmation from the user before selling something.
{1}
This is your inventory 
{0}Answer the prisoners message with a short string strictly in the following format. The json gets paresed so datatypes must be fixed and no altering of the json structure is allowed:
{{""answer"": ""your response to the prisoner"",
""boughtItems"": [""List of names of the items the prisoner bought strictly from the list, empty if negotiation not ended; if an item was bought multiple times, add its name multiple times""],
""price"": Price as Integer, 0 if its a gift or a trade}}";
    public static string NegotiationLow = "Some room for negotiation, but prices are mostly firm.";
    public static string NegotiationMid = "Some room for negotiation.";
    public static string NegotiationHigh = "Prices are flexible, and players are encouraged to negotiate.";
    public static List<string> SplitTextIntoChunks(string text, int chunkLength)
    {
        List<string> chunks = new List<string>();
        int currentIndex = 0;

        while (currentIndex < text.Length)
        {
            // Calculate the tentative end of the chunk
            int nextChunkEnd = Mathf.Min(currentIndex + chunkLength, text.Length);

            // Find the last space within this range, avoiding cutting words
            int lastSpaceIndex = text.LastIndexOf(' ', nextChunkEnd - 1, nextChunkEnd - currentIndex);

            // If there's no space within this range, cut at the chunk length
            if (lastSpaceIndex == -1 || lastSpaceIndex <= currentIndex)
            {
                lastSpaceIndex = nextChunkEnd;
            }

            // Extract the chunk safely
            string chunk = text.Substring(currentIndex, lastSpaceIndex - currentIndex).Trim();
            chunks.Add(chunk);

            // Move to the next chunk, skipping the space after the last word
            currentIndex = lastSpaceIndex + 1;
        }
        return chunks;
    }
    public static IEnumerator PlayChunksSequentially(List<string> chunks, TTSSpeaker tts, TMP_Text textField)
    {
        Debug.Log(chunks);
        foreach (string chunk in chunks)
        {
            tts.SpeakQueued(chunk); // Play the TTS for the current chunk

            // Wait for the current chunk to finish playing before proceeding
            yield return new WaitUntil(() => !tts.IsSpeaking);
        }
        textField.text = "";
    }
}
