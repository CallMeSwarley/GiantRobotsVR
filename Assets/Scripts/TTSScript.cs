using Meta.WitAi.TTS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTSScript : MonoBehaviour // This script is attached to an GameObject in the scene and should do the talking
{
    public IEnumerator SpeakAsync(string text, TTSSpeaker tts)
    {
        if (text.Length > 280)
        {
            List<string> textChunks = SplitTextIntoChunks(text, 280);
            yield return PlayChunksSequentiallyAsync(textChunks, tts);
        }
        else
        {
            yield return tts.SpeakAsync(text);
        }
    }
    public void Speak(string text, TTSSpeaker tts)
    {
        Debug.Log("Text Length: " + text.Length);
        if (text.Length > 280)
        {
            List<string> textChunks = SplitTextIntoChunks(text, 280);
            StartCoroutine(PlayChunksSequentially(textChunks, tts));
        }
        else
        {
            tts.Speak(text);
        }
    }
    private List<string> SplitTextIntoChunks(string text, int chunkLength)
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
            Debug.Log("Chunk: " + chunk);
        }

        return chunks;
    }
    private IEnumerator PlayChunksSequentially(List<string> chunks, TTSSpeaker tts)
    {
        foreach (string chunk in chunks)
        {
            tts.SpeakQueued(chunk); // Play the TTS for the current chunk

            // Wait for the current chunk to finish playing before proceeding
            yield return new WaitUntil(() => !tts.IsSpeaking);
        }
    }
    private IEnumerator PlayChunksSequentiallyAsync(List<string> chunks, TTSSpeaker tts)
    {
        foreach (string chunk in chunks)
        {
            tts.SpeakQueuedAsync(chunk); // Play the TTS for the current chunk

            // Wait for the current chunk to finish playing before proceeding
            yield return new WaitUntil(() => !tts.IsSpeaking);
        }
    }
}
