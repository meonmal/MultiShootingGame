using System.Collections.Generic;
using UnityEngine;

/*
그냥 원본 SO를 그대로 쓰는 것이 아닌 왜 굳이 RunTime상태를 만들어야 하느냐? 
만일 원본 데이터를 그대로 쓰게 되면 그 데이터가 망가질 가능성이 매우 높다.(경험담)
예를 들어 게임에서 이동속도를 높였다고 생각해보자.
그런데 이 올라간 스탯은 게임이 끝나도 계속 올라가 있는 상태로 있을 수 있다.
최악의 경우에는 씬이 바뀌어도 변환된 상태 그대로 일 수 있게 된다.
그것을 예방하기 위해서 이렇게 가짜 스탯을 만들어 게임이 끝나도 원본 데이터를 보호할 수 있다.
또한 버프와 디버프를 계산하기 쉽게 하기 위함도 있다.
 */

public class PlayerRunTimeStats
{
    /// <summary>
    /// StatType을 키로 해서 각 스탯의 런타임 데이터를 관리한다.
    /// </summary>
    private Dictionary<StatType, RunTimeStats> stats;

    /// <summary>
    /// PlayerStats에 저장된 스탯 데이터를 기반으로
    /// 런타임 스탯 Dictionary를 초기화하는 생성자
    /// </summary>
    /// <param name="data">PlayerStats의 스탯</param>
    public PlayerRunTimeStats(PlayerStats data)
    {
        stats = new Dictionary<StatType, RunTimeStats>
        {
            { StatType.MoveSpeed, new RunTimeStats(data.moveSpeed) },
            { StatType.PlayerDamage, new RunTimeStats(data.playerDamage) },
            { StatType.CoolTime, new RunTimeStats(data.coolTime) },
            { StatType.BulletSpeed, new RunTimeStats(data.bulletSpeed) },
            { StatType.PlayerHP, new RunTimeStats(data.playerHP) },
        };
    }

    public List<StatType> GetAvailableStats()
    {
        List<StatType> result = new List<StatType>();

        foreach (var pair in stats)
        {
            if (!pair.Value.IsMax)
            {
                result.Add(pair.Key);
            }
        }

        return result;
    }

    public float GetNextStat(StatType type)
    {
        return stats[type].GetNextValue();
    }

    public float GetDelta(StatType type)
    {
        return stats[type].GetDelta();
    }

    /// <summary>
    /// 외부에서 값을 가져갈 때 쓸 함수
    /// </summary>
    /// <param name="type">쓸 스탯의 키</param>
    /// <returns>현재 레벨에 해당하는 스탯의 값</returns>
    public float GetStat(StatType type) => stats[type].Values;

    /// <summary>
    /// 지정한 스탯의 레벨을 1 증가시킨다.
    /// </summary>
    /// <param name="type">레벨업 할 스탯의 타입</param>
    public void LevelUp(StatType type) => stats[type].LevelUp();

    /// <summary>
    /// 지정한 스탯이 최대 레벨인지 확인한다.
    /// </summary>
    /// <param name="type">확인할 스탯의 타입</param>
    /// <returns>최대 레벨이면 true</returns>
    public bool IsMax(StatType type) => stats[type].IsMax;
}
