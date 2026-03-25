using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    /// <summary>
    /// PlayerPrefs에 저장할 해상도 인덱스 키.
    /// </summary>
    private const string RESOLUTION_INDEX_KEY = "RESOLUTION_INDEX";
    /// <summary>
    /// PlayerPrefs에 저장할 전체화면 여부 키.
    /// 1이면 전체화면, 0이면 창모드로 사용한다.
    /// </summary>
    private const string FULLSCREEN_KEY = "FULLSCREEN";
    /// <summary>
    /// PlayerPrefs에 저장할 안티 앨리어싱 키.
    /// 드롭다운의 인덱스 값을 그대로 저장한다.
    /// </summary>
    private const string ANTI_ALIASING_KEY = "ANTI_ALIASING";
    /// <summary>
    /// PlayerPrefs에 저장할 프레임 제한 키.
    /// 실제 프레임 값(30, 60, 144)을 저장한다.
    /// </summary>
    private const string FRAME_RATE_KEY = "FRAME_RATE";

    /// <summary>
    /// 해상도 선택 드롭다운.
    /// </summary>
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    /// <summary>
    /// 전체화면 여부를 선택하는 토글.
    /// </summary>
    [SerializeField]
    private Toggle fullScreenToggle;
    /// <summary>
    /// BGM 볼륨 조절 슬라이더.
    /// </summary>
    [SerializeField]
    private Slider bgmSlider;
    /// <summary>
    /// SFX 볼륨 조절 슬라이더.
    /// </summary>
    [SerializeField]
    private Slider sfxSlider;
    /// <summary>
    /// 안티 앨리어싱 선택 드롭다운.
    /// </summary>
    [SerializeField]
    private TMP_Dropdown antiAliasingDropdown;
    /// <summary>
    /// 프레임 제한 선택 드롭다운.
    /// </summary>
    [SerializeField]
    private TMP_Dropdown frameRateDropdown;
    /// <summary>
    /// URP 카메라 설정을 바꾸기 위한 메인 카메라.
    /// </summary>
    [SerializeField]
    private Camera mainCamera;

    /// <summary>
    /// 현재 기기에서 지원하는 전체 해상도 목록.
    /// </summary>
    private Resolution[] resolutions;
    /// <summary>
    /// 중복 해상도를 제거한 뒤 실제로 드롭다운에 사용할 해상도 목록.
    /// 같은 width / height는 하나만 남긴다.
    /// </summary>
    private readonly List<Resolution> filteredResolutions = new();

    /// <summary>
    /// 현재 화면 해상도와 일치하는 해상도의 인덱스.
    /// 저장된 값이 없을 때 기본값으로 사용된다.
    /// </summary>
    private int currentResolutionIndex;

    /// <summary>
    /// 프레임 드롭다운과 실제 프레임 값을 매핑하기 위한 배열.
    /// 0 = 30, 1 = 60, 2 = 144 로 사용한다.
    /// </summary>
    private readonly int[] frameRates = { 30, 60, 144 };

    /// <summary>
    /// 시작 시 설정 UI를 초기화하는 함수.
    /// 해상도 목록 생성 -> 저장된 디스플레이 설정 불러오기 -> 사운드 UI 반영 -> 이벤트 연결 순서로 진행한다.
    /// </summary>
    private void Start()
    {
        InitResolution();
        LoadDisplaySettings();
        InitSoundUI();
        BindUIEvents();
    }

    /// <summary>
    /// 현재 기기에서 지원하는 해상도 목록을 가져온 뒤,
    /// width x height 기준으로 중복을 제거해서 드롭다운에 표시하는 함수.
    /// </summary>
    private void InitResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions.Clear();

        if (resolutionDropdown == null)
        {
            return;
        }

        resolutionDropdown.ClearOptions();

        List<string> options = new();
        HashSet<string> addedResolutionSet = new();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i];
            string option = $"{resolution.width} x {resolution.height}";

            // 이미 같은 해상도가 추가된 경우에는 넘어간다.
            // 주사율이 다르더라도 현재 프로젝트에서는 해상도만 보여주기 때문에 하나만 사용한다.
            if (addedResolutionSet.Contains(option))
            {
                continue;
            }

            addedResolutionSet.Add(option);
            filteredResolutions.Add(resolution);
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        // 현재 실행 중인 화면 해상도와 같은 항목의 인덱스를 찾아둔다.
        currentResolutionIndex = GetCurrentResolutionIndex();
    }

    /// <summary>
    /// 현재 실행 중인 화면의 width / height와 일치하는 해상도 인덱스를 찾는 함수.
    /// 일치하는 해상도가 없으면 0번 인덱스를 반환한다.
    /// </summary>
    /// <returns>현재 화면과 일치하는 해상도 인덱스</returns>
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

    /// <summary>
    /// 저장된 해상도 / 전체화면 설정을 불러와 실제 화면에 적용하고,
    /// 그 결과를 UI에도 반영하는 함수.
    /// 이후 그래픽 설정(안티 앨리어싱, 프레임 제한)도 함께 불러온다.
    /// </summary>
    private void LoadDisplaySettings()
    {
        bool hasResolutionData = filteredResolutions.Count > 0;

        int savedResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, currentResolutionIndex);
        bool isFullScreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;

        // 저장된 해상도 인덱스가 범위를 벗어나면 현재 화면 기준 인덱스로 되돌린다.
        if (!hasResolutionData || savedResolutionIndex < 0 || savedResolutionIndex >= filteredResolutions.Count)
        {
            savedResolutionIndex = currentResolutionIndex;
        }

        ApplyResolution(savedResolutionIndex, isFullScreen);

        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = savedResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = isFullScreen;
        }

        LoadGraphicsSettings();
    }

    /// <summary>
    /// 저장된 그래픽 옵션을 불러와 실제 게임에 적용하고,
    /// 드롭다운 UI 상태도 맞춰주는 함수.
    /// </summary>
    private void LoadGraphicsSettings()
    {
        // 기본값은 기존 코드와 동일하게 유지한다.
        // 안티 앨리어싱은 2(SMAA), 프레임 제한은 60을 기본값으로 사용한다.
        int antiAliasingIndex = PlayerPrefs.GetInt(ANTI_ALIASING_KEY, 2);
        int frameRate = PlayerPrefs.GetInt(FRAME_RATE_KEY, 60);

        ApplyAntiAliasing(antiAliasingIndex);
        ApplyFrameRate(frameRate);

        if (antiAliasingDropdown != null)
        {
            antiAliasingDropdown.value = antiAliasingIndex;
            antiAliasingDropdown.RefreshShownValue();
        }

        if (frameRateDropdown != null)
        {
            frameRateDropdown.value = GetFrameDropdownIndex(frameRate);
            frameRateDropdown.RefreshShownValue();
        }
    }

    /// <summary>
    /// 드롭다운 인덱스에 따라 카메라의 안티 앨리어싱 설정을 적용하는 함수.
    /// 0 = None, 1 = FXAA, 2 = SMAA
    /// </summary>
    /// <param name="index">드롭다운에서 선택된 안티 앨리어싱 인덱스</param>
    private void ApplyAntiAliasing(int index)
    {
        // 메인 카메라가 없으면 더 이상 진행할 수 없다.
        if (mainCamera == null)
        {
            return;
        }

        // URP 카메라 추가 데이터가 있어야 antialiasing 값을 변경할 수 있다.
        UniversalAdditionalCameraData additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();

        if (additionalData == null)
        {
            return;
        }

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

            default:
                additionalData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
        }
    }

    /// <summary>
    /// 실제 게임의 프레임 제한을 적용하는 함수.
    /// vSync는 끄고, Application.targetFrameRate 값을 사용한다.
    /// </summary>
    /// <param name="frame">적용할 목표 프레임 값</param>
    private void ApplyFrameRate(int frame)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frame;
    }

    /// <summary>
    /// 저장된 사운드 값을 슬라이더에 반영하는 함수.
    /// SoundManager가 없는 경우에는 아무 작업도 하지 않는다.
    /// </summary>
    private void InitSoundUI()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        if (bgmSlider != null)
        {
            bgmSlider.value = SoundManager.Instance.GetBgmVolume();
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = SoundManager.Instance.GetSfxVolume();
        }
    }

    /// <summary>
    /// UI 이벤트를 등록하는 함수.
    /// 사용자가 드롭다운, 토글, 슬라이더 값을 바꾸면 각각의 처리 함수가 호출된다.
    /// </summary>
    private void BindUIEvents()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        if (fullScreenToggle != null)
        {
            fullScreenToggle.onValueChanged.AddListener(OnFullScreenChanged);
        }

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        if (antiAliasingDropdown != null)
        {
            antiAliasingDropdown.onValueChanged.AddListener(OnAntiAliasingChanged);
        }

        if (frameRateDropdown != null)
        {
            frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        }
    }

    /// <summary>
    /// 해상도 드롭다운 값이 바뀌었을 때 호출되는 함수.
    /// 선택한 해상도를 즉시 적용하고, 해당 인덱스를 저장한다.
    /// </summary>
    /// <param name="index">선택된 해상도 인덱스</param>
    public void OnResolutionChanged(int index)
    {
        currentResolutionIndex = index;

        bool isFullScreen = fullScreenToggle != null && fullScreenToggle.isOn;
        ApplyResolution(currentResolutionIndex, isFullScreen);

        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, currentResolutionIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 전체화면 토글 값이 바뀌었을 때 호출되는 함수.
    /// 현재 선택된 해상도를 유지한 채 전체화면 여부만 바꿔 적용하고 저장한다.
    /// </summary>
    /// <param name="isFullScreen">전체화면 여부</param>
    public void OnFullScreenChanged(bool isFullScreen)
    {
        int resolutionIndex = resolutionDropdown != null ? resolutionDropdown.value : currentResolutionIndex;

        ApplyResolution(resolutionIndex, isFullScreen);

        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// BGM 슬라이더 값이 바뀌었을 때 호출되는 함수.
    /// SoundManager가 존재하는 경우에만 실제 볼륨에 반영한다.
    /// </summary>
    /// <param name="volume">적용할 BGM 볼륨</param>
    public void OnBgmVolumeChanged(float volume)
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.SetBgmVolume(volume);
    }

    /// <summary>
    /// SFX 슬라이더 값이 바뀌었을 때 호출되는 함수.
    /// SoundManager가 존재하는 경우에만 실제 볼륨에 반영한다.
    /// </summary>
    /// <param name="volume">적용할 SFX 볼륨</param>
    public void OnSfxVolumeChanged(float volume)
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.SetSfxVolume(volume);
    }

    /// <summary>
    /// 안티 앨리어싱 드롭다운 값이 바뀌었을 때 호출되는 함수.
    /// 선택한 인덱스를 즉시 적용하고 저장한다.
    /// </summary>
    /// <param name="index">선택된 안티 앨리어싱 인덱스</param>
    public void OnAntiAliasingChanged(int index)
    {
        ApplyAntiAliasing(index);

        PlayerPrefs.SetInt(ANTI_ALIASING_KEY, index);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 프레임 제한 드롭다운 값이 바뀌었을 때 호출되는 함수.
    /// 드롭다운 인덱스를 실제 프레임 값으로 변환해서 적용하고 저장한다.
    /// </summary>
    /// <param name="index">선택된 프레임 드롭다운 인덱스</param>
    public void OnFrameRateChanged(int index)
    {
        int frame = GetFrameFromIndex(index);

        ApplyFrameRate(frame);

        PlayerPrefs.SetInt(FRAME_RATE_KEY, frame);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 선택된 해상도 인덱스와 전체화면 여부를 실제 화면에 적용하는 함수.
    /// 인덱스가 범위를 벗어나면 아무 작업도 하지 않는다.
    /// </summary>
    /// <param name="index">적용할 해상도 인덱스</param>
    /// <param name="isFullScreen">전체화면 여부</param>
    private void ApplyResolution(int index, bool isFullScreen)
    {
        if (index < 0 || index >= filteredResolutions.Count)
        {
            return;
        }

        Resolution resolution = filteredResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
    }

    /// <summary>
    /// 드롭다운 인덱스를 실제 프레임 값으로 바꾸는 함수.
    /// 범위를 벗어나면 기본값 60을 반환한다.
    /// </summary>
    /// <param name="index">프레임 드롭다운 인덱스</param>
    /// <returns>실제 프레임 값</returns>
    private int GetFrameFromIndex(int index)
    {
        if (index < 0 || index >= frameRates.Length)
        {
            return 60;
        }

        return frameRates[index];
    }

    /// <summary>
    /// 실제 프레임 값을 드롭다운 인덱스로 바꾸는 함수.
    /// 등록되지 않은 값이면 기본적으로 60fps 인덱스를 반환한다.
    /// </summary>
    /// <param name="frame">실제 프레임 값</param>
    /// <returns>드롭다운 인덱스</returns>
    private int GetFrameDropdownIndex(int frame)
    {
        for (int i = 0; i < frameRates.Length; i++)
        {
            if (frameRates[i] == frame)
            {
                return i;
            }
        }

        return 1;
    }

    /// <summary>
    /// 설정창을 여는 함수.
    /// 게임 시간을 멈추고, 이 오브젝트를 활성화한다.
    /// 이 스크립트가 설정 패널 오브젝트에 붙어 있다는 전제로 사용한다.
    /// </summary>
    public void SettingOn()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 설정창을 닫는 함수.
    /// 게임 시간을 다시 흐르게 하고, 이 오브젝트를 비활성화한다.
    /// 이 스크립트가 설정 패널 오브젝트에 붙어 있다는 전제로 사용한다.
    /// </summary>
    public void SettingClose()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 게임 종료 함수.
    /// 에디터에서는 플레이 모드를 종료하고,
    /// 빌드된 게임에서는 실제로 프로그램을 종료한다.
    /// </summary>
    public void GameExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
