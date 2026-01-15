using UnityEngine;

public class ListeningIndicator : MonoBehaviour
{
    [SerializeField] private GameObject visuals;

    void OnEnable()
    {
        ListeningManager.Instance.OnListeningChanged += Toggle;
    }

    void OnDisable()
    {
        if (ListeningManager.Instance == null) return;
        ListeningManager.Instance.OnListeningChanged -= Toggle;
    }

    void Toggle(bool isListening) => visuals.SetActive(isListening);
}
