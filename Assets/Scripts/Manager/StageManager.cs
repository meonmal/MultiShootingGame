using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using System.Collections;

public enum StageState
{
    None,
    Running,
    BossFight,
    Clear,
    End,
}

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private StageData[] stageDatas;
    [SerializeField]
    private Transform bossSpawnPoint;

    private Player player;
    private BossBase currentBoss;

    private int currentStageIndex;
    private float currentStageTime;
    private bool hasBossSpawned;
    private StageState currentState = StageState.None;

    public BossBase CurrentBoss => currentBoss;

    public void Init(Player targetPlayer)
    {
        player = targetPlayer;
    }

    private void Start()
    {
        if(stageDatas == null || stageDatas.Length == 0)
        {
            return;
        }

        StartStage(0);
    }

    private void Update()
    {
        if(currentState != StageState.Running)
        {
            return;
        }

        StageData stageData = stageDatas[currentStageIndex];

        currentStageTime += Time.deltaTime;

        if(!hasBossSpawned && currentStageTime >= stageData.bossSpawnTime)
        {
            SpawnBoss();
        }
    }

    private void StartStage(int stageIndex)
    {
        currentStageIndex = stageIndex;
        currentStageTime = 0f;
        hasBossSpawned = false;
        currentBoss = null;
        currentState = StageState.Running;

        Debug.Log($"스테이지 {stageDatas[currentStageIndex].stageIndex} 시작");
    }

    private void SpawnBoss()
    {
        StageData stageData = stageDatas[currentStageIndex];

        if(player == null || stageData.bossPrefab == null)
        {
            return;
        }

        hasBossSpawned = true;
        currentState = StageState.BossFight;

        currentBoss = Instantiate(stageData.bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        currentBoss.Init(player.transform);
        currentBoss.OnDead += HandleBossDead;

        Debug.Log($"보스 등장 : {stageData.bossPrefab.name}");
    }

    private void HandleBossDead(BossBase boss)
    {
        boss.OnDead -= HandleBossDead;
        currentBoss = null;
        currentState = StageState.Clear;

        Debug.Log($"스테이지 {stageDatas[currentStageIndex].stageIndex} 클리어");

        StartCoroutine(NextStageRoutine());
    }

    private void GoToNextStage()
    {
        currentStageIndex++;

        if (currentStageIndex >= stageDatas.Length)
        {
            currentState = StageState.End;
            Debug.Log("모든 스테이지 클리어");
            return;
        }

        StartStage(currentStageIndex);
    }

    private IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(2f);

        GoToNextStage();
    }

    public float GetCurrentStageTime()
    {
        return currentStageTime;
    }

    public StageState GetCurrentState()
    {
        return currentState;
    }
}
