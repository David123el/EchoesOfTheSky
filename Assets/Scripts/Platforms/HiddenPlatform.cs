using UnityEngine;

public class HiddenPlatform : MonoBehaviour
{
    private enum PlatformState
    {
        Hidden,
        Revealed,
        Locked
    }

    [Header("Settings")]
    [SerializeField] private float lockDelay = 0.2f;

    private PlatformState state = PlatformState.Hidden;
    private Collider2D col;
    private SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        SetHidden();
    }

    void SetHidden()
    {
        col.enabled = false;
        sr.enabled = false;
        state = PlatformState.Hidden;
    }

    void SetRevealed()
    {
        sr.enabled = true;
        col.enabled = false;
        state = PlatformState.Revealed;
    }

    void SetLocked()
    {
        sr.enabled = true;
        col.enabled = true;
        state = PlatformState.Locked;
    }

    // נקרא ע"י ListeningZone
    public void Reveal()
    {
        if (state != PlatformState.Hidden) return;

        SetRevealed();
        Invoke(nameof(SetLocked), lockDelay);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != PlatformState.Revealed) return;
        if (!collision.collider.CompareTag("Player")) return;

        SetLocked();
    }
}
