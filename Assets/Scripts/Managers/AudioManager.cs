using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public enum SFXType
{
    Music,
    SFX,
    UISFX,
    Ambience
}

[System.Serializable]
public class AudioSFX
{
    public string name;
    public AudioClip audioClip;
    [Range(0, 1)] public float volume = 1f;
    public SFXType SfxType;
    public bool randomizePitch;
    public Vector2 randomizePitchValues = new Vector2(0.9f, 1.1f);
    public bool playOnAwake;
    public bool loop;
    [Min(1)] public int poolSize = 1;
}

public class AudioManager : Singleton<AudioManager>
{
    [Header("Mixer Settings")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup masterMixer, musicMixer, sfxMixer, ambienceMixer, uiMixer;

    [Header("Audio Clip Settings")]
    [SerializeField] private List<AudioSFX> audioSFXList;

    // Pool Dictionary
    private Dictionary<string, List<AudioSource>> audioPools;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioPools();
    }

    private void InitializeAudioPools()
    {
        audioPools = new Dictionary<string, List<AudioSource>>();
        GameObject audioSourcesParent = new GameObject("AudioSources");
        DontDestroyOnLoad(audioSourcesParent); // Added by kupole - to keep audio sources between scenes
        audioSourcesParent.transform.SetParent(transform);

        foreach (AudioSFX sfx in audioSFXList)
        {
            List<AudioSource> pool = new List<AudioSource>();

            GameObject sfxParent = new GameObject(sfx.name + "_Pool");
            DontDestroyOnLoad(sfxParent); // Added by kupole - to keep audio sources between scenes
            sfxParent.transform.SetParent(audioSourcesParent.transform);

            for (int i = 0; i < sfx.poolSize; i++)
            {
                GameObject sfxObject = new GameObject(sfx.name + "_Source_" + i);
                sfxObject.transform.SetParent(sfxParent.transform);

                AudioSource newAudioSource = sfxObject.AddComponent<AudioSource>();
                newAudioSource.clip = sfx.audioClip;
                newAudioSource.volume = sfx.volume;
                newAudioSource.loop = sfx.loop;
                newAudioSource.playOnAwake = sfx.playOnAwake;
                newAudioSource.spatialBlend = 0f;

                switch (sfx.SfxType)
                {
                    case SFXType.Music: newAudioSource.outputAudioMixerGroup = musicMixer; break;
                    case SFXType.UISFX: newAudioSource.outputAudioMixerGroup = uiMixer; break;
                    case SFXType.Ambience: newAudioSource.outputAudioMixerGroup = ambienceMixer; break;
                    default: newAudioSource.outputAudioMixerGroup = sfxMixer; break;
                }

                if (sfx.SfxType == SFXType.UISFX)
                {
                    newAudioSource.bypassReverbZones = true;
                }
                pool.Add(newAudioSource);
            }

            audioPools.Add(sfx.name, pool);

            if (sfx.playOnAwake)
                PlaySound(sfx.name);
        }
    }

    private AudioSFX GetAudioSFXByName(string name)
    {
        return audioSFXList.Find(sfx => sfx.name == name);
    }

    private AudioSource GetAvailableSource(string name)
    {
        if (!audioPools.ContainsKey(name))
            return null;

        List<AudioSource> pool = audioPools[name];
        AudioSFX sfx = GetAudioSFXByName(name);

        foreach (AudioSource src in pool)
        {
            if (!src.isPlaying)
                return src;
        }

        Debug.Log(pool.Count);
        Debug.Log(pool[0]);

        GameObject sfxParent = pool[0].transform.parent.gameObject;

        GameObject sfxObject = new GameObject(name + "_Source_Extra_" + pool.Count);
        DontDestroyOnLoad(sfxObject); // Added by kupole - to keep audio sources between scenes
        if (sfxParent != null)
        {
            sfxObject.transform.SetParent(sfxParent.transform);
        }

        AudioSource newAudioSource = sfxObject.AddComponent<AudioSource>();
        newAudioSource.clip = sfx.audioClip;
        newAudioSource.volume = sfx.volume;
        newAudioSource.loop = sfx.loop;
        newAudioSource.playOnAwake = false;

        switch (sfx.SfxType)
        {
            case SFXType.Music: newAudioSource.outputAudioMixerGroup = musicMixer; break;
            case SFXType.UISFX: newAudioSource.outputAudioMixerGroup = uiMixer; break;
            case SFXType.Ambience: newAudioSource.outputAudioMixerGroup = ambienceMixer; break;
            default: newAudioSource.outputAudioMixerGroup = sfxMixer; break;
        }
        if (sfx.SfxType == SFXType.UISFX)
        {
            newAudioSource.bypassReverbZones = true;
        }
        // Add to pool
        pool.Add(newAudioSource);

        Debug.LogWarning($"[AudioManager] Pool for '{name}' expanded to {pool.Count} sources.");

        return newAudioSource;
    }

    public void PlaySound(string name)
    {
        AudioSFX sfx = GetAudioSFXByName(name);
        if (sfx == null) return;

        AudioSource source = GetAvailableSource(name);
        if (source == null) return;

        if (sfx.randomizePitch)
            source.pitch = Random.Range(sfx.randomizePitchValues.x, sfx.randomizePitchValues.y);
        else
            source.pitch = 1f;

        source.spatialBlend = 0f;
        source.transform.SetParent(transform);
        source.transform.localPosition = Vector3.zero;
        source.Play();
    }

    public void PlaySound(string name, Vector3 position)
    {
        AudioSFX sfx = GetAudioSFXByName(name);
        if (sfx == null) return;

        AudioSource source = GetAvailableSource(name);
        if (source == null) return;

        if (sfx.randomizePitch)
            source.pitch = Random.Range(sfx.randomizePitchValues.x, sfx.randomizePitchValues.y);
        else
            source.pitch = 1f;

        source.transform.SetParent(null);
        source.transform.position = position;
        source.spatialBlend = 1f;
        source.Play();
    }

    public void PlaySound(string name, Transform parent)
    {
        AudioSFX sfx = GetAudioSFXByName(name);
        if (sfx == null) return;

        AudioSource source = GetAvailableSource(name);
        if (source == null) return;

        if (sfx.randomizePitch)
            source.pitch = Random.Range(sfx.randomizePitchValues.x, sfx.randomizePitchValues.y);
        else
            source.pitch = 1f;

        source.transform.SetParent(parent);
        source.transform.localPosition = Vector3.zero;
        source.spatialBlend = 1f;
        source.Play();
    }

    public void StopSound(string name)
    {
        if (!audioPools.ContainsKey(name)) return;
        foreach (AudioSource src in audioPools[name])
        {
            if (src.isPlaying)
                src.Stop();
        }
    }
}
