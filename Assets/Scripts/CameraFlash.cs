using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    [SerializeField] private Image flashPanel;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float maxAlpha = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;

    private void Awake()
    {
        // Ensure panel exists and starts invisible
        if (flashPanel != null)
        {
            Color startColor = flashPanel.color;
            startColor.a = 0f;
            flashPanel.color = startColor;
        }
    }

    public void Flash()
    {
        if (flashPanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine());
        }
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsedTime = 0f;
        Color panelColor = flashPanel.color;

        while (elapsedTime < flashDuration)
        {
            float normalizedTime = elapsedTime / flashDuration;
            float curveValue = flashCurve.Evaluate(normalizedTime);
            
            panelColor.a = maxAlpha * curveValue;
            flashPanel.color = panelColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure panel ends invisible
        panelColor.a = 0f;
        flashPanel.color = panelColor;
    }
}