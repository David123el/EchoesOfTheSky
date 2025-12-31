using UnityEngine;
using UnityEngine.UI;

public class ListeningMeterUI : MonoBehaviour
{
    [SerializeField] private ListeningMeter meter;
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;

    void Update()
    {
        if (meter == null) return;

        float normalized = meter.CurrentMeter / meter.MaxMeter;
        fillImage.fillAmount = normalized;

        // צבע לפי עומס
        if (normalized < 0.6f)
            fillImage.color = normalColor;
        else if (normalized < 0.85f)
            fillImage.color = warningColor;
        else
            fillImage.color = dangerColor;

        // נראות — מופיע רק כשיש פעילות
        bool visible = meter.CurrentMeter > 0f || meter.IsListening;
        canvasGroup.alpha = visible ? 1f : 0f;
    }
}
