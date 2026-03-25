using System;
using System.Collections;
using UnityEngine;

public abstract class BossBase : MonoBehaviour, IDamageble
{
    /// <summary>
    /// 보스가 죽었을 때 실행할 함수들을 등록해놓는 통로.
    /// </summary>
    public event Action<BossBase> OnDead;

    /// <summary>
    /// 보스의 최대 체력.
    /// </summary>
    [SerializeField]
    protected float maxHp;
    /// <summary>
    /// 보스의 이동 속도.
    /// </summary>
    [SerializeField]
    protected float moveSpeed;
    /// <summary>
    /// 보스의 이동 범위.
    /// </summary>
    [SerializeField]
    protected float moveRange;
    /// <summary>
    /// 보스가 이동할 때 중심이 되는 곳.
    /// </summary>
    [SerializeField]
    protected Transform moveCenter;
    /// <summary>
    /// 보스의 이름.
    /// </summary>
    [SerializeField]
    protected string bossName;

    /// <summary>
    /// 보스의 현재 체력.
    /// </summary>
    protected float currentHp;
    /// <summary>
    /// 보스의 타겟(플레이어)
    /// </summary>
    protected Transform target;
    /// <summary>
    /// 보스가 죽었는지 판별 여부.
    /// </summary>
    protected bool isDead;
    /// <summary>
    /// 객체가 초기화 되었는지 체크하는 플래그.
    /// </summary>
    protected bool isInitialized;
    /// <summary>
    /// 보스의 패턴 코루틴 함수들.
    /// </summary>
    protected Coroutine patternCoroutine;

    // 외부에서도 쓸 수 있게 프로퍼티 작성.
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public string BossName => bossName;

    protected virtual void Awake()
    {
        // 현재 체력을 최대 체력으로 초기화.
        currentHp = maxHp;
    }

    /// <summary>
    /// 보스가 생성되면 실핼할 초기화 함수.
    /// </summary>
    /// <param name="targetTransform"></param>
    public virtual void Init(Transform targetTransform)
    {
        // 만약 건너받은 타겟이 없다면 이 함수는 실행을 취소한다.
        if(targetTransform == null)
        {
            return;
        }

        // StageManager에게 받은 타겟을 보스의 타겟으로 설정.
        target = targetTransform;
        // Awake에서 한번 하긴 했지만 그래도 혹시 모르니 초기화 함수에서 한번 더 현재 체력을 설정한다.
        currentHp = maxHp;
        // 보스가 태어나자마자 죽진 않으니 isDead는 fasle.
        isDead = false;
        // 이 함수가 실행되면 초기화가 되었다고 판별.
        isInitialized = true;

        // 패턴 중복 방지.
        BeginBoss();
    }

    /// <summary>
    /// 기존 패턴이 돌고 있으면 끄고 새 패턴을 시작하는 함수.
    /// </summary>
    protected virtual void BeginBoss()
    {
        // 혹시 지금 실행하고 있는 패턴이 있다면
        if(patternCoroutine != null)
        {
            // 실행을 중지한다.
            StopCoroutine(patternCoroutine);
        }

        // 현재 실행중인 코루틴을 저장한다.
        patternCoroutine = StartCoroutine(PatternRoutine());
    }

    /// <summary>
    /// 각 보스가 자신의 공격/이동 패턴을 구현하는 코루틴 함수.
    /// 자식 클래스에서 반드시 구현해야 한다.
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator PatternRoutine();

    /// <summary>
    /// 보스가 데미지를 받게 해주는 함수.
    /// </summary>
    /// <param name="damage">보스가 받는 데미지.</param>
    public virtual void TakeDamage(float damage)
    {
        // 만약 초기화되지 않았거나 현재 죽은 상태라면 함수 실행 취소.
        if(!isInitialized || isDead)
        {
            return;
        }

        // 보스가 받은 데미지만큼 현재 체력을 깎는다.
        currentHp -= damage;
        // 맞을 때 마다 해당 효과음 재생.
        SoundManager.Instance.PlaySfx(SfxType.EnemyHit);

        // 만약 현재 체력이 0과 같거나 적다면 실행.
        if (currentHp <= 0f)
        {
            // 현재 체력을 0으로 맞추고
            currentHp = 0f;
            // Die() 함수 실행.
            Die();
            // 죽을 때 나는 소리를 재생한다.
            SoundManager.Instance.PlaySfx(SfxType.EnemyDead);
        }
    }

    /// <summary>
    /// 보스 몬스터가 죽었을 때 실행할 함수.
    /// </summary>
    protected virtual void Die()
    {
        // 이미 죽은 상태라면 이 함수는 실행하지 않는다.
        if (isDead)
        {
            return;
        }

        // isDead는 true로,
        isDead = true;
        // 초기화 판별 여부는 false로 맞춘다.
        isInitialized = false;

        // 현재 실행하고 있는 패턴이 있다면
        if(patternCoroutine != null)
        {
            // 해당 패턴은 실행을 중지한다.
            StopCoroutine(patternCoroutine);
            patternCoroutine = null;
        }

        // OnDead에 저장한 함수들을 실행하고
        OnDead?.Invoke(this);
        // 해당 보스는 삭제한다.
        Destroy(gameObject);
    }
}
