using UnityEngine;
using System.Collections;

public class AmbientEventPlayer : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;

    [Header("Timing")]
    [SerializeField] private Vector2 delayRange = new Vector2(8f, 20f);

    [Header("Volume")]
    [SerializeField] private Vector2 volumeRange = new Vector2(0.05f, 0.12f);

    [Header("Stage Control")]
    [SerializeField] private int minStage = 0;

    private WorldAwakeningManager world;

    private void Awake()
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        world = FindFirstObjectByType<WorldAwakeningManager>();
    }

    private void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            float delay = Random.Range(delayRange.x, delayRange.y);
            yield return new WaitForSeconds(delay);

            if (clips.Length == 0 || world == null)
                continue;

            if (world.CurrentWindStrength < minStage * 0.25f)
                continue;

            source.clip = clips[Random.Range(0, clips.Length)];
            source.volume = Random.Range(volumeRange.x, volumeRange.y);
            source.Play();
        }
    }
}
