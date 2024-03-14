using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour {

    public static int AntiAliasingQuality { get; protected set; } = 2;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private BasicChannel controlsChangedChannel;

    [SerializeField] private UnityEvent<int> OnFullscreenModeIndexLoad;
    [SerializeField] private UnityEvent<float> On3DResolutionLoad;
    [SerializeField] private UnityEvent<int> OnAAQualityLoad;
    [SerializeField] private UnityEvent<float> OnMasterVolumeLoad;
    [SerializeField] private UnityEvent<float> OnMusicVolumeLoad;
    [SerializeField] private UnityEvent<float> OnEffectsVolumeLoad;
    [SerializeField] private UnityEvent<float> OnUIVolumeLoad;
    [SerializeField] private UnityEvent<float> OnMouseSensitivityLoad;
    [SerializeField] private UnityEvent<float> OnGamepadSensitivityLoad;
    [SerializeField] private UnityEvent<float> OnAimAssistIntensityLoad;

    private UniversalRenderPipelineAsset originalAsset;
    private UniversalRenderPipelineAsset asset;
    private bool savePrefs = false;

    private void Awake() {
        originalAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        asset = Instantiate(originalAsset);
        QualitySettings.renderPipeline = asset;
    }

    private void OnDestroy() {
        QualitySettings.renderPipeline = originalAsset;
    }

    private void Start() {
        savePrefs = false;
        float renderScale = PlayerPrefs.GetFloat("3DResolution", 1f);
        Set3DResolution(renderScale);
        On3DResolutionLoad?.Invoke(renderScale);

        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(volume);
        OnMasterVolumeLoad?.Invoke(volume);

        volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetMusicVolume(volume);
        OnMusicVolumeLoad?.Invoke(volume);

        volume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        SetEffectsVolume(volume);
        OnEffectsVolumeLoad?.Invoke(volume);

        volume = PlayerPrefs.GetFloat("UIVolume", 1f);
        SetUIVolume(volume);
        OnUIVolumeLoad?.Invoke(volume);

        switch(Screen.fullScreenMode) {
            case FullScreenMode.ExclusiveFullScreen:
                OnFullscreenModeIndexLoad?.Invoke(2);
                break;
            case FullScreenMode.FullScreenWindow:
                OnFullscreenModeIndexLoad?.Invoke(1);
                break;
            case FullScreenMode.MaximizedWindow:
            case FullScreenMode.Windowed:
                OnFullscreenModeIndexLoad?.Invoke(0);
                break;
        }

        resolutionsDropdown.ClearOptions();
        int i = 0;
        foreach(Resolution resolution in Screen.resolutions) {
            resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
            if(resolution.Equals(Screen.currentResolution)) {
                resolutionsDropdown.value = i;
            }
            i++;
        }

        OnMouseSensitivityLoad?.Invoke(PlayerPrefs.GetFloat("MouseSensitivity", 2f));
        OnGamepadSensitivityLoad?.Invoke(PlayerPrefs.GetFloat("GamepadSensitivity", 1f));
        OnAimAssistIntensityLoad?.Invoke(PlayerPrefs.GetFloat("AimAssistIntensity", 0.5f));

        AntiAliasingQuality = PlayerPrefs.GetInt("AAQuality", 1);
        OnAAQualityLoad?.Invoke(AntiAliasingQuality);
        SetAAQuality(AntiAliasingQuality);

        savePrefs = true;
    }

    public float LinearVolumeToDecibels(float volume) {
        if(volume <= 0f) {
            return LinearVolumeToDecibels(Mathf.Epsilon);
        }
        return Mathf.Clamp(20f * Mathf.Log10(volume), -80f, 20f);
    }

    private void SetVolume(float volume, string target) {
        mixer.SetFloat(target, LinearVolumeToDecibels(volume));
        if(savePrefs) {
            PlayerPrefs.SetFloat(target, volume);
        }
    }

    public void SetMasterVolume(float volume) {
        SetVolume(volume, "MasterVolume");
    }

    public void SetMusicVolume(float volume) {
        SetVolume(volume, "MusicVolume");
    }

    public void SetEffectsVolume(float volume) {
        SetVolume(volume, "EffectsVolume");
    }

    public void SetUIVolume(float volume) {
        SetVolume(volume, "UIVolume");
    }

    public void SetFullscreenMode(int mode) {
        switch(mode) {
            case 0:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Screen.fullScreen = false;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.fullScreen = true;
                break;
        }
    }

    public void SetResolutionIndex(int i) {
        Resolution target = Screen.resolutions[i];
        Screen.SetResolution(target.width, target.height, Screen.fullScreenMode, target.refreshRateRatio);
    }

    public void Set3DResolution(float resolution) {
        if(savePrefs) {
            PlayerPrefs.SetFloat("3DResolution", resolution);
        }
        asset.renderScale = resolution;
    }

    public void SetMouseSensitivity(float value) {
        if(savePrefs) {
            PlayerPrefs.SetFloat("MouseSensitivity", value);
        }
        controlsChangedChannel.FireEvent();
    }

    public void SetGamepadSensitivity(float value) {
        if(savePrefs) {
            PlayerPrefs.SetFloat("GamepadSensitivity", value);
        }
        controlsChangedChannel.FireEvent();
    }

    public void SetAimAssistIntensity(float value) {
        if(savePrefs) {
            PlayerPrefs.SetFloat("AimAssistIntensity", value);
        }
        controlsChangedChannel.FireEvent();
    }

    public void SetAAQuality(int value) {
        if(savePrefs) {
            PlayerPrefs.SetInt("AAQuality", value);
        }
        AntiAliasingQuality = value;
        CameraSettings.UpdateAllCameras();
    }

}
