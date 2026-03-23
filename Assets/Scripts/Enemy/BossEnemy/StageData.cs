using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public int stageIndex;
    public float bossSpawnTime;
    public BossBase bossPrefab;
}
