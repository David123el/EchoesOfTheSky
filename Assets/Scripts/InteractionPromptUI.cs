using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;

    private void OnEnable()
    {
        PlayerInteraction.OnShowPrompt += Show;
        PlayerInteraction.OnHidePrompt += Hide;
    }

    private void OnDisable()
    {
        PlayerInteraction.OnShowPrompt -= Show;
        PlayerInteraction.OnHidePrompt -= Hide;
    }

    private void Show(string text)
    {
        promptRoot.SetActive(true);
        promptText.text = $"[E] {text}";
    }

    private void Hide()
    {
        promptRoot.SetActive(false);
    }
}
