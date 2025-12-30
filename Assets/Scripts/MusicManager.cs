using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Layers (Low → High)")]
    [SerializeField] private AudioSource[] musicLayers;

    [Header("Volume Settings")]
    [SerializeField] private float baseLayerVolume = 0.06f;
    [SerializeField] private float windDuckAmount = 0.4f;
    [SerializeField] private float volumeSmoothSpeed = 2f;

    private int activeLayers = 0;
    private WorldAwakeningManager world;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        world = FindFirstObjectByType<WorldAwakeningManager>();

        // כל השכבות מוכנות אך שקטות
        for (int i = 0; i < musicLayers.Length; i++)
        {
            musicLayers[i].loop = true;
            musicLayers[i].volume = 0f;
            musicLayers[i].Play();
        }

        // 🎵 מאזינים רק ל־Base Stage (Shards)
        SoundShardManager.OnBaseStageChanged += HandleBaseStageChanged;
    }

    private void OnDestroy()
    {
        SoundShardManager.OnBaseStageChanged -= HandleBaseStageChanged;
    }

    private void Update()
    {
        if (world == null) return;

        float wind = world.CurrentWindStrength;
        float duckMultiplier = Mathf.Lerp(1f, 1f - windDuckAmount, wind);

        // רק שכבות שכבר נפתחו
        for (int i = 0; i < activeLayers; i++)
        {
            AudioSource layer = musicLayers[i];
            float targetVolume = baseLayerVolume * duckMultiplier;

            layer.volume = Mathf.Lerp(
                layer.volume,
                targetVolume,
                Time.deltaTime * volumeSmoothSpeed
            );
        }
    }

    // =========================
    // 🎵 BASE STAGE HANDLING
    // =========================
    private void HandleBaseStageChanged(int baseStage)
    {
        int targetLayers = Mathf.Clamp(baseStage, 0, musicLayers.Length);

        // פותחים רק מה שחסר
        for (int i = activeLayers; i < targetLayers; i++)
        {
            ActivateLayer(i);
        }

        activeLayers = targetLayers;
    }

    private void ActivateLayer(int index)
    {
        AudioSource layer = musicLayers[index];
        StartCoroutine(FadeIn(layer, 2f));
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, baseLayerVolume, t / duration);
            yield return null;
        }

        source.volume = baseLayerVolume;
    }
}
