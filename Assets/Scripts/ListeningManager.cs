using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class ListeningManager : MonoBehaviour
{
    public static ListeningManager Instance;

    public event Action OnListeningStarted;
    public event Action OnListeningStopped;

    public bool IsListening { get; private set; }

    private PlayerInput playerInput;
    private InputAction listenAction;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInput = GetComponent<PlayerInput>();
        listenAction = playerInput.actions["Listen"];
    }

    void OnEnable()
    {
        listenAction.started += OnListenStarted;
        listenAction.canceled += OnListenCanceled;
    }

    void OnDisable()
    {
        listenAction.started -= OnListenStarted;
        listenAction.canceled -= OnListenCanceled;
    }

    private void OnListenStarted(InputAction.CallbackContext context)
    {
        if (IsListening) return;

        IsListening = true;
        OnListeningStarted?.Invoke();
    }

    private void OnListenCanceled(InputAction.CallbackContext context)
    {
        if (!IsListening) return;

        IsListening = false;
        OnListeningStopped?.Invoke();
    }

    public void ForceStopListening()
    {
        if (!IsListening) return;

        IsListening = false;
        OnListeningStopped?.Invoke();
    }
}
