using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageble
{
    [SerializeField]
    private EnemyStats enemyStats;

    private float currentHP;

    private Rigidbody2D rigid;
    private IObjectPool<Enemy> _pool;
    private PlayerExperience _playerExperience;

    public void SetPool(IObjectPool<Enemy> pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(PlayerExperience playerExperience)
    {
        _playerExperience = playerExperience;
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
        if(_playerExperience != null)
        {
            _playerExperience.AddExp(enemyStats.EnemyExp);
        }

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
