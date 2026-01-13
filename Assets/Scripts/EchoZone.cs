using UnityEngine;

public class EchoZone : MonoBehaviour
{
    [SerializeField] private EchoPlatform platform;

    private bool playerInside;

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningChanged += HandleListeningChanged;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance != null)
            ListeningManager.Instance.OnListeningChanged -= HandleListeningChanged;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (ListeningManager.Instance.IsListening)
            platform.ActivateFromZone();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        platform.RequestDeactivate();
    }

    void HandleListeningChanged(bool isListening)
    {
        if (!playerInside) return;

        if (isListening)
            platform.ActivateFromZone();
        else
            platform.RequestDeactivate();
    }
}
