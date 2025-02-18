using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shieldBreakSound;
    public AudioClip deactivateShield;
    public AudioClip activateShield;

    public ShieldBlinkEffect blinkEffect;

    //private void Start()
    //{
    //    audioSource = GetComponentInChildren<AudioSource>();
    //    Debug.Log("ShieldCollision audioSource: " + audioSource);
    //}
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponentInChildren<AudioSource>();
        }

        if (activateShield == null)
        {
            Debug.LogWarning("ShieldActivateSound is not assigned in the Inspector.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            blinkEffect.BreakShield();
        }
    }

    public void Activate()
    {
        if (activateShield != null && audioSource != null)
        {
            audioSource.PlayOneShot(activateShield);
        }
        else
        {
            Debug.LogWarning("ShieldActivateSound or AudioSource is not assigned!");
        }
    }

    public void Deactivate()
    {
        StartCoroutine(DeactivateAfterSound());
    }

    private IEnumerator DeactivateAfterSound()
    {
        audioSource.PlayOneShot(deactivateShield);
        yield return new WaitForSeconds(deactivateShield.length);
        gameObject.SetActive(false);
    }

}
