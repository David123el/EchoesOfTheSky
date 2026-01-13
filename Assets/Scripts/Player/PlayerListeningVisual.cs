using UnityEngine;

public class PlayerListeningVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningChanged += OnListeningChanged;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance == null) return;

        ListeningManager.Instance.OnListeningChanged -= OnListeningChanged;
    }

    void OnListeningChanged(bool isListening)
    {
        animator.SetTrigger("Listen");
        animator.SetBool("IsListening", isListening);
    }
}
