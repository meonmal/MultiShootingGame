using UnityEngine;
using UnityEngine.Pool;

public class BuffItemPoolManager : MonoBehaviour
{
    /// <summary>
    /// 생성할 BuffItem 프리팹.
    /// 버프의 종류가 여러가지이지만
    /// 버프 아이템은 SO만 바꿀 예정이기에
    /// 버프 아이템의 프리팹은 배열로 있을 필요가 없다.
    /// </summary>
    [SerializeField]
    private BuffItem buffItemPrefab;
    /// <summary>
    /// 초기 생성 개수.
    /// </summary>
    [SerializeField]
    private int defaultCapacity = 10;
    /// <summary>
    /// 최대 생성 개수.
    /// </summary>
    [SerializeField]
    private int maxSize = 50;

    /// <summary>
    /// 오브젝트 풀.
    /// </summary>
    private IObjectPool<BuffItem> pool;

    private void Awake()
    {
        // 버프 아이템 전용 오브젝트 풀을 생성한다.
        // 생성, 꺼낼 때, 반환할 때, 삭제할 때 실행할 함수를 각각 등록한다.
        pool = new ObjectPool<BuffItem>(
            CreateItem,
            OnGetItem,
            OnReleaseItem,
            OnDestroyItem,
            true,
            defaultCapacity,
            maxSize
        );
    }

    /// <summary>
    /// BuffItem을 생성하는 함수.
    /// </summary>
    /// <returns>생성한 버프 아이템을 반환한다.</returns>
    private BuffItem CreateItem()
    {
        // 풀이 새로운 버프 아이템을 필요로 할 때
        // BuffItem 프리팹을 1개 생성한다.
        BuffItem item = Instantiate(buffItemPrefab, transform);
        // 생성한 버프 아이템은 당장은 필요가 없으니 게임 오브젝트를 비활성화 한다.
        item.gameObject.SetActive(false);
        // 버프 아이템을 반환한다.
        return item;
    }

    /// <summary>
    /// 풀에서 꺼낸 버프 아이템을 활성화시키는 함수.
    /// 데이터 초기화는 Spawn()에서 별도로 진행한다.
    /// </summary>
    /// <param name="item">활성화 시킬 버프 아이템.</param>
    private void OnGetItem(BuffItem item)
    {
        // 버프 아이템 게임 오브젝트 활성화.
        item.gameObject.SetActive(true);
    }

    /// <summary>
    /// 버프 아이템을 비활성화 시키는 함수.
    /// </summary>
    /// <param name="item">비활성화 시킬 버프 아이템.</param>
    private void OnReleaseItem(BuffItem item)
    {
        // 버프 아이템 게임 오브젝트 비활성화.
        item.gameObject.SetActive(false);
    }

    /// <summary>
    /// 버프 아이템을 삭제시키는 함수.
    /// </summary>
    /// <param name="item">삭제 시킬 버프 아이템.</param>
    private void OnDestroyItem(BuffItem item)
    {
        // 버프 아이템 게임 오브젝트 삭제.
        Destroy(item.gameObject);
    }

    /// <summary>
    /// 버프 아이템을 풀에서 꺼내고 초기화해서 배치하는 함수.
    /// </summary>
    /// <param name="data">버프 아이템에 적용할 버프 데이터.</param>
    /// <param name="position">버프 아이템을 배치할 위치.</param>
    public void Spawn(BuffData data, Vector3 position)
    {
        // 버프 아이템을 풀에서 꺼낸다.
        BuffItem item = pool.Get();
        // 버프 아이템의 위치를 설정.(몬스터가 죽은 위치)
        item.transform.position = position;
        // 버프 데이터와 풀 매니저 정보를 전달하여 버프 아이템을 초기화한다.
        item.Init(data, this);
    }

    /// <summary>
    /// 버프 아이템을 풀로 반환하는 함수.
    /// </summary>
    /// <param name="item">반환할 버프 아이템.</param>
    public void Release(BuffItem item)
    {
        // 버프 아이템을 풀로 반환한다.
        pool.Release(item);
    }
}
