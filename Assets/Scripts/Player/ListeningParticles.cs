using UnityEngine;

public class ListeningParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    void Awake()
    {
        if (particles == null)
            particles = GetComponent<ParticleSystem>();

        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

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
        if (isListening)
            Play();
        else Stop();
    }

    void Play()
    {
        if (!particles.isPlaying)
            particles.Play();
    }

    void Stop()
    {
        particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
