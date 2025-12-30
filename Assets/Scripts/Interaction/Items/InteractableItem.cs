using UnityEngine;
using UnityEngine.Events;

public class InteractableItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public ItemType itemType = ItemType.SoundShard;
    public int value = 1;

    [Header("Visual & Audio")]
    public GameObject collectEffect;
    public AudioClip collectSound;

    [Header("Events")]
    public UnityEvent<int> onCollected; // returns value

    private bool collected = false;

    public void Interact(GameObject interactor)
    {
        if (collected) return;
        collected = true;

        // Notify game systems
        onCollected?.Invoke(value);

        // Optional: send to GameManager
        GameManager.Instance?.AddItem(itemType, value);

#if UNITY_EDITOR
        // Debug: show on screen which item has been collected
        DebugUIManager.Instance?.ShowMessage($"Collected {itemType} (+{value})");
#endif

        // FX
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        Destroy(gameObject);
    }

    public string GetPromptText()
    {
        return "Collect";
    }
}
