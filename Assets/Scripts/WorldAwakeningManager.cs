using System;
using UnityEngine;

public class WorldAwakeningManager : MonoBehaviour
{
    // =======================
    // 🧭 MODE
    // =======================
    public enum BuildMode
    {
        Demo,
        FullGame
    }

    [Header("Build Mode")]
    [SerializeField] private BuildMode buildMode = BuildMode.Demo;

    // =======================
    // 🎨 DEMO VISUAL BASELINE
    // =======================
    [Header("Demo Settings")]
    [Tooltip("Base saturation used in demo (no progression)")]
    [SerializeField] private float demoSaturation = 0.75f;

    // =======================
    // 🌍 AWAKENING TARGETS
    // =======================
    [Header("Awakening Targets")]
    [SerializeField] private AwakenableSprite[] awakenables;

    [Header("World Stages")]
    [SerializeField] private Color[] stageTints;
    [SerializeField] private float[] stageIntensity;

    // =======================
    // 🌈 SATURATION (GRAY → COLOR)
    // =======================
    [Header("Saturation Materials")]
    [Tooltip("Materials using PixelSaturation shader")]
    [SerializeField] private Material[] saturationMaterials;
    [SerializeField] private float minSaturation = 0f;
    [SerializeField] private float maxSaturation = 1f;

    // =======================
    // 🌊 ECHO SATURATION
    // =======================
    [Header("Echo Color Control")]
    [SerializeField] private Material[] echoMaterials;
    [SerializeField] private float echoMinSaturation = 0.25f;
    [SerializeField] private float echoMaxSaturation = 0.6f;

    // =======================
    // ✨ COLOR PULSE (FEEDBACK)
    // =======================
    [Header("Color Pulse")]
    [SerializeField] private float pulseIntensityBoost = 0.25f;
    [SerializeField] private float pulseDuration = 0.35f;

    private Coroutine pulseRoutine;

    // =======================
    // 🌬️ WIND (FULL GAME ONLY)
    // =======================
    [Header("Wind")]
    [SerializeField] private ParticleSystem windParticles;
    [SerializeField] private float[] windEmissionByStage;
    [SerializeField] private float[] windSpeedByStage;
    [SerializeField] private float maxWindSpeed = 10f;

    public float CurrentWindStrength { get; private set; }

    // =======================
    // 🔊 AUDIO
    // =======================
    [Header("Audio")]
    [SerializeField] private AudioClip stageUnlockSFX;

    public static event Action<int> OnStageChanged;

    private int lastAppliedStage = -1;

    // =======================
    // 🔊 LISTENING
    // =======================
    private void OnEnable()
    {
        if (ListeningManager.Instance != null)
            ListeningManager.Instance.OnListeningChanged += HandleListeningChanged;
    }

    private void OnDisable()
    {
        if (ListeningManager.Instance != null)
            ListeningManager.Instance.OnListeningChanged -= HandleListeningChanged;
    }

    private void HandleListeningChanged(bool isListening)
    {
        ApplyEchoSaturation(isListening ? 1f : 0f);
    }

    // =======================
    // 🌍 STAGE APPLY (FULL GAME)
    // =======================
    public void ApplyStage(int stage)
    {
        if (buildMode == BuildMode.Demo)
            return; // ❌ אין progression בדמו

        stage = Mathf.Clamp(stage, 0, GetMaxStage());
        if (stage == lastAppliedStage)
            return;

        bool isFirstInit = lastAppliedStage == -1;
        lastAppliedStage = stage;

        // 🎨 Awakening visuals
        Color tint = GetValue(stageTints, stage, Color.white);
        float intensity = GetValue(stageIntensity, stage, 1f);

        foreach (var awakenable in awakenables)
        {
            awakenable.ApplyAwakening(tint, intensity);
        }

        // 🌈 Saturation progression
        ApplyStageSaturation(stage);

        // 🌬️ Wind
        UpdateWind(stage);

        // 🔊 SFX
        if (!isFirstInit)
            AudioManager.Instance?.PlaySFX(stageUnlockSFX, 0.6f);

        OnStageChanged?.Invoke(stage);
    }

    // =======================
    // 🌈 SATURATION
    // =======================
    private void ApplyStageSaturation(int stage)
    {
        float t = Mathf.InverseLerp(0, GetMaxStage(), stage);
        float saturation = Mathf.Lerp(minSaturation, maxSaturation, t);
        SetSaturation(saturation);
    }

    private float GetBaseSaturation()
    {
        if (buildMode == BuildMode.Demo)
            return demoSaturation;

        float t = Mathf.InverseLerp(0, GetMaxStage(), lastAppliedStage);
        return Mathf.Lerp(minSaturation, maxSaturation, t);
    }

    private void SetSaturation(float value)
    {
        foreach (var mat in saturationMaterials)
        {
            if (mat != null)
                mat.SetFloat("_Saturation", value);
        }
    }

    // =======================
    // 🌊 ECHO SATURATION
    // =======================
    public void ApplyEchoSaturation(float normalizedValue)
    {
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

    // =======================
    // ✨ COLOR PULSE (ALWAYS ON)
    // =======================
    public void PlayColorPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(ColorPulseRoutine());
    }

    private System.Collections.IEnumerator ColorPulseRoutine()
    {
        float baseSaturation = GetBaseSaturation();
        float pulseSaturation = Mathf.Clamp01(baseSaturation + pulseIntensityBoost);

        float t = 0f;

        // 🔼 Fast rise
        while (t < pulseDuration * 0.4f)
        {
            t += Time.deltaTime;
            SetSaturation(Mathf.Lerp(baseSaturation, pulseSaturation, t / (pulseDuration * 0.4f)));
            yield return null;
        }

        t = 0f;

        // 🔽 Slow return
        while (t < pulseDuration * 0.6f)
        {
            t += Time.deltaTime;
            SetSaturation(Mathf.Lerp(pulseSaturation, baseSaturation, t / (pulseDuration * 0.6f)));
            yield return null;
        }

        SetSaturation(baseSaturation);
    }

    // =======================
    // 🌬️ WIND
    // =======================
    private void UpdateWind(int stage)
    {
        if (windParticles == null)
            return;

        float emissionRate = GetValue(windEmissionByStage, stage, 0f);
        float speed = GetValue(windSpeedByStage, stage, 0f);

        CurrentWindStrength = Mathf.InverseLerp(0f, maxWindSpeed, speed);

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
}
