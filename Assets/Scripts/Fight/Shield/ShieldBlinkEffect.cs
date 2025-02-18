using System.Collections;
using UnityEngine;

public class ShieldBlinkEffect : MonoBehaviour
{
    public float blinkInterval = 0.05f;
    public int blinkCount = 10;

    private Material shieldMaterial;
    private Color originalColor;
    private Color blinkColor = new Color(1f, 0f, 0f, 0.39f);

    void Start()
    {
        shieldMaterial = gameObject.GetComponent<Renderer>().material;
        originalColor = shieldMaterial.color;
    }

    public void BreakShield()
    {
        StartCoroutine(BlinkAndVanish());
    }

    private IEnumerator BlinkAndVanish()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            shieldMaterial.color = blinkColor;

            yield return new WaitForSeconds(blinkInterval);

            shieldMaterial.color = originalColor;

            yield return new WaitForSeconds(blinkInterval);
        }
        gameObject.SetActive(false);
    }
}