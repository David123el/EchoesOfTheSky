using UnityEngine;
using UnityEngine.Rendering;

public class AmbientAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WorldAwakeningManager world;

    [Header("Wind Audio")]
    [SerializeField] private AudioSource windSource;
    [SerializeField] private float windMinVolume = 0.03f;
    [SerializeField] private float windMaxVolume = 0.4f;
    [SerializeField] private float windSmooth = 1.5f;

    [Header("City Hum Audio")]
    [SerializeField] private AudioSource cityHumSource;
    [SerializeField] private float cityHumVolume = 0.15f;
    [SerializeField] private int cityHumStartStage = 2;
    [SerializeField] private float cityHumFadeSpeed = 1.2f;

    private float cityHumTarget = 0f;

    private void Awake()
    {
        if (world == null)
            world = FindFirstObjectByType<WorldAwakeningManager>();
    }

    private void OnEnable()
    {
        WorldAwakeningManager.OnStageChanged += HandleStageChanged;
    }

    private void OnDisable()
    {
        WorldAwakeningManager.OnStageChanged -= HandleStageChanged;
    }

    private void Update()
    {
        UpdateWind();
        UpdateCityHum();
    }

    // 🌬️ WIND
    private void UpdateWind()
    {
        if (world == null || windSource == null) return;

        float wind = world.CurrentWindStrength;
        float targetVolume = Mathf.Lerp(windMinVolume, windMaxVolume, wind);

        windSource.volume = Mathf.Lerp(
            windSource.volume,
            targetVolume,
            Time.deltaTime * windSmooth
        );

        if (targetVolume > 0.01f && !windSource.isPlaying)
            windSource.Play();
        else if (targetVolume <= 0.01f && windSource.isPlaying)
            windSource.Stop();
    }

    // 🏙️ CITY HUM
    private void HandleStageChanged(int stage)
    {
        if (stage >= cityHumStartStage)
            cityHumTarget = cityHumVolume;
        else
            cityHumTarget = 0f;
    }

    private void UpdateCityHum()
    {
        if (cityHumSource == null) return;

        cityHumSource.volume = Mathf.Lerp(
            cityHumSource.volume,
            cityHumTarget,
            Time.deltaTime * cityHumFadeSpeed
        );

        if (cityHumTarget > 0.01f && !cityHumSource.isPlaying)
            cityHumSource.Play();
        else if (cityHumTarget <= 0.01f && cityHumSource.isPlaying)
            cityHumSource.Stop();
    }
}
