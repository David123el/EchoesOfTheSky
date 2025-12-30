using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WindAmbientParticles : MonoBehaviour
{
    [Header("Wind Influence")]
    [SerializeField] private float maxEmission = 30f;
    [SerializeField] private float maxSpeed = 3f;

    [Header("Smoothing")]
    [SerializeField] private float smooth = 3f;

    private ParticleSystem ps;
    private WorldAwakeningManager world;

    private float currentEmission;
    private float currentSpeed;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        world = FindFirstObjectByType<WorldAwakeningManager>();
    }

    private void Update()
    {
        if (world == null) return;

        float wind = world.CurrentWindStrength;

        float targetEmission = wind * maxEmission;
        float targetSpeed = wind * maxSpeed;

        currentEmission = Mathf.Lerp(
            currentEmission,
            targetEmission,
            Time.deltaTime * smooth
        );

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            Time.deltaTime * smooth
        );

        var emission = ps.emission;
        var main = ps.main;

        emission.rateOverTime = currentEmission;
        main.startSpeed = currentSpeed;

        if (currentEmission > 0.1f && !ps.isPlaying)
            ps.Play();
        else if (currentEmission <= 0.1f && ps.isPlaying)
            ps.Stop();
    }
}
