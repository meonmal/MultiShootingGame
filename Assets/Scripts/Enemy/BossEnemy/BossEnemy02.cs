using UnityEngine;
using System.Collections;

/// <summary>
/// 전체적으로 BossEnemy01과 같지만
/// 총알의 개수와 발사 각도가 조금 다르다.
/// 그 부분만 주석으로 적고 나머지는 BossEnemy01을 참고하길 바람.
/// </summary>
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
    /// <summary>
    /// 총알끼리의 간격.
    /// </summary>
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

    /// <summary>
    /// 2번 보스의 총알 발사 함수.
    /// 이 보스는 한번에 총알을 3개만 발사할 예정이다.
    /// </summary>
    private void Fire()
    {
        if (bulletPool == null || firePoint == null)
        {
            return;
        }

        // 우선 중심 축을 설정한다.
        Vector2 centerDirection = Vector2.left;

        // for문을 이용해서 총알이 한번에 3번 발사할 수 있도록 만들어준다.
        for(int i = -1; i <= 1; i++)
        {
            float angle = spreadAngle * i;
            Vector2 dir = RotateDirection(centerDirection, angle);
            FireBullet(dir);
        }
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

    /// <summary>
    /// 주어진 방향 벡터를 특정 각도만큼 회전시켜 반환하는 함수.
    /// Quaternion을 이용해 회전을 처리한다.
    /// </summary>
    /// <param name="direction">기준이 되는 방향 벡터.</param>
    /// <param name="angle">회전할 각도. (도 단위)</param>
    /// <returns>회전된 방향 벡터.</returns>
    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        return (Quaternion.Euler(0f, 0f, angle) * direction).normalized;
    }
}
