using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this script to a parent GameObject with a ParticleSystem component as childs, to destroy the GameObject when all its child particle systems have stopped.
/// </summary>
public class DestroyParticleSystem : MonoBehaviour
{
    private ParticleSystem[] childParticleSystems;
    private AudioSource audioSource;
    private AudioClip impactSound;
    void Start()
    {
        // Get all ParticleSystem components in the parent and its children
        childParticleSystems = GetComponentsInChildren<ParticleSystem>();
        audioSource = GetComponentInChildren<AudioSource>();
        impactSound = Resources.Load<AudioClip>("Audio/impact");
        if (audioSource != null && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
    }

    void Update()
    {
        bool allStopped = true;

        foreach (ParticleSystem ps in childParticleSystems)
        {
            if (ps.IsAlive())
            {
                allStopped = false;
                break;
            }
        }
        if(audioSource != null && audioSource.isPlaying)
        {
            allStopped = false;
        }

        if (allStopped)
        {
            Destroy(gameObject);
        }
    }
}
