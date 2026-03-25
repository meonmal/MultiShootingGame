using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using System.Collections;

/// <summary>
/// 현재 스테이지의 진행 상태를 나타내는 Enum.
/// </summary>
public enum StageState
{
    None,       // 초기 상태 (아직 시작 안됨)
    Running,    // 일반 몬스터 진행 중
    BossFight,  // 보스전 진행 중
    Clear,      // 보스 처치 후 클리어 상태
    End,        // 모든 스테이지 종료
}

/// <summary>
/// 스테이지 전체 흐름을 관리하는 매니저.
/// 
/// 역할:
/// - 스테이지 시작 및 전환
/// - 시간 기반 보스 소환
/// - 보스 사망 처리
/// - 다음 스테이지 이동
/// </summary>
public class StageManager : MonoBehaviour
{
    /// <summary>
    /// 스테이지 데이터 배열.
    /// 각 스테이지마다 몬스터, 보스, 등장 시간 등이 정의되어 있다.
    /// </summary>
    [SerializeField]
    private StageData[] stageDatas;

    /// <summary>
    /// 보스가 생성될 위치.
    /// </summary>
    [SerializeField]
    private Transform bossSpawnPoint;

    /// <summary>
    /// 일반 몬스터 스폰을 담당하는 스포너.
    /// </summary>
    [SerializeField]
    private EnemySpawner enemySpawner;

    /// <summary>
    /// 모든 스테이지를 클리어했을 때 보여줄 UI 패널.
    /// </summary>
    [SerializeField]
    private GameObject gameClearPanel;

    /// <summary>
    /// 플레이어 참조.
    /// 보스가 플레이어를 추적하거나 공격할 때 사용된다.
    /// </summary>
    private Player player;

    /// <summary>
    /// 현재 스테이지에 등장한 보스.
    /// </summary>
    private BossBase currentBoss;

    /// <summary>
    /// 현재 스테이지 인덱스.
    /// </summary>
    private int currentStageIndex;

    /// <summary>
    /// 현재 스테이지 진행 시간 (초 단위).
    /// 보스 등장 타이밍 계산에 사용된다.
    /// </summary>
    private float currentStageTime;

    /// <summary>
    /// 보스가 이미 등장했는지 여부.
    /// 중복 소환 방지용.
    /// </summary>
    private bool hasBossSpawned;

    /// <summary>
    /// 현재 스테이지 상태.
    /// </summary>
    private StageState currentState = StageState.None;

    /// <summary>
    /// 현재 보스 접근용 프로퍼티.
    /// </summary>
    public BossBase CurrentBoss => currentBoss;

    /// <summary>
    /// 현재 스테이지 인덱스 접근용 프로퍼티.
    /// </summary>
    public int CurrentStageIndex => currentStageIndex;

    /// <summary>
    /// 현재 스테이지 시간 접근용 프로퍼티.
    /// </summary>
    public float CurrentStageTime => currentStageTime;

    /// <summary>
    /// 외부(GameSceneManager 등)에서 플레이어를 전달받아 초기화한다.
    /// </summary>
    public void Init(Player targetPlayer)
    {
        player = targetPlayer;
    }

    /// <summary>
    /// 시작 시 첫 번째 스테이지를 자동으로 시작한다.
    /// </summary>
    private void Start()
    {
        if (stageDatas == null || stageDatas.Length == 0)
        {
            return;
        }

        StartStage(0);
    }

    /// <summary>
    /// 매 프레임마다 스테이지 진행을 체크한다.
    /// 
    /// Running 상태에서만:
    /// - 시간 증가
    /// - 보스 등장 조건 체크
    /// </summary>
    private void Update()
    {
        // Running 상태가 아니면 시간도 흐르지 않고 로직도 진행하지 않는다.
        if (currentState != StageState.Running)
        {
            return;
        }

        StageData stageData = stageDatas[currentStageIndex];

        // 스테이지 시간 증가
        currentStageTime += Time.deltaTime;

        // 설정된 시간에 도달하면 보스를 소환한다.
        if (!hasBossSpawned && currentStageTime >= stageData.bossSpawnTime)
        {
            SpawnBoss();
        }
    }

    /// <summary>
    /// 특정 스테이지를 시작하는 함수.
    /// 상태 초기화 + 일반 몬스터 설정을 담당한다.
    /// </summary>
    private void StartStage(int stageIndex)
    {
        currentStageIndex = stageIndex;
        currentStageTime = 0f;
        hasBossSpawned = false;
        currentBoss = null;
        currentState = StageState.Running;

        Debug.Log($"스테이지 {stageDatas[currentStageIndex].stageIndex} 시작");

        // 일반 몬스터 스폰 설정
        StageData stageData = stageDatas[currentStageIndex];
        enemySpawner.SetStageEnemies(stageData.normalEnemyPrefabs);
    }

    /// <summary>
    /// 보스를 생성하는 함수.
    /// 
    /// - 상태를 BossFight로 변경
    /// - 보스 생성 및 초기화
    /// - 보스 사망 이벤트 등록
    /// </summary>
    private void SpawnBoss()
    {
        StageData stageData = stageDatas[currentStageIndex];

        // 플레이어나 보스 프리팹이 없으면 생성 불가
        if (player == null || stageData.bossPrefab == null)
        {
            return;
        }

        hasBossSpawned = true;
        currentState = StageState.BossFight;

        // 보스전 BGM으로 변경
        SoundManager.Instance.PlayBgm(BgmType.Boss);

        // 보스 생성
        currentBoss = Instantiate(stageData.bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        // 보스에게 플레이어 위치 전달 (추적/공격용)
        currentBoss.Init(player.transform);

        // 보스 사망 이벤트 구독
        currentBoss.OnDead += HandleBossDead;

        Debug.Log($"보스 등장 : {stageData.bossPrefab.name}");
    }

    /// <summary>
    /// 보스가 사망했을 때 호출되는 함수.
    /// 
    /// - 이벤트 해제
    /// - 상태를 Clear로 변경
    /// - 일반 BGM으로 복귀
    /// - 다음 스테이지 준비
    /// </summary>
    private void HandleBossDead(BossBase boss)
    {
        // 이벤트 해제 (메모리 누수 및 중복 호출 방지)
        boss.OnDead -= HandleBossDead;

        currentBoss = null;
        currentState = StageState.Clear;

        // 일반 전투 BGM으로 변경
        SoundManager.Instance.PlayBgm(BgmType.Game);

        Debug.Log($"스테이지 {stageDatas[currentStageIndex].stageIndex} 클리어");

        // 일정 시간 후 다음 스테이지로 이동
        StartCoroutine(NextStageRoutine());
    }

    /// <summary>
    /// 다음 스테이지로 이동하는 함수.
    /// 마지막 스테이지면 게임 종료 처리.
    /// </summary>
    private void GoToNextStage()
    {
        currentStageIndex++;

        // 모든 스테이지를 클리어한 경우
        if (currentStageIndex >= stageDatas.Length)
        {
            currentState = StageState.End;

            Debug.Log("모든 스테이지 클리어");

            // 게임 정지 + 클리어 UI 표시
            Time.timeScale = 0f;
            gameClearPanel.SetActive(true);
            return;
        }

        // 다음 스테이지 시작
        StartStage(currentStageIndex);
    }

    /// <summary>
    /// 보스 처치 후 잠시 대기한 뒤 다음 스테이지로 넘어가는 코루틴.
    /// </summary>
    private IEnumerator NextStageRoutine()
    {
        // 클리어 연출을 위해 2초 대기
        yield return new WaitForSeconds(2f);

        GoToNextStage();
    }

    /// <summary>
    /// 현재 스테이지 진행 시간을 반환한다.
    /// </summary>
    public float GetCurrentStageTime()
    {
        return currentStageTime;
    }

    /// <summary>
    /// 현재 스테이지 상태를 반환한다.
    /// </summary>
    public StageState GetCurrentState()
    {
        return currentState;
    }
}
