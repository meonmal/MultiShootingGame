using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageble
{
    /// <summary>
    /// 적의 스탯.
    /// </summary>
    [SerializeField]
    private EnemyStats enemyStats;

    /// <summary>
    /// 적의 현재 체력.
    /// </summary>
    private float currentHP;
    /// <summary>
    /// 적이 풀로 반환될 위치.
    /// 쉽게 말해 x좌표가 -12 이하라면 풀로 반환된다.
    /// </summary>
    private float dieX = -12f;

    /// <summary>
    /// 적의 리지드바디2D.
    /// </summary>
    private Rigidbody2D rigid;
    /// <summary>
    /// 적을 담당할 오브젝트 풀.
    /// </summary>
    private IObjectPool<Enemy> _pool;
    /// <summary>
    /// 적이 죽으면 플레이어에게 경험치를 주기 위한 PlayerExperience 참조.
    /// </summary>
    private PlayerExperience _playerExperience;
    /// <summary>
    /// 적이 죽으면 버프를 생성하게 해줄 버프 드롭 매니저.
    /// </summary>
    private BuffDropManager buffDropManager;

    /// <summary>
    /// 풀을 지정하는 함수.
    /// </summary>
    /// <param name="pool">이 적을 관리할 오브젝트 풀.</param>
    public void SetPool(IObjectPool<Enemy> pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        // 리지드바디2D 컴포넌트 정보 가져오기.
        rigid = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 적의 초기화 함수.
    /// </summary>
    /// <param name="playerExperience">경험치 지급에 사용할 PlayerExperience 참조.</param>
    /// <param name="dropManager">버프 드롭 처리에 사용할 BuffDropManager 참조.</param>
    public void Init(PlayerExperience playerExperience, BuffDropManager dropManager)
    {
        _playerExperience = playerExperience;
        buffDropManager = dropManager;
        currentHP = enemyStats.EnemyHP;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적과 닿은 오브젝트에게 IDamageble 컴포넌트 정보를 얻어온다.
        IDamageble iDamageble = collision.GetComponent<IDamageble>();

        // 만약 IDamageble이 있고, 그 오브젝트의 태그가 Player라면 실행.
        if(iDamageble != null && collision.CompareTag("Player"))
        {
            // 플레이어에게 데미지를 주고 본인은 풀로 반환한다.
            iDamageble.TakeDamage(enemyStats.EnemyDamage);
            ThisRelease();
        }
    }

    /// <summary>
    /// 데미지를 받는 함수.
    /// </summary>
    /// <param name="damage">적이 받는 데미지.</param>
    public void TakeDamage(float damage)
    {
        // 데미지만큼 현재 체력을 줄인다.
        currentHP -= damage;
        SoundManager.Instance.PlaySfx(SfxType.EnemyHit);

        // 현재 체력이 0 이하라면 실행.
        if(currentHP <= 0)
        {
            // Die() 함수 실행.
            SoundManager.Instance.PlaySfx(SfxType.EnemyDead);
            Die();
        }
    }

    /// <summary>
    /// 적의 체력이 0 이하가 되면 실행될 함수.
    /// </summary>
    private void Die()
    {
        // PlayerExperience를 제대로 넘겨 받았다면 실행.
        if (_playerExperience != null)
        {
            // 플레이어에게 경험치를 준다.
            _playerExperience.AddExp(enemyStats.EnemyExp);
        }

        // BuffDropManager가 null이 아니라면 본인의 위치로 TryDrop() 실행.
        buffDropManager?.TryDrop(transform.position);

        // 풀이 없다면 그냥 삭제시킨다.
        if (_pool == null)
        {
            Destroy(gameObject);
            return;
        }

        // 본인을 풀로 반환.
        _pool.Release(this);
    }

    /// <summary>
    /// 적의 이동 함수.
    /// </summary>
    private void Movement()
    {
        // 적의 Rigidbody2D를 이용해서 왼쪽으로 이동시킨다.
        rigid.linearVelocity = Vector2.left * enemyStats.EnemySpeed;

        // 적의 x 좌표의 위치가 dieX를 넘어가면 풀로 반환한다.
        if(transform.position.x <= dieX)
        {
            // 적이 dieX를 넘어서 죽었을 때
            // 플레이어에게 경험치를 주는 것을 방지하기 위해
            // Die()가 아닌 ThisRelease()를 실행.
            ThisRelease();

            // 만약 풀이 없다면 그냥 삭제시킨다.
            if(_pool == null)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 본인을 풀로 반환시키는 함수.
    /// </summary>
    private void ThisRelease()
    {
        this._pool.Release(this);
    }
}
