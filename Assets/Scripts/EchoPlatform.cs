using UnityEngine;
using System.Collections;

public class EchoPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer platformRenderer;
    [SerializeField] private ParticleSystem echoParticles;

    [Header("Settings")]
    [SerializeField] private float disappearDelay = 0.8f;

    private Coroutine disappearRoutine;
    private bool isActive;
    private int playersOnPlatform;

    void Awake()
    {
        SetActive(false);
    }

    public void ActivateFromZone()
    {
        if (isActive)
            return;

        if (disappearRoutine != null)
        {
            StopCoroutine(disappearRoutine);
            disappearRoutine = null;
        }

        SetActive(true);
    }

    public void RequestDeactivate()
    {
        if (!isActive)
            return;

        if (playersOnPlatform > 0)
            return;

        if (disappearRoutine == null)
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

        if (echoParticles != null)
        {
            if (active && !echoParticles.isPlaying)
                echoParticles.Play();
            else if (!active && echoParticles.isPlaying)
                echoParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
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
