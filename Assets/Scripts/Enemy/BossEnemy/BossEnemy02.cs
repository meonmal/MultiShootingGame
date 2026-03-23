using UnityEngine;
using System.Collections;

public class BossEnemy02 : BossBase
{
    [SerializeField]
    private float stopX;
    [SerializeField]
    private BossBulletPool bulletPool;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private float fireInterval;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float bulletDamage;
    [SerializeField]
    private float spreadAngle = 15f;

    private int moveDirection = 1;
    private float fireTimer;
    private bool hasEntered;
    private float centerY;

    protected override IEnumerator PatternRoutine()
    {
        yield return EnterRoutine();

        centerY = transform.position.y;
        fireTimer = 0f;
        moveDirection = 1;
        hasEntered = true;

        while (!isDead)
        {
            Move();
            HandleFire();

            yield return null;
        }
    }

    private IEnumerator EnterRoutine()
    {
        while (!isDead)
        {
            Vector3 position = transform.position;
            position += Vector3.left * moveSpeed * Time.deltaTime;

            if (position.x <= stopX)
            {
                position.x = stopX;
                transform.position = position;
                yield break;
            }

            transform.position = position;
            yield return null;
        }
    }

    private void Move()
    {
        Vector3 position = transform.position;
        position += Vector3.up * moveDirection * moveSpeed * Time.deltaTime;

        float maxY = centerY + moveRange;
        float minY = centerY - moveRange;

        if (position.y >= maxY)
        {
            position.y = maxY;
            moveDirection = -1;
        }
        else if (position.y <= minY)
        {
            position.y = minY;
            moveDirection = 1;
        }

        transform.position = position;
    }

    private void HandleFire()
    {
        if (!hasEntered)
        {
            return;
        }

        fireTimer += Time.deltaTime;

        if (fireTimer < fireInterval)
        {
            return;
        }

        fireTimer = 0f;
        Fire();
    }

    private void Fire()
    {
        if (bulletPool == null || firePoint == null)
        {
            return;
        }

        Vector2 centerDirection = Vector2.left;
        Vector2 upDirection = RotateDirection(centerDirection, spreadAngle);
        Vector2 downDirection = RotateDirection(centerDirection, -spreadAngle);

        FireBullet(centerDirection);
        FireBullet(upDirection);
        FireBullet(downDirection);
    }

    private void FireBullet(Vector2 direction)
    {
        BossBullet bullet = bulletPool.GetBullet();

        if (bullet == null)
        {
            return;
        }

        bullet.transform.SetParent(null);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.Fire(direction, bulletSpeed, bulletDamage);
    }

    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        float x = direction.x * cos - direction.y * sin;
        float y = direction.x * sin + direction.y * cos;

        return new Vector2(x, y).normalized;
    }
}
