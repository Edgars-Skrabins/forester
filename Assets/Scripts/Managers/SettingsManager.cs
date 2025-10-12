using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float musicVolume = 0.7f;
    [SerializeField] private float sfxVolume = 0.7f;
    [SerializeField] private float ambienceVolume = 0.7f;
    [SerializeField] private float uiVolume = 0.7f;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private bool fullscreen = true;
    
    [Header("Audio")]
    public AudioMixer audioMixer;
    
    [Header("UI Elements")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider ambienceVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider uiVolumeSlider;
    public Slider mouseSensitivitySlider;
    public Toggle fullscreenToggle;
    
    public void Awake()
    {
        PlayerPrefs.DeleteAll();
        InitializeUI();
        LoadSettings();
        ApplySettings();
    }
    
    private void InitializeUI()
    {
        // Setup UI from saved values
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        ambienceVolumeSlider.value = ambienceVolume;
        sfxVolumeSlider.value = sfxVolume;
        uiVolumeSlider.value = uiVolume;
        mouseSensitivitySlider.value = mouseSensitivity;
        fullscreenToggle.isOn = fullscreen;

        // Hook listeners
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        ambienceVolumeSlider.onValueChanged.AddListener(SetAmbienceVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("UIVolume", uiVolume);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log("Settings Saved");
    }

    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        musicVolume = PlayerPrefs.GetFloat("AmbienceVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        uiVolume = PlayerPrefs.GetFloat("UIVolume", 0.7f);
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }

    private void ApplySettings()
    {
        SetMixerVolume("MasterVolume", masterVolume);
        SetMixerVolume("MusicVolume", musicVolume);
        SetMixerVolume("SFXVolume", sfxVolume);
        SetMixerVolume("AmbienceVolume", ambienceVolume);
        SetMixerVolume("UIVolume", uiVolume);

        Screen.fullScreen = fullscreen;
    }

    private void SetMixerVolume(string parameter, float value)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning($"AudioMixer not assigned! Could not set {parameter}.");
            return;
        }
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.000001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }

    private void SetMasterVolume(float value)
    {
        masterVolume = value;
        ApplySettings();
        SaveSettings();
    }

    private void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplySettings();
        SaveSettings();
    }

    private void SetSFXVolume(float value)
    {
        sfxVolume = value;
        ApplySettings();
        SaveSettings();
    }

    private void SetAmbienceVolume(float value)
    {
        ambienceVolume = value;
        ApplySettings();
        SaveSettings();
    }

    private void SetUIVolume(float value)
    {
        uiVolume = value;
        ApplySettings();
        SaveSettings();
    }

    private void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        SaveSettings();
    }

    private void ToggleFullscreen(bool isFullscreen)
    {
        fullscreen = isFullscreen;
        ApplySettings();
        SaveSettings();
    }
}