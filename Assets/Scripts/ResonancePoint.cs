using UnityEngine;

public class ResonancePoint : MonoBehaviour
{
    public float requiredStillTime = 1.2f;

    [SerializeField] private SoundShardManager shardManager;
    public ParticleSystem resonanceVFX;

    private float timer;
    private bool activated;

    private PlayerMovement player;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        if (shardManager == null)
            shardManager = FindFirstObjectByType<SoundShardManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<PlayerMovement>();
        playerRb = other.GetComponent<Rigidbody2D>();
        timer = 0f;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activated || player == null) return;

        bool isStill =
            Mathf.Abs(playerRb.linearVelocity.x) < 0.05f &&
            player.IsGrounded;

        if (isStill)
        {
            timer += Time.deltaTime;
            if (timer >= requiredStillTime)
                Activate();
        }
        else
        {
            timer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //shardManager.RegressStage();
        //shardManager.ExitResonance();

        /*activated = false;

        if (resonanceVFX != null)
            resonanceVFX.Stop();*/

        timer = 0f;

        Deactivate();
    }

    private void Activate()
    {
        activated = true;

        if (resonanceVFX != null)
            resonanceVFX.Play();

        //shardManager?.AdvanceStage();
        shardManager.EnterResonance();
    }

    private void Deactivate()
    {
        activated = false;

        if (resonanceVFX != null)
            resonanceVFX.Stop();

        shardManager.ExitResonance();
    }
}
