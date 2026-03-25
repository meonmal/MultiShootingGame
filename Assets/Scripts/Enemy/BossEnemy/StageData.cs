using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    /// <summary>
    /// 스테이지 별로 다르게 나올 적 프리팹.
    /// </summary>
    public Enemy[] normalEnemyPrefabs;
    /// <summary>
    /// 해당 스테이지.
    /// </summary>
    public int stageIndex;
    /// <summary>
    /// 보스 소환 시간.
    /// </summary>
    public float bossSpawnTime;
    /// <summary>
    /// 해당 스테이지에 나올 보스.
    /// </summary>
    public BossBase bossPrefab;
}
