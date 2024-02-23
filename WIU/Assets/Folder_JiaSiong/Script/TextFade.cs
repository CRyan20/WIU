using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    public TextMeshProUGUI textMeshProComponent;
    public float fadeInDuration = 2.0f;

    void Start()
    {
        // Ensure the TextMeshPro component is not visible initially
        Color textColor = textMeshProComponent.color;
        textColor.a = 0f;
        textMeshProComponent.color = textColor;

        // Set the TextMeshPro component active
        textMeshProComponent.gameObject.SetActive(true);

        // Start the fade-in coroutine
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color textColor = textMeshProComponent.color;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            textColor.a = alpha;
            textMeshProComponent.color = textColor;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final alpha value is exactly 1
        textColor.a = 1f;
        textMeshProComponent.color = textColor;
    }
}

