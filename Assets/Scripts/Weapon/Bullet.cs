using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private float speed;

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

    private void Movement()
    {
        speed = player.GetStats(StatType.BulletSpeed);

        rigid.linearVelocity = Vector2.right * speed;
    }
}
