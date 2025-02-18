using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFightTrigger : MonoBehaviour
{
    public Transform playerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == playerTransform)
        {
            SceneManager.LoadScene("CombatScene");
        }
    }
}
