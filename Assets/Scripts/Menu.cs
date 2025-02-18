using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource audioSource;
    public Button startButton;
    public Button resetButton;
    public Canvas pauseCanvas;
    public GameObject noConnectionHint;
    //private Microphone mic;

    private void Start()
    {
        Time.timeScale = 1f; // Unpause the game if it was paused
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("TutorialCompleted", -1) != 1)
        {
            startButton.gameObject.SetActive(false); 
        }
        // Reset the game settings to default for the game
        PlayerPrefs.SetInt("wonCount", 0);
        PlayerPrefs.SetInt("lifePoints", 100);
        PlayerPrefs.SetInt("shieldCount", 3);
        PlayerPrefs.SetInt("shootDamage", 20);
        PlayerPrefs.SetInt("loadCapacity", 3);
        PlayerPrefs.SetFloat("dodgeSuccessRate", 0.5f);
        PlayerPrefs.SetFloat("disarmSuccessRate", 0.5f);
        PlayerPrefs.SetInt("karmaScore", 50);
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.Save();
    }

    public void OnPlayButton()
    {
        if (!IsConnectedToInternet())
        {
            StartCoroutine(ShowNoConnectionHint());
            return;
        }
        else
            StartCoroutine(PlaySoundAndExecute(() => SceneManager.LoadScene("Prolog")));
    }

    public void OnQuitButton()
    {
        StartCoroutine(PlaySoundAndExecute(() =>
        {
            Time.timeScale = 1f; // Unpause the game if it was paused  

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
           Application.Quit();  
#endif
        }));
    }
    
    public void OnLevel1Button()
    {
        PlayerPrefs.SetInt("wonCount", 0);
    }

    public void OnLevel2Button()
    {
        PlayerPrefs.SetInt("wonCount", 1);

    }

    public void OnLevel3Button()
    {
        PlayerPrefs.SetInt("wonCount", 2);

    }
    
    public void OnAudioDemoButton()
    {
        if (!IsConnectedToInternet())
        {
            StartCoroutine(ShowNoConnectionHint());
            return;
        }
        else
            StartCoroutine(PlaySoundAndExecute(() => SceneManager.LoadScene("WanderingScene")));
    }

    public void OnTutorialButton()
    {
        if (!IsConnectedToInternet())
        {
            StartCoroutine(ShowNoConnectionHint());
            return;
        }
        else
            StartCoroutine(PlaySoundAndExecute(() => SceneManager.LoadScene("Tutorial")));
    }

    public void OnCombatButton()
    {
        if (!IsConnectedToInternet())
        {
            StartCoroutine(ShowNoConnectionHint());
            return;
        }
        else
            StartCoroutine(PlaySoundAndExecute(() => SceneManager.LoadScene("CombatScene")));
    }
    public void OnWanderingButton()
    {
        if (!IsConnectedToInternet())
        {
            StartCoroutine(ShowNoConnectionHint());
            return;
        }
        else
            StartCoroutine(PlaySoundAndExecute(() => SceneManager.LoadScene("WanderingScene")));
    }

    public void OnResetButton()
    {
        PlayerPrefs.DeleteAll();
    }
    
    private IEnumerator PlaySoundAndExecute(System.Action callback)
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSecondsRealtime(clickSound.length); // need realtime otherwise callback isnt called in PauseGame
        EventSystem.current.SetSelectedGameObject(null);
        callback?.Invoke();
    }

    private IEnumerator ShowNoConnectionHint()
    {
        if (noConnectionHint != null) noConnectionHint.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (noConnectionHint != null) noConnectionHint.SetActive(false);
    }

    private bool IsConnectedToInternet()
    {
        // Check if there is an internet connection
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false; // No connection
        }

        return true; // Connection is available
    }

    //private void OnDestroy()
    //{
    //    OVRManager.HMDMounted -= PauseGame;
    //}
}
