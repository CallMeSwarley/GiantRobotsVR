using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBreathing : MonoBehaviour
{
    public float scaleSpeed = 3f;
    public float scaleAmount = 0.01f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
        transform.localScale = originalScale + Vector3.one * scaleFactor;
    }
}
