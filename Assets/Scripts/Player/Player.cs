using UnityEngine;

public class Player : MonoBehaviour, IDamageble
{
    /// <summary>
    /// 플레이어의 스탯 SO
    /// </summary>
    [SerializeField]
    private PlayerStats playerStats;
    [SerializeField]
    private LevelUpManager levelUpManager;

    private float currentHP;

    private PlayerRunTimeStats runtime;
    private PlayerExperience playerexperience;

    public float CurrentHP => currentHP;
    public PlayerExperience PlayerExperience => playerexperience;
    public PlayerRunTimeStats RunTimeStats => runtime;
    
    private void Awake()
    {
        runtime = new PlayerRunTimeStats(playerStats);
        playerexperience = GetComponent<PlayerExperience>();
    }

    private void Start()
    {
        currentHP = GetStats(StatType.PlayerHP);
        levelUpManager.Init(this);
    }

    public float GetStats(StatType type)
    {
        return runtime.GetStat(type);
    }

    public void LeveUp(StatType type)
    {
        runtime.LevelUp(type);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
    }
}
