using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;
    public GameObject impactEffect;


    private Transform target; // Opponent's transform
    private Vector3 initTargetPosition;
    private bool hit = true;
    private bool hasImpacted = false;

    // Set the target for the projectile
    public void SetTarget(Transform opponent, bool hit)
    {
        target = opponent;
        this.hit = hit;
        initTargetPosition = opponent.position;
        if (!hit)
        {
            initTargetPosition.y = 0;
        }
    }

    void Update()
    {
        if (hasImpacted)
            return;
        if (target != null)
        {
            Vector3 direction;
            if (hit)
            {
                // If hit is true, move towards the target
                direction = (target.position - transform.position).normalized;
            }
            else
            {
                direction = (initTargetPosition - transform.position);
                direction.Normalize();
            }

            // Move the projectile
            transform.position += direction * speed * Time.deltaTime;

            // Rotate so that the projectile is facing the movement direction
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction);
            transform.rotation = targetRotation;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target || other.CompareTag("Shield") || other.CompareTag("Floor") || other.CompareTag("Opponent"))
        {
            hasImpacted = true;
            DestroyChildren();
            if (impactEffect != null && hit)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
                StartCoroutine(ChainExplosion());
            }else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator ChainExplosion()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 2; i++)
        {
            float xOffset = Random.value > 0.5f ? -5f * (i + 1) : 5f * (i + 1);
            float zRotation = 90 * (i + 1);
            Instantiate(impactEffect, transform.position + new Vector3(xOffset, 5f * (i + 1), 0), Quaternion.Euler(0, 0, zRotation));
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }

    void DestroyChildren()
    {
        // Iterate through all child objects and destroy them
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
