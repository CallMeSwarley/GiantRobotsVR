using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource audioSource;
    public Button previousButton;
    public GameObject[] tutorials;
    private int currentTutorialIndex = 0;

    private void Start()
    {
        if(tutorials.Length == 0)
        {
            SceneManager.LoadScene("StartMenu");
            return;
        }
        previousButton.enabled = false;
        tutorials[currentTutorialIndex].SetActive(true);
    }
    public void OnNextButton()
    {
        StartCoroutine(PlaySoundAndExecute(() =>
        {
            previousButton.enabled = true;
            tutorials[currentTutorialIndex].SetActive(false);
            currentTutorialIndex++;
            if (currentTutorialIndex >= tutorials.Length)
            {
                PlayerPrefs.SetInt("TutorialCompleted", 1);
                SceneManager.LoadScene("StartMenu");
            }
            else
                tutorials[currentTutorialIndex].SetActive(true);
        }));
    }
    public void OnPreviousButton()
    {
        StartCoroutine(PlaySoundAndExecute(() =>
        {
            tutorials[currentTutorialIndex].SetActive(false);
            currentTutorialIndex--;
            if (currentTutorialIndex <= 0)
            {
                currentTutorialIndex = 0;
                previousButton.enabled = false;
            }
            tutorials[currentTutorialIndex].SetActive(true);
        }));
    }

    private IEnumerator PlaySoundAndExecute(System.Action callback)
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSecondsRealtime(clickSound.length); // need realtime otherwise callback isnt called in PauseGame
        EventSystem.current.SetSelectedGameObject(null);
        callback?.Invoke();
    }
}
