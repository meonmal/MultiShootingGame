using UnityEngine;

public class BuffItem : MonoBehaviour
{
    /// <summary>
    /// 버프 아이템의 생성 이후
    /// 플레이어가 먹지 않았을 경우 자동으로 사라지기까지의 시간.
    /// 쉽게 말해 버프아이템은 안 먹으면 5초 뒤에 풀로 반환된다.
    /// </summary>
    [SerializeField]
    private float lifeTime = 5f;

    /// <summary>
    /// 버프 데이터.
    /// </summary>
    private BuffData buffData;
    /// <summary>
    /// 사라지기까지 남은 시간.
    /// </summary>
    private float currentLifeTime;
    /// <summary>
    /// 버프 아이템을 관리하는 풀 매니저.
    /// </summary>
    private BuffItemPoolManager poolManager;
    /// <summary>
    /// 버프 아이템의 종류에 따른 이미지.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // SpriteRenderer 컴포넌트 정보 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// BuffItemPoolManager에서 실행할 초기화 함수.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="manager"></param>
    public void Init(BuffData data, BuffItemPoolManager manager)
    {
        // BuffData와 BuffItemPoolManager는 넘겨받은 정보로 초기화한다.
        buffData = data;
        poolManager = manager;
        // 버프 아이템이 새로 활성화된 시점이므로
        // 남은 수명을 전체 수명(lifeTime)으로 초기화한다.
        currentLifeTime = lifeTime;

        // 만약 spriteRenderer와 buffData가 존재하면(null이 아니면) 실행.
        if (spriteRenderer != null && buffData != null)
        {
            // 버프 아이템의 이미지는 BuffData의 icon으로 설정.
            spriteRenderer.sprite = buffData.icon;
        }

        // 게임 오브젝트 활성화
        gameObject.SetActive(true);
    }

    private void Update()
    {
        // 아직 먹지 않은 버프 아이템의 수명 감소.
        currentLifeTime -= Time.deltaTime;

        // 현재 남은 시간이 0과 같거나 작다면 실행.
        if (currentLifeTime <= 0f)
        {
            // 풀로 되돌린다.
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트에 Player 컴포넌트가 있는지 확인한다.
        // 버프 아이템은 플레이어와 닿았을 때만 먹히도록 처리한다.
        Player player = other.GetComponent<Player>();

        // 플레이어가 아닌 오브젝트와 충돌한 경우 실행.
        if (player == null)
        {
            // 버프를 적용하지 않고 함수를 종료한다.
            return;
        }

        // 버프를 먹으면 해당 사운드를 재생하고
        SoundManager.Instance.PlaySfx(SfxType.BuffPickup);
        // Player 스크립트에 있는 AddBuff()함수를 실행한다.
        player.AddBuff(buffData);
        // 해당 오브젝트를 풀로 되돌린다.
        ReturnToPool();
    }

    /// <summary>
    /// 해당 버프 아이템을 풀로 반환하는 함수.
    /// </summary>
    private void ReturnToPool()
    {
        // 풀 매니저가 있다면 실행
        if (poolManager != null)
        {
            // 이 버프 아이템을 풀 매니저로 반환한다.
            // 실제로 오브젝트를 삭제하지 않고 재사용이 가능한 상태로 돌려보내는 것이다.
            poolManager.Release(this);
        }
        // 만약 풀 매니저가 없다면 실행
        else
        {
            // 게임 오브젝트 비활성화.
            // 최소한 화면에서 제거하기 위함이다.
            gameObject.SetActive(false);
        }
    }
}
