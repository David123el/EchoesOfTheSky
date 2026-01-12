using UnityEngine;

public class EchoZone : MonoBehaviour
{
    [SerializeField] private EchoPlatform platform;
    private bool playerInside;

    void Start()
    {
        // ğøùîéí ø÷ ëùäÎListeningManager áèåç ÷ééí
        ListeningManager.Instance.OnListeningStarted += OnListeningStarted;
    }

    void OnDestroy()
    {
        if (ListeningManager.Instance == null) return;
        ListeningManager.Instance.OnListeningStarted -= OnListeningStarted;
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
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void OnListeningStarted()
    {
        if (playerInside)
            platform.ActivateFromZone();
    }
}
