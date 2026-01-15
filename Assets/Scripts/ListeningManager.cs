using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ListeningManager : MonoBehaviour
{
    public static ListeningManager Instance;

    public event Action<bool> OnListeningChanged;

    public bool IsListening { get; private set; }

    [Header("Listening Timing")]
    [SerializeField] private float holdTimeToListen = 0.25f;
    [SerializeField] private float exitGraceTime = 0.1f;

    private bool canListen;          // מגיע מהשחקן
    private float holdTimer;
    private float exitTimer;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        UpdateListeningState();
    }

    // =========================
    // 🧠 State Logic
    // =========================

    void UpdateListeningState()
    {
        if (canListen)
        {
            exitTimer = 0f;

            if (!IsListening)
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= holdTimeToListen)
                    EnterListening();
            }
        }
        else
        {
            holdTimer = 0f;

            if (IsListening)
            {
                exitTimer += Time.deltaTime;

                if (exitTimer >= exitGraceTime)
                    ExitListening();
            }
        }
    }

    void EnterListening()
    {
        IsListening = true;
        OnListeningChanged?.Invoke(true);
    }

    void ExitListening()
    {
        IsListening = false;
        OnListeningChanged?.Invoke(false);
    }

    // =========================
    // 🔌 Public API
    // =========================

    public void SetCanListen(bool value)
    {
        canListen = value;
    }
}
