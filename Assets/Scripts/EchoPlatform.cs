using UnityEngine;
using System.Collections;

public class EchoPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer platformRenderer;

    [Header("Settings")]
    [SerializeField] private float disappearDelay = 0.3f;

    private bool playerInsideZone = false;
    private Coroutine disappearRoutine;

    void Awake()
    {
        SetActive(false);
    }

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningStarted += HandleListeningStarted;
        ListeningManager.Instance.OnListeningStopped += HandleListeningStopped;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance == null) return;

        ListeningManager.Instance.OnListeningStarted -= HandleListeningStarted;
        ListeningManager.Instance.OnListeningStopped -= HandleListeningStopped;
    }

    void HandleListeningStarted()
    {
        if (!playerInsideZone) return;

        if (disappearRoutine != null)
        {
            StopCoroutine(disappearRoutine);
            disappearRoutine = null;
        }

        SetActive(true);
    }

    void HandleListeningStopped()
    {
        if (!platformCollider.enabled) return;

        if (disappearRoutine != null)
            StopCoroutine(disappearRoutine);

        disappearRoutine = StartCoroutine(DisappearAfterDelay());
    }

    IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);
        SetActive(false);
        disappearRoutine = null;
    }

    void SetActive(bool active)
    {
        platformCollider.enabled = active;
        platformRenderer.enabled = active;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInsideZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInsideZone = false;
    }
}
