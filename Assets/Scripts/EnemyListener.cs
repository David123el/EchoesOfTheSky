using UnityEngine;

public class EnemyListener : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float listenRadius = 4f;
    [SerializeField] private float wakeUpTime = 1.0f;

    private Transform player;
    private float listenTimer = 0f;
    private bool awakened = false;

    private SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningStarted += OnListeningStarted;
        ListeningManager.Instance.OnListeningStopped += OnListeningStopped;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance == null) return;

        ListeningManager.Instance.OnListeningStarted -= OnListeningStarted;
        ListeningManager.Instance.OnListeningStopped -= OnListeningStopped;
    }

    void OnListeningStarted()
    {
        listenTimer = 0f;
    }

    void OnListeningStopped()
    {
        listenTimer = 0f;
    }

    void Update()
    {
        if (awakened) return;
        if (!ListeningManager.Instance.IsListening) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > listenRadius) return;

        listenTimer += Time.deltaTime;

        // Visual hint before awakening. Enemy turns red.
        // Needs color reset when releasing E key.
        //float t = listenTimer / wakeUpTime;
        //sr.color = Color.Lerp(sr.color, Color.red, t);

        if (listenTimer >= wakeUpTime)
        {
            Awaken();
        }
    }

    void Awaken()
    {
        awakened = true;
        Debug.Log($"{name} awakened by listening!");
        // כאן תפעיל AI / אנימציה / תנועה
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, listenRadius);
    }
}
