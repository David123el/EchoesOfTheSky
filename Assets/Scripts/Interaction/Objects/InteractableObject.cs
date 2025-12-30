using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [Tooltip("What the interaction is called. Example: 'Open', 'Collect', 'Talk'")]
    public string interactionName = "Interact";

    [Tooltip("Should the player see a highlight or effect when close?")]
    public bool highlightOnApproach = true;

    [Tooltip("If true, object can be interacted with multiple times.")]
    public bool repeatable = false;

    [Header("Events")]
    public UnityEvent onInteract;

    private bool hasInteracted = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // Called by PlayerInteraction script
    public void Interact(GameObject interactor)
    {
        if (!repeatable && hasInteracted)
            return;

        hasInteracted = true;

        onInteract?.Invoke();

#if UNITY_EDITOR
        //Debug: show on screen which event has been called
        DebugUIManager.Instance?.ShowMessage($"{interactionName} triggered");
#endif
    }

    public string GetPromptText()
    {
        return "Use";
    }

    // Called when player enters interaction range
    public void Highlight()
    {
        if (!highlightOnApproach || spriteRenderer == null)
            return;

        spriteRenderer.color = new Color(originalColor.r * 1.2f, originalColor.g * 1.2f, originalColor.b * 1.2f);
    }

    // Called when player leaves interaction range
    public void RemoveHighlight()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = originalColor;
    }
}
