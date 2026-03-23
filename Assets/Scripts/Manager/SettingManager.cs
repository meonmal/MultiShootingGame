using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    private const string RESOLUTION_INDEX_KEY = "RESOLUTION_INDEX";
    private const string FULLSCREEN_KEY = "FULLSCREEN";
    private const string ANTI_ALIASING_KEY = "ANTI_ALIASING";
    private const string FRAME_RATE_KEY = "FRAME_RATE";

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private TMP_Dropdown antiAliasingDropdown;
    [SerializeField]
    private TMP_Dropdown frameRateDropdown;
    [SerializeField]
    private Camera mainCamera;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new();

    private int currentResolutionIndex;

    private void Start()
    {
        InitResolution();
        LoadDisplaySettings();
        InitSoundUI();
        BindUIEvents();
    }

    private void InitResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions.Clear();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> addedResolutionSet = new HashSet<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i];
            string option = $"{resolution.width} x {resolution.height}";

            if (addedResolutionSet.Contains(option))
                continue;

            addedResolutionSet.Add(option);
            filteredResolutions.Add(resolution);
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        currentResolutionIndex = GetCurrentResolutionIndex();
    }

    private int GetCurrentResolutionIndex()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == screenWidth &&
                filteredResolutions[i].height == screenHeight)
            {
                return i;
            }
        }

        return 0;
    }

    private void LoadDisplaySettings()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, currentResolutionIndex);
        bool isFullScreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;

        if (savedResolutionIndex < 0 || savedResolutionIndex >= filteredResolutions.Count)
        {
            savedResolutionIndex = currentResolutionIndex;
        }

        ApplyResolution(savedResolutionIndex, isFullScreen);

        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullScreenToggle.isOn = isFullScreen;

        LoadGraphicsSettings();
    }

    private void LoadGraphicsSettings()
    {
        int aa = PlayerPrefs.GetInt(ANTI_ALIASING_KEY, 2); // 기본 SMAA
        int frame = PlayerPrefs.GetInt(FRAME_RATE_KEY, 60);

        ApplyAntiAliasing(aa);
        ApplyFrameRate(frame);

        Debug.Log($"aa = {aa}, frame = {frame}");

        antiAliasingDropdown.value = aa;
        antiAliasingDropdown.RefreshShownValue();

        frameRateDropdown.value = GetFrameDropdownIndex(frame);
        frameRateDropdown.RefreshShownValue();

        Debug.Log($"aa dropdown = {antiAliasingDropdown.value}, frame dropdown = {frameRateDropdown.value}");
    }

    private void ApplyAntiAliasing(int index)
    {
        var additionalData = mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

        switch (index)
        {
            case 0:
                additionalData.antialiasing = AntialiasingMode.None;
                break;
            case 1:
                additionalData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                break;
            case 2:
                additionalData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
        }
    }

    private void ApplyFrameRate(int frame)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frame;
    }

    public void OnAntiAliasingChanged(int index)
    {
        ApplyAntiAliasing(index);

        PlayerPrefs.SetInt(ANTI_ALIASING_KEY, index);
        PlayerPrefs.Save();
    }

    public void OnFrameRateChanged(int index)
    {
        int frame = GetFrameFromIndex(index);

        ApplyFrameRate(frame);

        PlayerPrefs.SetInt(FRAME_RATE_KEY, frame);
        PlayerPrefs.Save();
    }

    private int GetFrameFromIndex(int index)
    {
        switch (index)
        {
            case 0: return 30;
            case 1: return 60;
            case 2: return 144;
        }
        return 60;
    }

    private int GetFrameDropdownIndex(int frame)
    {
        switch (frame)
        {
            case 30: return 0;
            case 60: return 1;
            case 144: return 2;
        }
        return 1;
    }

    private void InitSoundUI()
    {
        if (SoundManager.Instance == null)
            return;

        bgmSlider.value = SoundManager.Instance.GetBgmVolume();
        sfxSlider.value = SoundManager.Instance.GetSfxVolume();
    }

    private void BindUIEvents()
    {
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullScreenToggle.onValueChanged.AddListener(OnFullScreenChanged);
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        antiAliasingDropdown.onValueChanged.AddListener(OnAntiAliasingChanged);
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
    }

    private void OnDestroy()
    {
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        fullScreenToggle.onValueChanged.RemoveListener(OnFullScreenChanged);
        bgmSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        antiAliasingDropdown.onValueChanged.RemoveListener(OnAntiAliasingChanged);
        frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateChanged);
    }

    public void OnResolutionChanged(int index)
    {
        currentResolutionIndex = index;
        ApplyResolution(currentResolutionIndex, fullScreenToggle.isOn);

        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, currentResolutionIndex);
        PlayerPrefs.Save();
    }

    public void OnFullScreenChanged(bool isFullScreen)
    {
        ApplyResolution(resolutionDropdown.value, isFullScreen);

        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnBgmVolumeChanged(float volume)
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.SetBgmVolume(volume);
    }

    public void OnSfxVolumeChanged(float volume)
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.SetSfxVolume(volume);
    }

    private void ApplyResolution(int index, bool isFullScreen)
    {
        if (index < 0 || index >= filteredResolutions.Count)
            return;

        Resolution resolution = filteredResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
    }

    public void SettingOn()
    {
        Time.timeScale = 0f;

        gameObject.SetActive(true);
    }

    public void SettingClose()
    {
        Time.timeScale = 1f;

        gameObject.SetActive(false);
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
        // 에디터에서 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 빌드된 게임 종료
        Application.Quit();
        #endif
    }
}
