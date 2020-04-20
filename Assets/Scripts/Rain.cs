using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    public ParticleSystem RainDrops;
    public ParticleSystem RainShadow;
    public float RainMaxVolume = 0.75f;
    public float RainTime = 8.0f;

    private Plant _plant;
    private ParticleSystem.EmissionModule dropsEmission;
    private ParticleSystem.EmissionModule shadowEmission;

    private AudioSource _rainAudio;
    private float _rainVolume;
    private float _rainTicker;

    bool CloseToOrigin => Vector3.SqrMagnitude(transform.position) < 1.0f;
    public bool IsRaining => _rainTicker > 0;

    void Awake()
    {
        _plant = FindObjectOfType<Plant>();
        _rainAudio = GetComponent<AudioSource>();

        dropsEmission = RainDrops.emission;
        shadowEmission = RainShadow.emission;

        _rainVolume = 0;
    }

    public void StartRain()
    {
        dropsEmission.enabled = true;
        shadowEmission.enabled = true;

        _rainTicker = RainTime;
    }

    void StopRain()
    {
        dropsEmission.enabled = false;
        shadowEmission.enabled = false;

        _plant.SetGrowingLoopEnabled(false);
    }

    void Update()
    {
        _rainTicker -= Time.deltaTime;

        if (IsRaining)
        {
            _plant.SetGrowingLoopEnabled(CloseToOrigin);

            _rainVolume += Time.deltaTime;
            _rainVolume = Mathf.Clamp(_rainVolume, 0, RainMaxVolume);
            _rainAudio.volume = _rainVolume;
        }
        else
        {
            StopRain();

            _rainVolume -= Time.deltaTime;
            _rainVolume = Mathf.Clamp(_rainVolume, 0, RainMaxVolume);
            _rainAudio.volume = _rainVolume;
        }
    }
}