using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource audioSource;

    public void OnPlayButton()
    {
        StartCoroutine(PlaySoundAndLoadScene());
    }

    public void OnQuitButton()
    {
        StartCoroutine(PlaySoundAndQuit());
    }

    private IEnumerator PlaySoundAndLoadScene()
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSeconds(clickSound.length);
        SceneManager.LoadScene(1); // TODO add scenes to build settings
    }

    private IEnumerator PlaySoundAndQuit()
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSeconds(clickSound.length);

        Time.timeScale = 1f; // Unpause the game if it was paused

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
