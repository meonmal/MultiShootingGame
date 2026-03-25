using UnityEngine;
using System.Collections;

public class BossEnemy01 : BossBase
{
    /// <summary>
    /// 보스가 등장 후 이동을 멈출 x축 위치
    /// </summary>
    [SerializeField]
    private float stopX;
    /// <summary>
    /// 보스 총알의 오브젝트 풀.
    /// </summary>
    [SerializeField]
    private BossBulletPool bulletPool;
    /// <summary>
    /// 보스의 총알 발사 지점.
    /// </summary>
    [SerializeField]
    private Transform firePoint;
    /// <summary>
    /// 보스가 총알을 발사하는 시간 간격.
    /// </summary>
    [SerializeField]
    private float fireInterval;
    /// <summary>
    /// 보스 총알의 속도.
    /// </summary>
    [SerializeField]
    private float bulletSpeed;
    /// <summary>
    /// 보스 총알의 데미지.
    /// </summary>
    [SerializeField]
    private float bulletDamage;

    /// <summary>
    /// 보스의 이동 방향.
    /// </summary>
    private int moveDirection = 1;
    /// <summary>
    /// 마지막 발사 후 부터 누적된 시간.
    /// </summary>
    private float fireTimer;
    /// <summary>
    /// 보스가 지정한 위치까지 이동을 완료했는지 여부.
    /// </summary>
    private bool hasEntered;
    /// <summary>
    /// 보스가 이동할 때 기준이 되는 y축 중심 위치.
    /// </summary>
    private float centerY;

    /// <summary>
    /// 보스의 전체 행동 패턴을 실행하는 코루틴 함수.
    /// 등장 위치까지 이동한 뒤에 위 아래로 움직이며 총알을 발사한다.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator PatternRoutine()
    {
        // 우선 보스가 자기 위치(stopX)까지 이동을 할 때 까지 기다린다.
        yield return EnterRoutine();

        // 이동을 완료 했다면 초기화 진행.
        centerY = transform.position.y;
        fireTimer = 0f;
        moveDirection = 1;
        hasEntered = true;

        // 죽은 상태가 아니라면 이동과 발사를 반복해서 실행한다.
        while (!isDead)
        {
            Move();
            HandleFire();

            yield return null;
        }
    }

    /// <summary>
    /// 보스가 생성된 뒤에 stopX 위치까지 이동하게 만드는 코루틴 함수.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnterRoutine()
    {
        // 죽은 상태가 아니라면 실행.
        while (!isDead)
        {
            // 현재 위치에서 왼쪽 방향으로 이동시킨다.
            Vector3 position = transform.position;
            position += Vector3.left * moveSpeed * Time.deltaTime;

            // 만약 현재 x 위치가 stopX와 같거나 작아졌으면 실행.
            if (position.x <= stopX)
            {
                // 현재 x 위치를 stopX로 맞춘다.
                position.x = stopX;
                transform.position = position;
                yield break;
            }

            transform.position = position;
            yield return null;
        }
    }

    /// <summary>
    /// 보스 몬스터가 위 아래로 왔다 갔다 하게 만드는 함수.
    /// </summary>
    private void Move()
    {
        // 현재 이동 방향에 따라 보스를 위 또는 아래로 이동시킨다.
        Vector3 position = transform.position;
        position += Vector3.up * moveDirection * moveSpeed * Time.deltaTime;

        // 최대, 최소 y 위치를 설정해놓고
        float maxY = centerY + moveRange;
        float minY = centerY - moveRange;

        // 최대 y 값과 같거나 더 올라가게 되면
        if (position.y >= maxY)
        {
            // 현재 y 위치를 maxY로 맞추고
            // moveDirection을 -1로 맞춘다.
            position.y = maxY;
            moveDirection = -1;
        }
        // 그 반대도 마찬가지.
        else if (position.y <= minY)
        {
            position.y = minY;
            moveDirection = 1;
        }

        transform.position = position;
    }

    /// <summary>
    /// 발사 타이머를 누적하여 일정 시간마다 총알을 발사하는 함수.
    /// </summary>
    private void HandleFire()
    {
        // 아직 stopX까지 오지 않았다면 실행 종료.
        if (!hasEntered)
        {
            return;
        }

        // 경과 시간을 누적한다.
        fireTimer += Time.deltaTime;

        // fireTimer가 fireInterval보다 작으면 실행 종료.
        if (fireTimer < fireInterval)
        {
            return;
        }

        // fireTimer가 fireInterval과 같거나 커졌으면 발사 실행.
        fireTimer = 0f;
        Fire();
    }

    /// <summary>
    /// 보스의 총알 발사함수.
    /// </summary>
    private void Fire()
    {
        // 보스 총알 풀과 발사 지점이 없다면 실행 종료.
        if (bulletPool == null || firePoint == null)
        {
            return;
        }

        // 총알 풀에서 총알을 가져온다.
        BossBullet bullet = bulletPool.GetBullet();

        // 만약 총알이 없다면 실행 종료.
        if (bullet == null)
        {
            return;
        }

        // 총알이 풀 오브젝트의 자식 상태로 움직이지 않도록 부모를 해제한다.
        bullet.transform.SetParent(null);
        // 발사 위치와 회전을 설정한다.
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.identity;

        // 총알이 계속 왼쪽으로 향하게 만들고
        Vector2 direction = Vector2.left;
        // 총알에게 값을 넘겨준다.
        bullet.Fire(direction, bulletSpeed, bulletDamage);
    }
}
