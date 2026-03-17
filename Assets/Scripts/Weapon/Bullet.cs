using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float xLimit;

    private float speed;
    private float damage;
    private Rigidbody2D rigid;
    private Player player;

    private IObjectPool<Bullet> _pool;

    public void SetPool(IObjectPool<Bullet> pool)
    {
        _pool = pool;
    }


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<Player>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageble iDamageble = collision.GetComponent<IDamageble>();

        if(iDamageble != null && collision.CompareTag("Enemy"))
        {
            damage = player.GetStats(StatType.PlayerDamage);

            iDamageble.TakeDamage(damage);

            ThisRelease();
        }
    }

    private void Movement()
    {
        speed = player.GetStats(StatType.BulletSpeed);

        rigid.linearVelocity = Vector2.right * speed;

        if(transform.position.x >= xLimit)
        {
            ThisRelease();
        }
    }

    private void ThisRelease()
    {
        this._pool.Release(this);
    }
}
