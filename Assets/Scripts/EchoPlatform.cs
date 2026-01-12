using UnityEngine;
using System.Collections;

public class EchoPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer platformRenderer;

    [Header("Settings")]
    [SerializeField] private float disappearDelay = 0.3f;

    private Coroutine disappearRoutine;
    private bool isActive;
    private int playersOnPlatform;

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

    public void ActivateFromZone()
    {
        if (disappearRoutine != null)
        {
            StopCoroutine(disappearRoutine);
            disappearRoutine = null;
        }

        if (!isActive)
            SetActive(true);
    }

    void HandleListeningStarted()
    {
        // ❗ EchoZone הוא זה שקורא ל־ActivateFromZone
        // כאן לא עושים כלום
    }

    void HandleListeningStopped()
    {
        if (!isActive) return;
        if (playersOnPlatform > 0) return;

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
        isActive = active;
        platformCollider.enabled = active;
        platformRenderer.enabled = active;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            playersOnPlatform++;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            playersOnPlatform = Mathf.Max(0, playersOnPlatform - 1);
    }
}
