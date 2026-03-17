using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageble
{
    [SerializeField]
    private EnemyStats enemyStats;

    private float currentHP;

    private Rigidbody2D rigid;
    private IObjectPool<Enemy> _pool;

    public void SetPool(IObjectPool<Enemy> pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        currentHP = enemyStats.EnemyHP;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageble iDamageble = collision.GetComponent<IDamageble>();

        if(iDamageble != null && collision.CompareTag("Player"))
        {
            iDamageble.TakeDamage(enemyStats.EnemyDamage);
            ThisRelease();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if(currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_pool == null)
        {
            Destroy(gameObject);
            return;
        }

        _pool.Release(this);
    }

    private void Movement()
    {
        rigid.linearVelocity = Vector2.left * enemyStats.EnemySpeed;
    }

    private void ThisRelease()
    {
        this._pool.Release(this);
    }
}
