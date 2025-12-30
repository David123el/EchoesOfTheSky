using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement movement;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource loopSource;     // Hum / Ground loop
    [SerializeField] private AudioSource oneShotSource;  // Footstep / Jump / Land

    [Header("Clips")]
    [SerializeField] private AudioClip groundHum;
    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;

    [Header("Volumes")]
    [Range(0f, 1f)] public float humVolume = 0.15f;
    [Range(0f, 1f)] public float footstepVolume = 0.6f;
    [Range(0f, 1f)] public float jumpVolume = 0.7f;
    [Range(0f, 1f)] public float landVolume = 0.8f;

    private bool wasGrounded;

    void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleGroundHum();
        HandleLanding();
    }

    // =========================
    // 🌍 Ground Hum (Loop)
    // =========================
    private void HandleGroundHum()
    {
        if (movement.IsGrounded)
        {
            if (!loopSource.isPlaying)
            {
                loopSource.clip = groundHum;
                loopSource.volume = humVolume;
                loopSource.loop = true;
                loopSource.Play();
            }
        }
        else
        {
            if (loopSource.isPlaying)
                loopSource.Stop();
        }
    }

    // =========================
    // 🦶 Landing
    // =========================
    private void HandleLanding()
    {
        if (!wasGrounded && movement.IsGrounded)
        {
            PlayLand();
        }

        wasGrounded = movement.IsGrounded;
    }

    // =========================
    // 🎵 Public API (Events)
    // =========================
    public void PlayFootstep()
    {
        if (!movement.IsGrounded) return;
        oneShotSource.PlayOneShot(footstep, footstepVolume);
    }

    public void PlayJump()
    {
        oneShotSource.PlayOneShot(jump, jumpVolume);
    }

    private void PlayLand()
    {
        oneShotSource.PlayOneShot(land, landVolume);
    }
}
