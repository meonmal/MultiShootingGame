using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    private const string RESOLUTION_INDEX_KEY = "RESOLUTION_INDEX";
    private const string FULLSCREEN_KEY = "FULLSCREEN";

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;

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
    }

    private void OnDestroy()
    {
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        fullScreenToggle.onValueChanged.RemoveListener(OnFullScreenChanged);
        bgmSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
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
