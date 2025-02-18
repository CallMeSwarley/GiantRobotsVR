using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    public float shakeAmount = 0.2f;
    public float shakeDuration = 0.5f;
    public AnimationCurve intensityOverTime;

    private Vector3 originalPosition;

    private void Awake()
    {
        // Default AnimationCurve if not set in Inspector
        if (intensityOverTime == null || intensityOverTime.length == 0)
        {
            intensityOverTime = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }
    }

    public void Shake()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float progress = elapsed / shakeDuration;
            float currentIntensity = shakeAmount * intensityOverTime.Evaluate(progress);

            float x = Random.Range(-currentIntensity, currentIntensity);
            float y = Random.Range(-currentIntensity, currentIntensity);
            float zRotation = Random.Range(-currentIntensity * 10, currentIntensity * 10); // Optional rotation

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            transform.localRotation = Quaternion.Euler(0, 0, zRotation);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
    }
}

