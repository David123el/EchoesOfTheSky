using UnityEngine;

public class EnemyListener : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float listenRadius = 4f;
    [SerializeField] private float wakeUpTime = 1.0f;

    private Transform player;
    private float listenTimer;
    private bool awakened;

    private SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (ListeningManager.Instance != null)
            ListeningManager.Instance.OnListeningChanged += HandleListeningChanged;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance != null)
            ListeningManager.Instance.OnListeningChanged -= HandleListeningChanged;
    }

    void HandleListeningChanged(bool isListening)
    {
        listenTimer = 0f;

        // כאן אפשר בעתיד להחזיר צבע / אפקט
        // if (!isListening) sr.color = Color.white;
    }

    void Update()
    {
        if (awakened) return;
        if (!ListeningManager.Instance.IsListening) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > listenRadius) return;

        listenTimer += Time.deltaTime;

        if (listenTimer >= wakeUpTime)
            Awaken();
    }

    void Awaken()
    {
        awakened = true;
        Debug.Log($"{name} awakened by listening!");
        // TODO: הפעלת AI / אנימציה
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, listenRadius);
    }
}
