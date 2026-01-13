using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ListeningManager : MonoBehaviour
{
    public static ListeningManager Instance;

    public event Action<bool> OnListeningChanged;

    public bool IsListening { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetListening(bool value)
    {
        if (IsListening == value)
            return;

        IsListening = value;
        OnListeningChanged?.Invoke(IsListening);
    }
}
