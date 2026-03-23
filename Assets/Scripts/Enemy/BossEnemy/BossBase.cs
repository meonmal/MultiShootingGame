using System;
using System.Collections;
using UnityEngine;

public abstract class BossBase : MonoBehaviour, IDamageble
{
    public event Action<BossBase> OnDead;

    [SerializeField]
    private protected float maxHp;
    [SerializeField]
    private protected float moveSpeed;
    [SerializeField]
    private protected float moveRange;
    [SerializeField]
    private protected Transform moveCenter;
    [SerializeField]
    private protected string bossName;

    protected float currentHp;
    protected Transform target;
    protected bool isDead;
    protected bool isInitialized;
    protected Coroutine patternCoroutine;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public string BossName => bossName;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void Init(Transform targetTransform)
    {
        target = targetTransform;
        currentHp = maxHp;
        isDead = false;
        isInitialized = true;

        BeginBoss();
    }

    protected virtual void BeginBoss()
    {
        if(patternCoroutine != null)
        {
            StopCoroutine(patternCoroutine);
        }

        patternCoroutine = StartCoroutine(PatternRoutine());
    }

    protected abstract IEnumerator PatternRoutine();

    public virtual void TakeDamage(float damage)
    {
        if(!isInitialized || isDead)
        {
            return;
        }

        currentHp -= damage;

        if(currentHp <= 0f)
        {
            currentHp = 0f;
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        isInitialized = false;

        if(patternCoroutine != null)
        {
            StopCoroutine(patternCoroutine);
            patternCoroutine = null;
        }

        OnDead?.Invoke(this);
        Destroy(gameObject);
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
