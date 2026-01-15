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
    [SerializeField] private float fadeDuration = 0.12f;

    private Coroutine disappearRoutine;
    private Coroutine fadeRoutine;

    private bool isActive;
    private int playersOnPlatform;

    private Material materialInstance;

    void Awake()
    {
        // 👇 חומר ייחודי לפלטפורמה
        materialInstance = Instantiate(platformRenderer.sharedMaterial);
        platformRenderer.material = materialInstance;

        SetVisible(false, instant: true);
    }

    // ========================
    // 🌊 Public API
    // ========================

    public void ActivateFromZone()
    {
        if (isActive)
            return;

        StopAllRoutines();
        SetVisible(true);
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

    // ========================
    // ⏳ Coroutines
    // ========================

    IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);
        SetVisible(false);
        disappearRoutine = null;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(from, to, t / fadeDuration);
            materialInstance.SetFloat("_Fade", v);
            yield return null;
        }

        materialInstance.SetFloat("_Fade", to);
    }

    // ========================
    // 🎭 Visual State
    // ========================

    void SetVisible(bool visible, bool instant = false)
    {
        isActive = visible;

        platformCollider.enabled = visible;
        platformRenderer.enabled = true; // תמיד ON

        if (echoParticles != null)
        {
            if (visible)
            {
                echoParticles.Play();

                // 🌈 Pulse צבע קצר
                FindAnyObjectByType<WorldAwakeningManager>()?.PlayColorPulse();
            }
            else
                echoParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        StopFade();

        if (instant)
        {
            materialInstance.SetFloat("_Fade", visible ? 1f : 0f);
        }
        else
        {
            fadeRoutine = StartCoroutine(
                Fade(visible ? 0f : 1f, visible ? 1f : 0f)
            );
        }
    }

    void StopAllRoutines()
    {
        if (disappearRoutine != null)
            StopCoroutine(disappearRoutine);

        StopFade();

        disappearRoutine = null;
    }

    void StopFade()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = null;
    }

    // ========================
    // 🧍 Player Tracking
    // ========================

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
