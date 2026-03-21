using System.Collections.Generic;
using UnityEngine;

public enum BgmType
{
    Title,
    Game,
    Result
}

public enum SfxType
{
    ButtonClick,
    PlayerShoot,
    EnemyHit,
    EnemyDead,
    BuffPickup,
    LevelUp,
    PlayerHit,
    PlayerDie
}

public class SoundManager : MonoBehaviour
{
    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    private const string SFX_VOLUME_KEY = "SFX_VOLUME";

    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class BgmData
    {
        public BgmType type;
        public AudioClip clip;
    }

    [System.Serializable]
    public class SfxData
    {
        public SfxType type;
        public AudioClip clip;
    }

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource sfxSource;

    [Header("Audio Clip Data")]
    [SerializeField]
    private List<BgmData> bgmList = new();
    [SerializeField]
    private List<SfxData> sfxList = new();

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float bgmVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float sfxVolume = 1f;

    private Dictionary<BgmType, AudioClip> bgmDict;
    private Dictionary<SfxType, AudioClip> sfxDict;

    private BgmType? currentBgmType = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    private void Init()
    {
        if (bgmSource == null || sfxSource == null)
        {
            Debug.LogError("SoundManager: AudioSource가 연결되지 않았어.");
            return;
        }

        bgmDict = new Dictionary<BgmType, AudioClip>();
        sfxDict = new Dictionary<SfxType, AudioClip>();

        foreach (var data in bgmList)
        {
            if (data.clip == null)
            {
                Debug.LogWarning($"SoundManager: BGM {data.type} 의 AudioClip이 비어 있어.");
                continue;
            }

            if (bgmDict.ContainsKey(data.type))
            {
                Debug.LogWarning($"SoundManager: BGM {data.type} 이 중복 등록되어 있어.");
                continue;
            }

            bgmDict.Add(data.type, data.clip);
        }

        foreach (var data in sfxList)
        {
            if (data.clip == null)
            {
                Debug.LogWarning($"SoundManager: SFX {data.type} 의 AudioClip이 비어 있어.");
                continue;
            }

            if (sfxDict.ContainsKey(data.type))
            {
                Debug.LogWarning($"SoundManager: SFX {data.type} 이 중복 등록되어 있어.");
                continue;
            }

            sfxDict.Add(data.type, data.clip);
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;

        LoadVolume();
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);

        PlayerPrefs.Save(); // 바로 저장
    }

    private void LoadVolume()
    {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        bgmSource.volume = bgmVolume;
    }

    public void PlayBgm(BgmType type)
    {
        if (currentBgmType.HasValue && currentBgmType.Value == type && bgmSource.isPlaying)
            return;

        if (bgmDict.TryGetValue(type, out AudioClip clip) == false)
        {
            Debug.LogWarning($"SoundManager: BGM {type} 을(를) 찾지 못했어.");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();

        currentBgmType = type;
    }

    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
        currentBgmType = null;
    }

    public void PlaySfx(SfxType type)
    {
        if (sfxDict.TryGetValue(type, out AudioClip clip) == false)
        {
            Debug.LogWarning($"SoundManager: SFX {type} 을(를) 찾지 못했어.");
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;

        SaveVolume();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        SaveVolume();
    }

    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    public float GetSfxVolume()
    {
        return sfxVolume;
    }
}