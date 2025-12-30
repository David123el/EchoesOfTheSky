using UnityEngine;
using System;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRadius = 1.2f;
    [SerializeField] private LayerMask interactableLayer;

    public static event Action<string> OnShowPrompt;
    public static event Action OnHidePrompt;

    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            interactRadius,
            interactableLayer
        );

        if (hit == null)
        {
            ClearInteractable();
            return;
        }

        IInteractable interactable = hit.GetComponent<IInteractable>();

        if (interactable != null && interactable != currentInteractable)
        {
            currentInteractable = interactable;
            OnShowPrompt?.Invoke(interactable.GetPromptText());
        }
    }

    private void ClearInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            OnHidePrompt?.Invoke();
        }
    }

    // נקרא ע"י Send Messages
    public void OnInteract()
    {
        if (currentInteractable == null) return;

        currentInteractable.Interact(gameObject);
        ClearInteractable();
    }
}
