using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    MoveSpeed,
    PlayerDamage,
    CoolTime,
    BulletSpeed, 
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    /*
    모든 스탯은 쉬운 관리를 위해 
    전부 float 타입으로 선언한다.
     */


    /// <summary>
    /// 플레이어의 이동속도
    /// </summary>
    public List<float> moveSpeed;

    /// <summary>
    /// 플레이어의 데미지
    /// </summary>
    public List<float> playerDamage;

    /// <summary>
    /// 발사 쿨타임
    /// </summary>
    public List<float> coolTime;

    /// <summary>
    /// 총알의 이동속도
    /// </summary>
    public List<float> bulletSpeed;
}
