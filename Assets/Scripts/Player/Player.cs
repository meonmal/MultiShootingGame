using UnityEngine;

public class Player : MonoBehaviour, IDamageble
{
    /// <summary>
    /// 플레이어의 스탯 SO
    /// </summary>
    [SerializeField]
    private PlayerStats playerStats;

    private float currentHP;

    private PlayerRunTimeStats runtime;
    
    private void Awake()
    {
        runtime = new PlayerRunTimeStats(playerStats);
    }

    private void Start()
    {
        currentHP = GetStats(StatType.PlayerHP);
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

        Debug.Log(currentHP);
    }
}
