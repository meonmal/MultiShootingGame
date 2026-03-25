using UnityEngine;

public class BossBullet : MonoBehaviour
{
    /// <summary>
    /// 총알이 발사되고 사라지기까지의 시간.
    /// 5초 정도로 설정할 예정이다.
    /// </summary>
    [SerializeField]
    private float lifeTime;

    /// <summary>
    /// 현재 남은 총알 수명.
    /// </summary>
    private float currentLifeTime;
    /// <summary>
    /// 총알의 이동 속도.
    /// </summary>
    private float moveSpeed;
    /// <summary>
    /// 총알의 데미지.
    /// </summary>
    private float damage;
    /// <summary>
    /// 총알의 이동 방향.
    /// </summary>
    private Vector2 moveDirection;

    /// <summary>
    /// 총알의 오브젝트 풀.
    /// </summary>
    private BossBulletPool pool;
    /// <summary>
    /// 총알이 활성화 되었는지 검사하는 변수.
    /// </summary>
    private bool isActiveBullet;

    /// <summary>
    /// BossBulletPool에서 실행할 초기화 함수.
    /// </summary>
    /// <param name="bossBulletPool"></param>
    public void Init(BossBulletPool bossBulletPool)
    {
        pool = bossBulletPool;
    }

    /// <summary>
    /// 보스가 총알을 발사하면 실행할 함수.
    /// </summary>
    /// <param name="direction">총알의 이동 방향.</param>
    /// <param name="speed">총알의 이동 속도.</param>
    /// <param name="bulletDamage">총알의 데미지.</param>
    public void Fire(Vector2 direction, float speed, float bulletDamage)
    {
        // 이동 방향, 이동 속도, 데미지는 보스 몬스터에게 넘겨 받은 값으로 저장한다.
        moveDirection = direction.normalized;
        moveSpeed = speed;
        damage = bulletDamage;

        // 현재 남은 수명은 최대 수명으로 맞추고
        currentLifeTime = lifeTime;
        // 활성화가 되었다고 판단한다.
        isActiveBullet = true;

        // 총알 게임 오브젝트 활성화.
        gameObject.SetActive(true);
    }

    private void Update()
    {
        // 만약 보스 총알이 활성화되지 않았다면 이 함수는 종료한다.
        if (!isActiveBullet)
        {
            return;
        }

        // 현재 위치에서 보스에게 넘겨 받은 이동 방향과 이동 속도로 움직인다.
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        // 현재 수명은 시간이 지날 수록 점점 줄어들게 만든다.
        currentLifeTime -= Time.deltaTime;

        // 만약 현재 수명이 다했다면 풀로 반환한다.
        if (currentLifeTime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 만약 활성화되지 않았다면 실행 취소.
        if (!isActiveBullet)
        {
            return;
        }

        // 이 총알과 닿은 오브젝트의 태그가 Player라면 실행.
        if (collision.CompareTag("Player"))
        {
            // 해당 오브젝트에게 IDamageble 컴포넌트 정보를 받아온다.
            IDamageble damageable = collision.GetComponent<IDamageble>();

            // IDamageble이 있다면 해당 오브젝트에게 데미지를 준다.
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // 풀로 반환.
            ReturnToPool();
        }
        // 만약 닿은 오브젝트의 태그가 Bullet이라면 실행. (플레이어의 총알)
        else if (collision.CompareTag("Bullet"))
        {
            // 다른 동작은 안 하고 풀로 반환하기만 한다.
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        // 애초에 활성화되지 않았다면 실행 취소.
        if (!isActiveBullet)
        {
            return;
        }

        // 만약 풀이 없다면 게임 오브젝트를 비활성화, 아래의 코드는 실행을 취소한다.
        if(pool == null)
        {
            gameObject.SetActive(false);
            return;
        }
            
        // 총알의 상태 초기화.
        isActiveBullet = false;
        moveDirection = Vector2.zero;
        moveSpeed = 0f;
        damage = 0f;
        currentLifeTime = 0f;

        // 풀로 반환한다.
        pool.ReturnBullet(this);
    }
}
