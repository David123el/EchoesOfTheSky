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

    [Header("Echo")]
    [SerializeField] private ParticleSystem echoParticles;

    private bool isActive = false;

    void Awake()
    {
        SetActive(false);

        if (echoParticles == null)
            echoParticles = GetComponentInChildren<ParticleSystem>();

        if (echoParticles != null)
            echoParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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

        if (!isActive)
        {
            SetActive(true);
            PlayEchoParticles(); // 👈 חיבור החלקיקים
        }
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
        isActive = active;

        platformCollider.enabled = active;
        platformRenderer.enabled = active;
    }

    void PlayEchoParticles()
    {
        if (echoParticles == null) return;

        echoParticles.Play();
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
