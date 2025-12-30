using System;
using UnityEngine;

public class SoundShardManager : MonoBehaviour
{
    public static SoundShardManager Instance;
    public static event Action<int> OnShardCountChanged;

    [Header("Progress")]
    private int collectedShards = 0;

    private int baseStage = 0;        // התקדמות קבועה
    private int resonanceCount = 0;   // זמני
    private int currentStage = 0;     // נגזר בלבד


    [Header("VFX")]
    [SerializeField] private ParticleSystem shardCollectVFX;
    [SerializeField] private Transform vfxContainer;

    [Header("References")]
    [SerializeField] private WorldAwakeningManager worldAwakening;

    [Header("Shard Data")]
    public int totalShardsInLevel;

    [Header("Audio")]
    [SerializeField] private AudioClip shardCollectSFX;

    public static event Action<int> OnBaseStageChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (worldAwakening == null)
        {
            Debug.LogError("SoundShardManager: WorldAwakeningManager missing");
            return;
        }

        RecalculateStage(false);
    }

    void RecalculateStage(bool playPulse = true)
    {
        currentStage = baseStage + resonanceCount;
        worldAwakening.ApplyStage(currentStage);

        if (playPulse)
            worldAwakening.PlayColorPulse();
    }

    public void EnterResonance()
    {
        resonanceCount++;
        RecalculateStage();
    }

    public void ExitResonance()
    {
        resonanceCount = Mathf.Max(0, resonanceCount - 1);
        RecalculateStage(false);
    }

    public void CollectShard(Vector3 worldPosition)
    {
        SpawnCollectVFX(worldPosition);

        collectedShards++;
        OnShardCountChanged?.Invoke(collectedShards);

        CameraController.Instance?.PulseZoom(3, 0.05f);
        AudioManager.Instance?.PlaySFX(shardCollectSFX, 0.7f);

        if (collectedShards > baseStage)
        {
            baseStage = collectedShards;
            OnBaseStageChanged?.Invoke(baseStage); // 🎵 מוזיקה מאזינה רק לזה
            RecalculateStage();
        }
    }

    private void SpawnCollectVFX(Vector3 pos)
    {
        if (shardCollectVFX == null) return;

        var vfx = Instantiate(
            shardCollectVFX,
            pos,
            Quaternion.identity,
            vfxContainer
        );

        Destroy(vfx.gameObject,
            vfx.main.duration + vfx.main.startLifetime.constantMax);
    }
}
