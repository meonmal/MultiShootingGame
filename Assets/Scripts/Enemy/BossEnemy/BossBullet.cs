using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField]
    private float lifeTime;

    private float currentLifeTime;
    private float moveSpeed;
    private float damage;
    private Vector2 moveDirection;

    private BossBulletPool pool;
    private bool isActiveBullet;

    public void Init(BossBulletPool bossBulletPool)
    {
        pool = bossBulletPool;
    }

    public void Fire(Vector2 direction, float speed, float bulletDamage)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        damage = bulletDamage;

        currentLifeTime = lifeTime;
        isActiveBullet = true;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isActiveBullet)
            return;

        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        currentLifeTime -= Time.deltaTime;

        if (currentLifeTime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActiveBullet)
            return;

        if (collision.CompareTag("Player"))
        {
            IDamageble damageable = collision.GetComponent<IDamageble>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            ReturnToPool();
        }
        else if (collision.CompareTag("Bullet"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (!isActiveBullet)
            return;

        isActiveBullet = false;
        moveDirection = Vector2.zero;
        moveSpeed = 0f;
        damage = 0f;

        pool.ReturnBullet(this);
    }
}
