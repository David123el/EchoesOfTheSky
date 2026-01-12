using System;
using UnityEngine;

public class WorldAwakeningManager : MonoBehaviour
{
    [Header("Awakening Targets")]
    [SerializeField] private AwakenableSprite[] awakenables;

    [Header("World Stages")]
    [SerializeField] private Color[] stageTints;
    [SerializeField] private float[] stageIntensity;

    [Header("Color Awakening (Gray → Color)")]
    [Tooltip("Materials using PixelSaturation shader")]
    [SerializeField] private Material[] saturationMaterials;
    [SerializeField] private float minSaturation = 0f; // אפור מוחלט
    [SerializeField] private float maxSaturation = 1f; // צבע מלא

    [Header("Echo Color Control")]
    [SerializeField] private Material[] echoMaterials;
    [SerializeField] private float echoMinSaturation = 0.25f;
    [SerializeField] private float echoMaxSaturation = 0.6f;

    [Header("Color Pulse")]
    [SerializeField] private float pulseIntensityBoost = 0.25f;
    [SerializeField] private float pulseDuration = 0.35f;

    private Coroutine pulseRoutine;

    [SerializeField] private WindowLight[] windowLights;

    [Header("Wind Particles")]
    [SerializeField] private ParticleSystem windParticles;
    [SerializeField] private float[] windEmissionByStage;
    [SerializeField] private float[] windSpeedByStage;
    [SerializeField] private float maxWindSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip stageUnlockSFX;

    public float CurrentWindStrength { get; private set; }

    public static event Action<int> OnStageChanged;

    private int lastAppliedStage = -1;

    private ListeningManager listeningManager;

    private void Awake()
    {
        listeningManager = FindAnyObjectByType<ListeningManager>();
    }

    private void OnEnable()
    {
        if (listeningManager == null) return;

        listeningManager.OnListeningStarted += HandleListeningStarted;
        listeningManager.OnListeningStopped += HandleListeningStopped;
    }

    private void OnDisable()
    {
        if (listeningManager == null) return;

        listeningManager.OnListeningStarted -= HandleListeningStarted;
        listeningManager.OnListeningStopped -= HandleListeningStopped;
    }

    private void HandleListeningStarted()
    {
        // Echo "מתעורר"
        ApplyEchoSaturation(1f);
    }

    private void HandleListeningStopped()
    {
        // Echo חוזר למצב רדום (אבל לא אפור לגמרי)
        ApplyEchoSaturation(0f);
    }

    public void ApplyStage(int stage)
    {
        stage = Mathf.Clamp(stage, 0, GetMaxStage());
        if (stage == lastAppliedStage) return;

        bool isFirstInit = lastAppliedStage == -1;
        lastAppliedStage = stage;

        // 🎨 Awakening visuals (existing system)
        Color tint = GetValue(stageTints, stage, Color.white);
        float intensity = GetValue(stageIntensity, stage, 1f);

        foreach (var awakenable in awakenables)
        {
            awakenable.ApplyAwakening(tint, intensity);
        }

        // Window Lights
        float t = Mathf.InverseLerp(0, GetMaxStage(), stage);

        foreach (var w in windowLights)
        {
            if (w != null)
                w.ApplyAwakening(t);
        }

        // 🌈 Gray → Color via Saturation
        ApplySaturation(stage);

        // 🌬️ Wind
        UpdateWind(stage);

        // 🔊 SFX (לא בהפעלה הראשונית)
        if (!isFirstInit)
        {
            AudioManager.Instance?.PlaySFX(stageUnlockSFX, 0.6f);
        }
        
        OnStageChanged?.Invoke(stage);
    }

    // =======================
    // 🌈 SATURATION CONTROL
    // =======================
    private void ApplySaturation(int stage)
    {
        if (saturationMaterials == null || saturationMaterials.Length == 0)
            return;

        float t = Mathf.InverseLerp(0, GetMaxStage(), stage);
        float saturation = Mathf.Lerp(minSaturation, maxSaturation, t);

        foreach (var mat in saturationMaterials)
        {
            if (mat != null)
                mat.SetFloat("_Saturation", saturation);
        }
    }

    public void ApplyEchoSaturation(float normalizedValue)
    {
        if (echoMaterials == null || echoMaterials.Length == 0)
            return;

        float saturation = Mathf.Lerp(
            echoMinSaturation,
            echoMaxSaturation,
            normalizedValue
        );

        foreach (var mat in echoMaterials)
        {
            if (mat != null)
                mat.SetFloat("_Saturation", saturation);
        }
    }

    public void PlayColorPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(ColorPulseRoutine());
    }

    // =======================
    // 🌬️ WIND
    // =======================
    private void UpdateWind(int stage)
    {
        float emissionRate = GetValue(windEmissionByStage, stage, 0f);
        float speed = GetValue(windSpeedByStage, stage, 0f);

        CurrentWindStrength = Mathf.InverseLerp(0f, maxWindSpeed, speed);

        if (windParticles == null) return;

        var emission = windParticles.emission;
        var main = windParticles.main;

        emission.rateOverTime = emissionRate;
        main.startSpeed = speed;

        if (emissionRate > 0 && !windParticles.isPlaying)
            windParticles.Play();
        else if (emissionRate == 0 && windParticles.isPlaying)
            windParticles.Stop();
    }

    // =======================
    // 🧠 HELPERS
    // =======================
    private int GetMaxStage()
    {
        return Mathf.Min(
            stageTints.Length,
            stageIntensity.Length,
            windEmissionByStage.Length,
            windSpeedByStage.Length
        ) - 1;
    }

    private T GetValue<T>(T[] array, int index, T fallback)
    {
        if (array == null || array.Length == 0)
            return fallback;

        if (index < 0 || index >= array.Length)
            return array[array.Length - 1];

        return array[index];
    }

    private System.Collections.IEnumerator ColorPulseRoutine()
    {
        if (saturationMaterials == null || saturationMaterials.Length == 0)
            yield break;

        float baseStageT = Mathf.InverseLerp(0, GetMaxStage(), lastAppliedStage);
        float baseSaturation = Mathf.Lerp(minSaturation, maxSaturation, baseStageT);

        float pulseSaturation = Mathf.Clamp01(baseSaturation + pulseIntensityBoost);

        float t = 0f;

        // 🔼 עליה מהירה
        while (t < pulseDuration * 0.4f)
        {
            t += Time.deltaTime;
            float lerp = t / (pulseDuration * 0.4f);
            SetSaturation(Mathf.Lerp(baseSaturation, pulseSaturation, lerp));
            yield return null;
        }

        t = 0f;

        // 🔽 חזרה איטית
        while (t < pulseDuration * 0.6f)
        {
            t += Time.deltaTime;
            float lerp = t / (pulseDuration * 0.6f);
            SetSaturation(Mathf.Lerp(pulseSaturation, baseSaturation, lerp));
            yield return null;
        }

        SetSaturation(baseSaturation);
    }

    private void SetSaturation(float value)
    {
        foreach (var mat in saturationMaterials)
        {
            if (mat != null)
                mat.SetFloat("_Saturation", value);
        }
    }
}
