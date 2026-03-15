using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 스탯 SO
    /// </summary>
    [SerializeField]
    private PlayerStats playerStats;

    private PlayerRunTimeStats runtime;

    private void Awake()
    {
        runtime = new PlayerRunTimeStats(playerStats);
    }

    public float GetStats(StatType type)
    {
        return runtime.GetStat(type);
    }

    public void LeveUp(StatType type)
    {
        runtime.LevelUp(type);
    }
}
