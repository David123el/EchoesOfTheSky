using UnityEngine;

public class EchoZone : MonoBehaviour
{
    [SerializeField] private EchoPlatform platform;

    private int playersInside;

    void OnEnable()
    {
        if (ListeningManager.Instance != null)
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

        playersInside++;
        
        if (playersInside == 1 && ListeningManager.Instance.IsListening)
            platform.ActivateFromZone();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInside = Mathf.Max(0, playersInside - 1);
        
        if (playersInside == 0)
            platform.RequestDeactivate();
    }

    void HandleListeningChanged(bool isListening)
    {
        if (playersInside == 0)
            return;

        if (isListening)
            platform.ActivateFromZone();
    }
}
