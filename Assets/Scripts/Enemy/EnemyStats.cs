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

    public float EnemyDamage => enemyDamage;

    public float EnemySpeed => enemySpeed;

    public float EnemyHP => enemyHP;

    public float EnemyExp => enemyExp;
}
