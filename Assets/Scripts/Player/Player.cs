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
        runtime.LevelUp(type);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
    }

    public void AddBuff(BuffData buffData)
    {
        buffController.AddBuff(buffData);

        if (uiManager != null)
        {
            uiManager.ShowBuffPopup(buffData.GetDescription(), transform.position);
        }
    }
}
