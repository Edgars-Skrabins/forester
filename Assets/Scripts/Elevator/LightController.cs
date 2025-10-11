using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class LightController : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.0f;

    [Header("Flicker Timing")]
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    [Header("Buzz Effect")]
    public AudioSource buzzAudio;
    [Range(0f, 1f)] public float buzzVolume = 0.6f;
    public float buzzPitchVariance = 0.05f;

    [Header("Smoothness")]
    public bool useDoTween = true;
    public float tweenDuration = 0.05f;
    public Ease tweenEase = Ease.InOutSine;

    private float nextFlickerTime;
    private float targetIntensity;

    void Start()
    {
        if (!targetLight)
            targetLight = GetComponent<Light>();

        if (buzzAudio)
        {
            buzzAudio.loop = true;
            buzzAudio.volume = buzzVolume;
            buzzAudio.Play();
        }

        ScheduleNextFlicker();
    }

    void Update()
    {
        if (Time.time >= nextFlickerTime)
        {
            Flicker();
            ScheduleNextFlicker();
        }
    }

    void Flicker()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);

        if (useDoTween)
        {
            targetLight.DOIntensity(targetIntensity, tweenDuration)
                .SetEase(tweenEase);
        }
        else
        {
            targetLight.intensity = targetIntensity;
        }

        if (buzzAudio)
        {
            float pitch = 1f + Random.Range(-buzzPitchVariance, buzzPitchVariance);
            buzzAudio.pitch = pitch;
        }
    }

    void ScheduleNextFlicker()
    {
        nextFlickerTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
    }
}
