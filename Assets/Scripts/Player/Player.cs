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
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private BuffUI buffUI;

    private float currentHP;

    private PlayerRunTimeStats runtime;
    private PlayerExperience playerexperience;
    private PlayerBuffController buffController;

    public float CurrentHP => currentHP;
    public PlayerExperience PlayerExperience => playerexperience;
    public PlayerRunTimeStats RunTimeStats => runtime;
    
    private void Awake()
    {
        buffController = GetComponent<PlayerBuffController>();
        playerexperience = GetComponent<PlayerExperience>();
        runtime = new PlayerRunTimeStats(playerStats,buffController);
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
        if (type == StatType.PlayerHP)
        {
            float oldMaxHP = runtime.GetBaseStat(StatType.PlayerHP);

            runtime.LevelUp(type);

            float newMaxHP = runtime.GetBaseStat(StatType.PlayerHP);
            float increase = newMaxHP - oldMaxHP;

            currentHP += increase;
            currentHP = Mathf.Clamp(currentHP, 0, newMaxHP);
        }
        else
        {
            runtime.LevelUp(type);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, GetStats(StatType.PlayerHP));
    }

    public void AddBuff(BuffData buffData)
    {
        buffController.AddBuff(buffData);
        buffUI.AddBuff(buffData);

        if (uiManager != null)
        {
            uiManager.ShowBuffPopup(buffData.GetDescription(), transform.position);
        }
    }

    public float GetBaseStats(StatType type)
    {
        return runtime.GetBaseStat(type);
    }

    public float GetNextBaseStats(StatType type)
    {
        return runtime.GetNextStat(type);
    }

    public float GetBaseDeltaStats(StatType type)
    {
        return runtime.GetDelta(type);
    }
}
