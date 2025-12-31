using Unity.VisualScripting;
using UnityEngine;

public class ListeningZone : MonoBehaviour
{
    [SerializeField] private HiddenPlatform platform;
    private bool playerInside;

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningStarted += HandleListeningStarted;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance == null) return;

        ListeningManager.Instance.OnListeningStarted -= HandleListeningStarted;
    }

    void HandleListeningStarted()
    {
        if (!playerInside) return;

        if (platform == null)
        {
            Debug.LogError("HiddenPlatform not assigned in ListeningZone");
            return;
        }

        platform.Reveal();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
