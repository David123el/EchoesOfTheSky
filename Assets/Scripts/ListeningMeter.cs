using UnityEngine;

public class ListeningMeter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxMeter = 3f;
    [SerializeField] private float drainRate = 1f;
    [SerializeField] private float recoveryRate = 1.5f;
    [SerializeField] private float overloadCooldown = 1.0f;

    public float CurrentMeter { get; private set; }
    public bool IsOverloaded { get; private set; }

    private bool listening;
    private float cooldownTimer;

    public float MaxMeter => maxMeter;
    public bool IsListening => listening;

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningStarted += StartListening;
        ListeningManager.Instance.OnListeningStopped += StopListening;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance == null) return;

        ListeningManager.Instance.OnListeningStarted -= StartListening;
        ListeningManager.Instance.OnListeningStopped -= StopListening;
    }

    void StartListening()
    {
        if (IsOverloaded) return;
        listening = true;
    }

    void StopListening()
    {
        listening = false;
    }

    void Update()
    {
        if (IsOverloaded)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                IsOverloaded = false;
                CurrentMeter = 0f;
            }
            return;
        }

        if (listening)
        {
            CurrentMeter += drainRate * Time.deltaTime;

            if (CurrentMeter >= maxMeter)
            {
                TriggerOverload();
            }
        }
        else
        {
            CurrentMeter -= recoveryRate * Time.deltaTime;
        }

        CurrentMeter = Mathf.Clamp(CurrentMeter, 0f, maxMeter);
    }

    void TriggerOverload()
    {
        IsOverloaded = true;
        listening = false;
        cooldownTimer = overloadCooldown;

        ListeningManager.Instance.ForceStopListening();
        Debug.Log("Listening Overload!");
    }
}
