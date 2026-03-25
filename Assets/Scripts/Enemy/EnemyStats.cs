using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [SerializeField]
    private float enemyDamage;

    [SerializeField]
    private float enemySpeed;

    [SerializeField]
    private float enemyHP;

    [SerializeField]
    private float enemyExp;

    /*
    적은 버프, 디버프가 필요 없고 ScriptableObject를 몬스터의 갯수대로 만들면 되기 때문에
    굳이 Runtime전용 상태가 필요 없다.
    때문에 Enemy 스크립트의 OnEnable에서 초기화만 진행해준다.
    또한 값을 수정할 필요도 없기 때문에 읽기 전용으로 프로퍼티를 써준다.
     */

    /// <summary>
    /// 적의 데미지.
    /// </summary>
    public float EnemyDamage => enemyDamage;

    /// <summary>
    /// 적의 이동속도.
    /// </summary>
    public float EnemySpeed => enemySpeed;

    /// <summary>
    /// 적의 최대 체력.
    /// </summary>
    public float EnemyHP => enemyHP;

    /// <summary>
    /// 적을 죽였을 때 플레이어가 얻을 경험치양
    /// </summary>
    public float EnemyExp => enemyExp;
}
